// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Threading.Tasks
{
	// <summary>
	// Helpers for safely using Task libraries. 
	// </summary>
	internal static class TaskHelpers
	{
		private static readonly Task _defaultCompleted = FromResult<AsyncVoid>(default(AsyncVoid));

		// <summary>
		// Returns a canceled Task. The task is completed, IsCanceled = True, IsFaulted = False.
		// </summary>
		private static Task Canceled()
		{
			return CancelCache<AsyncVoid>.Canceled;
		}

		// <summary>
		// Returns a canceled Task of the given type. The task is completed, IsCanceled = True, IsFaulted = False.
		// </summary>
		internal static Task<TResult> Canceled<TResult>()
		{
			return CancelCache<TResult>.Canceled;
		}

		// <summary>
		// Returns a completed task that has no result. 
		// </summary>        
		private static Task Completed()
		{
			return _defaultCompleted;
		}

		// <summary>
		// Returns an error task. The task is Completed, IsCanceled = False, IsFaulted = True
		// </summary>
		internal static Task FromError(Exception exception)
		{
			return FromError<AsyncVoid>(exception);
		}

		// <summary>
		// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
		// </summary>
		// <typeparam name="TResult"></typeparam>
		internal static Task<TResult> FromError<TResult>(Exception exception)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exception);
			return tcs.Task;
		}

		// <summary>
		// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
		// </summary>
		internal static Task<TResult> FromErrors<TResult>(IEnumerable<Exception> exceptions)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exceptions);
			return tcs.Task;
		}

		// <summary>
		// Returns a successful completed task with the given result.  
		// </summary>        
		internal static Task<TResult> FromResult<TResult>(TResult result)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			tcs.SetResult(result);
			return tcs.Task;
		}

		// <summary>
		// Return a task that runs all the tasks inside the iterator sequentially. It stops as soon
		// as one of the tasks fails or cancels, or after all the tasks have run successfully.
		// </summary>
		// <param name="asyncIterator">collection of tasks to wait on</param>
		// <param name="cancellationToken">cancellation token</param>
		// <param name="disposeEnumerator">whether or not to dispose the enumerator we get from <paramref name="asyncIterator"/>.
		// Only set to <c>false</c> if you can guarantee that <paramref name="asyncIterator"/>'s enumerator does not have any resources it needs to dispose.</param>
		// <returns>a task that signals completed when all the incoming tasks are finished.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception is propagated in a Task.")]
		internal static Task Iterate(IEnumerable<Task> asyncIterator, CancellationToken cancellationToken = default(CancellationToken), bool disposeEnumerator = true, Func<Task, bool> breakCondition = null)
		{
			IEnumerator<Task> enumerator = null;
			try
			{
				enumerator = asyncIterator.GetEnumerator();
				Task task = IterateImpl(enumerator, cancellationToken, breakCondition);
				return (disposeEnumerator && enumerator != null) ? task.Finally(enumerator.Dispose, runSynchronously: true) : task;
			}
			catch (Exception ex)
			{
				return TaskHelpers.FromError(ex);
			}
		}

		// <summary>
		// Provides the implementation of the Iterate method.
		// Contains special logic to help speed up common cases.
		// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The exception is propagated in a Task.")]
		private static Task IterateImpl(IEnumerator<Task> enumerator, CancellationToken cancellationToken, Func<Task, bool> breakCondition)
		{
			try
			{
				while (true)
				{
					// short-circuit: iteration canceled
					if (cancellationToken.IsCancellationRequested)
					{
						return TaskHelpers.Canceled();
					}

					// short-circuit: iteration complete
					if (!enumerator.MoveNext())
					{
						return TaskHelpers.Completed();
					}

					// fast case: Task completed synchronously & successfully
					Task currentTask = enumerator.Current;
					if (currentTask.Status == TaskStatus.RanToCompletion)
					{
						if (breakCondition != null && breakCondition(currentTask))
							return currentTask;

						continue;
					}

					// fast case: Task completed synchronously & unsuccessfully
					if (currentTask.IsCanceled || currentTask.IsFaulted)
					{
						return currentTask;
					}

					// slow case: Task isn't yet complete
					return IterateImplIncompleteTask(enumerator, currentTask, cancellationToken, breakCondition);
				}
			}
			catch (Exception ex)
			{
				return TaskHelpers.FromError(ex);
			}
		}

		// <summary>
		// Fallback for IterateImpl when the antecedent Task isn't yet complete.
		// </summary>
		private static Task IterateImplIncompleteTask(IEnumerator<Task> enumerator, Task currentTask, CancellationToken cancellationToken, Func<Task, bool> breakCondition)
		{
			// There's a race condition here, the antecedent Task could complete between
			// the check in Iterate and the call to Then below. If this happens, we could
			// end up growing the stack indefinitely. But the chances of (a) even having
			// enough Tasks in the enumerator in the first place and of (b) *every* one
			// of them hitting this race condition are so extremely remote that it's not
			// worth worrying about.

			return currentTask.Then(
				() => breakCondition != null && breakCondition(currentTask) ? Completed() : IterateImpl(enumerator, cancellationToken, breakCondition),
				runSynchronously: true
			);
		}

		internal static Task<TResult> RunSynchronously<TResult>(Func<TResult> func, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Canceled<TResult>();
			}

			try
			{
				return FromResult(func());
			}
			catch (Exception e)
			{
				return FromError<TResult>(e);
			}
		}

		// <summary>
		// Overload of RunSynchronously that avoids a call to Unwrap(). 
		// This overload is useful when func() starts doing some synchronous work and then hits IO and 
		// needs to create a task to finish the work. 
		// </summary>
		// <typeparam name="TResult">type of result that Task will return</typeparam>
		// <param name="func">function that returns a task</param>
		// <param name="cancellationToken">cancellation token. This is only checked before we run the task, and if canceled, we immediately return a canceled task.</param>
		// <returns>a task, created by running func().</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		internal static Task<TResult> RunSynchronously<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Canceled<TResult>();
			}

			try
			{
				return func();
			}
			catch (Exception e)
			{
				return FromError<TResult>(e);
			}
		}

		// <summary>
		// Set a completion source from the given Task.
		// </summary>
		// <typeparam name="TResult">result type for completion source.</typeparam>
		// <param name="tcs">completion source to set</param>
		// <param name="source">Task to get values from.</param>
		// <returns>true if this successfully sets the completion source.</returns>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "This is a known safe usage of Task.Result, since it only occurs when we know the task's state to be completed.")]
		internal static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> tcs, Task source)
		{
			if (source.Status == TaskStatus.Canceled)
			{
				return tcs.TrySetCanceled();
			}

			if (source.Status == TaskStatus.Faulted)
			{
				return tcs.TrySetException(source.Exception.InnerExceptions);
			}

			if (source.Status == TaskStatus.RanToCompletion)
			{
				Task<TResult> taskOfResult = source as Task<TResult>;
				return tcs.TrySetResult(taskOfResult == null ? default(TResult) : taskOfResult.Result);
			}

			return false;
		}

		// <summary>
		// Used as the T in a "conversion" of a Task into a Task{T}
		// </summary>
		private struct AsyncVoid
		{
		}

		// <summary>
		// This class is a convenient cache for per-type canceled tasks
		// </summary>
		private static class CancelCache<TResult>
		{
			public static readonly Task<TResult> Canceled = GetCancelledTask();

			private static Task<TResult> GetCancelledTask()
			{
				TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
				tcs.SetCanceled();
				return tcs.Task;
			}
		}
	}
}

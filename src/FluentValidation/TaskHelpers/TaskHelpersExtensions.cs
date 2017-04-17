// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Threading.Tasks
{
	using FluentValidation.Internal;

	internal static class TaskHelpersExtensions
	{
		private static readonly Type[] EmptyTypes = new Type[0];
		private static readonly Task<AsyncVoid> DefaultCompleted = TaskHelpers.FromResult<AsyncVoid>(default(AsyncVoid));
		private static readonly Action<Task> RethrowWithNoStackLossDelegate = GetRethrowWithNoStackLossDelegate();

		// <summary>
		// A version of task.Unwrap that is optimized to prevent unnecessarily capturing the
		// execution context when the antecedent task is already completed.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1202:DoNotUseProblematicTaskTypes", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<TResult> FastUnwrap<TResult>(this Task<Task<TResult>> task)
		{
			Task<TResult> innerTask = task.Status == TaskStatus.RanToCompletion ? task.Result : null;
			return innerTask ?? task.Unwrap();
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, regardless of the state
		// the task ended in. Intended to roughly emulate C# 5's support for "finally" in async methods.
		// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		internal static Task Finally(this Task task, Action continuation, bool runSynchronously = false)
		{
			// Stay on the same thread if we can
			if (task.IsCompleted)
			{
				try
				{
					continuation();
					return task;
				}
				catch (Exception ex)
				{
					MarkExceptionsObserved(task);
					return TaskHelpers.FromError(ex);
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return FinallyImplContinuation<AsyncVoid>(task, continuation, runSynchronously);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		private static Task<TResult> FinallyImplContinuation<TResult>(Task task, Action continuation, bool runSynchronously = false)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();

			task.ContinueWith(innerTask =>
			{
				try
				{
					continuation();
					tcs.TrySetFromTask(innerTask);
				}
				catch (Exception ex)
				{
					MarkExceptionsObserved(innerTask);
					tcs.TrySetException(ex);
				}
			}, runSynchronously ? TaskContinuationOptions.ExecuteSynchronously : TaskContinuationOptions.None);

			return tcs.Task;
		}

		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "This general exception is not intended to be seen by the user")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This general exception is not intended to be seen by the user")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		private static Action<Task> GetRethrowWithNoStackLossDelegate()
		{
#if NETFX_CORE
            return task => task.GetAwaiter().GetResult();
#else
			MethodInfo getAwaiterMethod = typeof(Task).GetTypeInfo().GetDeclaredMethod("GetAwaiter");
			if (getAwaiterMethod != null)
			{
				// .NET 4.5 - dump the same code the 'await' keyword would have dumped
				// >> task.GetAwaiter().GetResult()
				// No-ops if the task completed successfully, else throws the originating exception complete with the correct call stack.
				var taskParameter = Expression.Parameter(typeof(Task));
				var getAwaiterCall = Expression.Call(taskParameter, getAwaiterMethod);
				var getResultCall = Expression.Call(getAwaiterCall, "GetResult", EmptyTypes);
				var lambda = Expression.Lambda<Action<Task>>(getResultCall, taskParameter);
				return lambda.Compile();
			}
			else
			{
				Func<Exception, Exception> prepForRemoting = null;
#if !PORTABLE && !PORTABLE40 && !NETSTANDARD1_0
				try
				{
					if (AppDomain.CurrentDomain.IsFullyTrusted)
					{
						// .NET 4 - do the same thing Lazy<T> does by calling Exception.PrepForRemoting
						// This is an internal method in mscorlib.dll, so pass a test Exception to it to make sure we can call it.
						var exceptionParameter = Expression.Parameter(typeof(Exception));
						var prepForRemotingCall = Expression.Call(exceptionParameter, "PrepForRemoting", EmptyTypes);
						var lambda = Expression.Lambda<Func<Exception, Exception>>(prepForRemotingCall, exceptionParameter);
						var func = lambda.Compile();
						func(new Exception()); // make sure the method call succeeds before assigning the 'prepForRemoting' local variable
						prepForRemoting = func;
					}
				}
				catch
				{
				} // If delegate creation fails (medium trust) we will simply throw the base exception.
#endif
				return task =>
				{
					try
					{
						task.Wait();
					}
					catch (AggregateException ex)
					{
						Exception baseException = ex.GetBaseException();
						if (prepForRemoting != null)
						{
							baseException = prepForRemoting(baseException);
						}
						throw baseException;
					}
				};
			}
#endif
		}

		// <summary>
		// Marks a Task as "exception observed". The Task is required to have been completed first.
		// </summary>
		// <remarks>
		// Useful for 'finally' clauses, as if the 'finally' action throws we'll propagate the new
		// exception and lose track of the inner exception.
		// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "unused", Justification = "We only call the property getter for its side effect; we don't care about the value.")]
		private static void MarkExceptionsObserved(this Task task)
		{
			Exception unused = task.Exception;
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault).
		// </summary>
		internal static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<TOuterResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
		{
			return task.ThenImpl(t => TaskHelpers.FromResult(continuation()), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not cancelled and did not fault).
		// </summary>
		internal static Task Then(this Task task, Func<Task> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
		{
			return task.Then(() => continuation().Then(() => default(AsyncVoid)),
							 cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not cancelled and did not fault).
		// </summary>
		internal static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
		{
			return task.ThenImpl(t => continuation(), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not cancelled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task Then<TInnerResult>(this Task<TInnerResult> task, Action<TInnerResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
		{
			return task.ThenImpl(t => ToAsyncVoidTask(() => continuation(t.Result)), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task, Func<TInnerResult, TOuterResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
		{
			return task.ThenImpl(t => TaskHelpers.FromResult(continuation(t.Result)), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task Then<TInnerResult>(this Task<TInnerResult> task, Func<TInnerResult, Task> continuation, CancellationToken token = default(CancellationToken), bool runSynchronously = false)
		{
			return task.ThenImpl(t => continuation(t.Result).ToTask<AsyncVoid>(), token, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task, Func<TInnerResult, Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false)
		{
			return task.ThenImpl(t => continuation(t.Result), cancellationToken, runSynchronously);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		private static Task<TOuterResult> ThenImpl<TTask, TOuterResult>(this TTask task, Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken, bool runSynchronously)
			where TTask : Task
		{
			// Stay on the same thread if we can
			if (task.IsCompleted)
			{
				if (task.IsFaulted)
				{
					return TaskHelpers.FromErrors<TOuterResult>(task.Exception.InnerExceptions);
				}
				if (task.IsCanceled || cancellationToken.IsCancellationRequested)
				{
					return TaskHelpers.Canceled<TOuterResult>();
				}
				if (task.Status == TaskStatus.RanToCompletion)
				{
					try
					{
						return continuation(task);
					}
					catch (Exception ex)
					{
						return TaskHelpers.FromError<TOuterResult>(ex);
					}
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return ThenImplContinuation(task, continuation, cancellationToken, runSynchronously);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		private static Task<TOuterResult> ThenImplContinuation<TOuterResult, TTask>(TTask task, Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken, bool runSynchronously = false)
			where TTask : Task
		{
			TaskCompletionSource<Task<TOuterResult>> tcs = new TaskCompletionSource<Task<TOuterResult>>();

			task.ContinueWith(innerTask =>
			{
				if (innerTask.IsFaulted)
				{
					tcs.TrySetException(innerTask.Exception.InnerExceptions);
				}
				else if (innerTask.IsCanceled || cancellationToken.IsCancellationRequested)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					tcs.TrySetResult(continuation(task));
				}
			}, runSynchronously ? TaskContinuationOptions.ExecuteSynchronously : TaskContinuationOptions.None);

			return tcs.Task.FastUnwrap();
		}

		// <summary>
		// Throws the first faulting exception for a task which is faulted. It attempts to preserve the original
		// stack trace when throwing the exception (which should always work in 4.5, and should also work in 4.0
		// when running in full trust). Note: It is the caller's responsibility not to pass incomplete tasks to
		// this method, because it does degenerate into a call to the equivalent of .Wait() on the task when it
		// hasn't yet completed.
		// </summary>
		internal static void ThrowIfFaulted(this Task task)
		{
			RethrowWithNoStackLossDelegate(task);
		}

		// <summary>
		// Adapts any action into a Task (returning AsyncVoid, so that it's usable with Task{T} extension methods).
		// </summary>
		private static Task<AsyncVoid> ToAsyncVoidTask(Action action)
		{
			return TaskHelpers.RunSynchronously<AsyncVoid>(() =>
			{
				action();
				return DefaultCompleted;
			});
		}

		// <summary>
		// Changes the return value of a task to the given result, if the task ends in the RanToCompletion state.
		// This potentially imposes an extra ContinueWith to convert a non-completed task, so use this with caution.
		// </summary>
		internal static Task<TResult> ToTask<TResult>(this Task task, CancellationToken cancellationToken = default(CancellationToken), TResult result = default(TResult))
		{
			if (task == null)
			{
				return null;
			}

			// Stay on the same thread if we can
			if (task.IsCompleted)
			{
				if (task.IsFaulted)
				{
					return TaskHelpers.FromErrors<TResult>(task.Exception.InnerExceptions);
				}
				if (task.IsCanceled || cancellationToken.IsCancellationRequested)
				{
					return TaskHelpers.Canceled<TResult>();
				}
				if (task.Status == TaskStatus.RanToCompletion)
				{
					return TaskHelpers.FromResult(result);
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return ToTaskContinuation(task, result);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		private static Task<TResult> ToTaskContinuation<TResult>(Task task, TResult result)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();

			task.ContinueWith(innerTask =>
			{
				if (task.Status == TaskStatus.RanToCompletion)
				{
					tcs.TrySetResult(result);
				}
				else
				{
					tcs.TrySetFromTask(innerTask);
				}
			}, TaskContinuationOptions.ExecuteSynchronously);

			return tcs.Task;
		}

		// <summary>
		// Attempts to get the result value for the given task. If the task ran to completion, then
		// it will return true and set the result value; otherwise, it will return false.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static bool TryGetResult<TResult>(this Task<TResult> task, out TResult result)
		{
			if (task.Status == TaskStatus.RanToCompletion)
			{
				result = task.Result;
				return true;
			}

			result = default(TResult);
			return false;
		}

		// <summary>
		// Used as the T in a "conversion" of a Task into a Task{T}
		// </summary>
		private struct AsyncVoid
		{
		}
	}
}

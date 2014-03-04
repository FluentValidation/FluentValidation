namespace System.Threading.Tasks {
	using Diagnostics;
	using Diagnostics.CodeAnalysis;
	using Diagnostics.Contracts;
	using Linq.Expressions;

	static class TaskHelpersExtensions {
		static readonly Task<AsyncVoid> DefaultCompleted = TaskHelpers.FromResult(default(AsyncVoid));
		static readonly Action<Task> RethrowWithNoStackLossDelegate = GetRethrowWithNoStackLossDelegate();
		static readonly Type[] EmptyTypes = new Type[0];

		// <summary>
		// Calls the given continuation, after the given task completes, if it ends in a faulted or canceled state.
		// Will not be called if the task did not fault or cancel (meaning, it will not be called if the task ran
		// to completion). Intended to roughly emulate C# 5's support for "try/catch" in
		// async methods. Note that this method allows you to return a Task, so that you can either return
		// a completed Task (indicating that you swallowed the exception) or a faulted task (indicating that
		// that the exception should be propagated). In C#, you cannot normally use await within a catch
		// block, so returning a real async task should never be done from Catch().
		// </summary>
		internal static Task Catch(this Task task, Func<CatchInfo, CatchInfoBase<Task>.CatchResult> continuation, CancellationToken cancellationToken = default(CancellationToken)) {
			// Fast path for successful tasks, to prevent an extra TCS allocation
			if (task.Status == TaskStatus.RanToCompletion) {
				return task;
			}

			return task.CatchImpl(() => continuation(new CatchInfo(task, cancellationToken)).Task.ToTask<AsyncVoid>(), cancellationToken);
		}

		// <summary>
		// Calls the given continuation, after the given task completes, if it ends in a faulted or canceled state.
		// Will not be called if the task did not fault or cancel (meaning, it will not be called if the task ran
		// to completion). Intended to roughly emulate C# 5's support for "try/catch" in
		// async methods. Note that this method allows you to return a Task, so that you can either return
		// a completed Task (indicating that you swallowed the exception) or a faulted task (indicating that
		// that the exception should be propagated). In C#, you cannot normally use await within a catch
		// block, so returning a real async task should never be done from Catch().
		// </summary>
		internal static Task<TResult> Catch<TResult>(this Task<TResult> task, Func<CatchInfo<TResult>, CatchInfoBase<Task<TResult>>.CatchResult> continuation, CancellationToken cancellationToken = default(CancellationToken)) {
			// Fast path for successful tasks, to prevent an extra TCS allocation
			if (task.Status == TaskStatus.RanToCompletion) {
				return task;
			}
			return task.CatchImpl(() => continuation(new CatchInfo<TResult>(task, cancellationToken)).Task, cancellationToken);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CatchInfo", Justification = "This is the name of a class.")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TaskHelpersExtensions", Justification = "This is the name of a class.")]
		static Task<TResult> CatchImpl<TResult>(this Task task, Func<Task<TResult>> continuation, CancellationToken cancellationToken) {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				if (task.IsFaulted || task.IsCanceled || cancellationToken.IsCancellationRequested) {
					try {
						var resultTask = continuation();
						if (resultTask == null) {
							// Not a resource because this is an internal class, and this is a guard clause that's intended
							// to be thrown by us to us, never escaping out to end users.
							throw new InvalidOperationException("You must set the Task property of the CatchInfo returned from the TaskHelpersExtensions.Catch continuation.");
						}

						return resultTask;
					}
					catch (Exception ex) {
						return TaskHelpers.FromError<TResult>(ex);
					}
				}

				if (task.Status == TaskStatus.RanToCompletion) {
					var tcs = new TaskCompletionSource<TResult>();
					tcs.TrySetFromTask(task);
					return tcs.Task;
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return CatchImplContinuation(task, continuation);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TaskHelpersExtensions", Justification = "This is the name of a class.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		static Task<TResult> CatchImplContinuation<TResult>(Task task, Func<Task<TResult>> continuation) {
			var tcs = new TaskCompletionSource<Task<TResult>>();

			// this runs only if the inner task did not fault
			task.ContinueWith(innerTask => tcs.TrySetFromTask(innerTask), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

			// this runs only if the inner task faulted
			task.ContinueWith(innerTask => {
				try {
					var resultTask = continuation();
					if (resultTask == null) {
						throw new InvalidOperationException("You cannot return null from the TaskHelpersExtensions.Catch continuation. You must return a valid task or throw an exception.");
					}

					tcs.TrySetResult(resultTask);
				}
				catch (Exception ex) {
					tcs.TrySetException(ex);
				}
			}, TaskContinuationOptions.NotOnRanToCompletion);

			return tcs.Task.FastUnwrap();
		}

		// <summary>
		// Upon completion of the task, copies its result into the given task completion source, regardless of the
		// completion state. This causes the original task to be fully observed, and the task that is returned by
		// this method will always successfully run to completion, regardless of the original task state.
		// Since this method consumes a task with no return value, you must provide the return value to be used
		// when the inner task ran to successful completion.
		// </summary>
		internal static Task CopyResultToCompletionSource<TResult>(this Task task, TaskCompletionSource<TResult> tcs, TResult completionResult) {
			return task.CopyResultToCompletionSourceImpl(tcs, innerTask => completionResult);
		}

		// <summary>
		// Upon completion of the task, copies its result into the given task completion source, regardless of the
		// completion state. This causes the original task to be fully observed, and the task that is returned by
		// this method will always successfully run to completion, regardless of the original task state.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task CopyResultToCompletionSource<TResult>(this Task<TResult> task, TaskCompletionSource<TResult> tcs) {
			return task.CopyResultToCompletionSourceImpl(tcs, innerTask => innerTask.Result);
		}

		static Task CopyResultToCompletionSourceImpl<TTask, TResult>(this TTask task, TaskCompletionSource<TResult> tcs, Func<TTask, TResult> resultThunk)
			where TTask : Task {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				switch (task.Status) {
					case TaskStatus.Canceled:
					case TaskStatus.Faulted:
						tcs.TrySetFromTask(task);
						break;

					case TaskStatus.RanToCompletion:
						tcs.TrySetResult(resultThunk(task));
						break;
				}

				return TaskHelpers.Completed();
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return CopyResultToCompletionSourceImplContinuation(task, tcs, resultThunk);
		}

		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		static Task CopyResultToCompletionSourceImplContinuation<TTask, TResult>(TTask task, TaskCompletionSource<TResult> tcs, Func<TTask, TResult> resultThunk)
			where TTask : Task {
			return task.ContinueWith(innerTask => {
				switch (innerTask.Status) {
					case TaskStatus.Canceled:
					case TaskStatus.Faulted:
						tcs.TrySetFromTask(innerTask);
						break;

					case TaskStatus.RanToCompletion:
						tcs.TrySetResult(resultThunk(task));
						break;
				}
			}, TaskContinuationOptions.ExecuteSynchronously);
		}

		// <summary>
		// Cast Task to Task of object
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<object> CastToObject(this Task task) {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				if (task.IsFaulted) {
					return TaskHelpers.FromErrors<object>(task.Exception.InnerExceptions);
				}
				if (task.IsCanceled) {
					return TaskHelpers.Canceled<object>();
				}
				if (task.Status == TaskStatus.RanToCompletion) {
					return TaskHelpers.FromResult((object) null);
				}
			}

			var tcs = new TaskCompletionSource<object>();

			// schedule a synchronous task to cast: no need to worry about sync context or try/catch
			task.ContinueWith(innerTask => {
				if (innerTask.IsFaulted) {
					tcs.SetException(innerTask.Exception.InnerExceptions);
				}
				else if (innerTask.IsCanceled) {
					tcs.SetCanceled();
				}
				else {
					tcs.SetResult(null);
				}
			}, TaskContinuationOptions.ExecuteSynchronously);

			return tcs.Task;
		}

		// <summary>
		// Cast Task of T to Task of object
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<object> CastToObject<T>(this Task<T> task) {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				if (task.IsFaulted) {
					return TaskHelpers.FromErrors<object>(task.Exception.InnerExceptions);
				}
				if (task.IsCanceled) {
					return TaskHelpers.Canceled<object>();
				}
				if (task.Status == TaskStatus.RanToCompletion) {
					return TaskHelpers.FromResult((object) task.Result);
				}
			}

			var tcs = new TaskCompletionSource<object>();

			// schedule a synchronous task to cast: no need to worry about sync context or try/catch
			task.ContinueWith(innerTask => {
				if (innerTask.IsFaulted) {
					tcs.SetException(innerTask.Exception.InnerExceptions);
				}
				else if (innerTask.IsCanceled) {
					tcs.SetCanceled();
				}
				else {
					tcs.SetResult(innerTask.Result);
				}
			}, TaskContinuationOptions.ExecuteSynchronously);

			return tcs.Task;
		}

		// <summary>
		// Cast Task of object to Task of T
		// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<TOuterResult> CastFromObject<TOuterResult>(this Task<object> task) {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				if (task.IsFaulted) {
					return TaskHelpers.FromErrors<TOuterResult>(task.Exception.InnerExceptions);
				}
				if (task.IsCanceled) {
					return TaskHelpers.Canceled<TOuterResult>();
				}
				if (task.Status == TaskStatus.RanToCompletion) {
					try {
						return TaskHelpers.FromResult((TOuterResult) task.Result);
					}
					catch (Exception exception) {
						return TaskHelpers.FromError<TOuterResult>(exception);
					}
				}
			}

			var tcs = new TaskCompletionSource<TOuterResult>();

			// schedule a synchronous task to cast: no need to worry about sync context or try/catch
			task.ContinueWith(innerTask => {
				if (innerTask.IsFaulted) {
					tcs.SetException(innerTask.Exception.InnerExceptions);
				}
				else if (innerTask.IsCanceled) {
					tcs.SetCanceled();
				}
				else {
					try {
						tcs.SetResult((TOuterResult) innerTask.Result);
					}
					catch (Exception exception) {
						tcs.SetException(exception);
					}
				}
			}, TaskContinuationOptions.ExecuteSynchronously);

			return tcs.Task;
		}

		// <summary>
		// A version of task.Unwrap that is optimized to prevent unnecessarily capturing the
		// execution context when the antecedent task is already completed.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1202:DoNotUseProblematicTaskTypes", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task FastUnwrap(this Task<Task> task) {
			var innerTask = task.Status == TaskStatus.RanToCompletion ? task.Result : null;
			return innerTask ?? task.Unwrap();
		}

		// <summary>
		// A version of task.Unwrap that is optimized to prevent unnecessarily capturing the
		// execution context when the antecedent task is already completed.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1202:DoNotUseProblematicTaskTypes", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<TResult> FastUnwrap<TResult>(this Task<Task<TResult>> task) {
			var innerTask = task.Status == TaskStatus.RanToCompletion ? task.Result : null;
			return innerTask ?? task.Unwrap();
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, regardless of the state
		// the task ended in. Intended to roughly emulate C# 5's support for "finally" in async methods.
		// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		internal static Task Finally(this Task task, Action continuation, bool runSynchronously = false) {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				try {
					continuation();
					return task;
				}
				catch (Exception ex) {
					MarkExceptionsObserved(task);
					return TaskHelpers.FromError(ex);
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return FinallyImplContinuation<AsyncVoid>(task, continuation, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, regardless of the state
		// the task ended in. Intended to roughly emulate C# 5's support for "finally" in async methods.
		// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		internal static Task<TResult> Finally<TResult>(this Task<TResult> task, Action continuation, bool runSynchronously = false) {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				try {
					continuation();
					return task;
				}
				catch (Exception ex) {
					MarkExceptionsObserved(task);
					return TaskHelpers.FromError<TResult>(ex);
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return FinallyImplContinuation<TResult>(task, continuation, runSynchronously);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		static Task<TResult> FinallyImplContinuation<TResult>(Task task, Action continuation, bool runSynchronously = false) {
			var tcs = new TaskCompletionSource<TResult>();

			task.ContinueWith(innerTask => {
				try {
					continuation();
					tcs.TrySetFromTask(innerTask);
				}
				catch (Exception ex) {
					MarkExceptionsObserved(innerTask);
					tcs.TrySetException(ex);
				}
			}, runSynchronously ? TaskContinuationOptions.ExecuteSynchronously : TaskContinuationOptions.None);

			return tcs.Task;
		}

		[SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "This general exception is not intended to be seen by the user")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This general exception is not intended to be seen by the user")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		static Action<Task> GetRethrowWithNoStackLossDelegate() {
#if NETFX_CORE
            return task => task.GetAwaiter().GetResult();
#else
			var getAwaiterMethod = typeof (Task).GetMethod("GetAwaiter", EmptyTypes);
			if (getAwaiterMethod != null) {
				// .NET 4.5 - dump the same code the 'await' keyword would have dumped
				// >> task.GetAwaiter().GetResult()
				// No-ops if the task completed successfully, else throws the originating exception complete with the correct call stack.
				var taskParameter = Expression.Parameter(typeof (Task));
				var getAwaiterCall = Expression.Call(taskParameter, getAwaiterMethod);
				var getResultCall = Expression.Call(getAwaiterCall, "GetResult", EmptyTypes);
				var lambda = Expression.Lambda<Action<Task>>(getResultCall, taskParameter);
				return lambda.Compile();
			}
			Func<Exception, Exception> prepForRemoting = null;
#if !PORTABLE
			try {
				if (AppDomain.CurrentDomain.IsFullyTrusted) {
					// .NET 4 - do the same thing Lazy<T> does by calling Exception.PrepForRemoting
					// This is an internal method in mscorlib.dll, so pass a test Exception to it to make sure we can call it.
					var exceptionParameter = Expression.Parameter(typeof (Exception));
					var prepForRemotingCall = Expression.Call(exceptionParameter, "PrepForRemoting", EmptyTypes);
					var lambda = Expression.Lambda<Func<Exception, Exception>>(prepForRemotingCall, exceptionParameter);
					var func = lambda.Compile();
					func(new Exception()); // make sure the method call succeeds before assigning the 'prepForRemoting' local variable
					prepForRemoting = func;
				}
			}
			catch {
			} // If delegate creation fails (medium trust) we will simply throw the base exception.
#endif
			return task => {
				try {
					task.Wait();
				}
				catch (AggregateException ex) {
					var baseException = ex.GetBaseException();
					if (prepForRemoting != null) {
						baseException = prepForRemoting(baseException);
					}
					throw baseException;
				}
			};
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
		static void MarkExceptionsObserved(this Task task) {
			Contract.Assert(task.IsCompleted);

			Exception unused = task.Exception;
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault).
		// </summary>
		internal static Task Then(this Task task, Action continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {
			return task.ThenImpl(t => ToAsyncVoidTask(continuation), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault).
		// </summary>
		internal static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<TOuterResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {
			return task.ThenImpl(t => TaskHelpers.FromResult(continuation()), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not cancelled and did not fault).
		// </summary>
		internal static Task Then(this Task task, Func<Task> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {
			return task.Then(() => continuation().Then(() => default(AsyncVoid)),
				cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not cancelled and did not fault).
		// </summary>
		internal static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {
			return task.ThenImpl(t => continuation(), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not cancelled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task Then<TInnerResult>(this Task<TInnerResult> task, Action<TInnerResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {
			return task.ThenImpl(t => ToAsyncVoidTask(() => continuation(t.Result)), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task, Func<TInnerResult, TOuterResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {
			return task.ThenImpl(t => TaskHelpers.FromResult(continuation(t.Result)), cancellationToken, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task Then<TInnerResult>(this Task<TInnerResult> task, Func<TInnerResult, Task> continuation, CancellationToken token = default(CancellationToken), bool runSynchronously = false) {
			return task.ThenImpl(t => continuation(t.Result).ToTask<AsyncVoid>(), token, runSynchronously);
		}

		// <summary>
		// Calls the given continuation, after the given task has completed, if the task successfully ran
		// to completion (i.e., was not canceled and did not fault). The continuation is provided with the
		// result of the task as its sole parameter.
		// </summary>
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		internal static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task, Func<TInnerResult, Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {
			return task.ThenImpl(t => continuation(t.Result), cancellationToken, runSynchronously);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		static Task<TOuterResult> ThenImpl<TTask, TOuterResult>(this TTask task, Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken, bool runSynchronously)
			where TTask : Task {
			// Stay on the same thread if we can
			if (task.IsCompleted) {
				if (task.IsFaulted) {
					return TaskHelpers.FromErrors<TOuterResult>(task.Exception.InnerExceptions);
				}
				if (task.IsCanceled || cancellationToken.IsCancellationRequested) {
					return TaskHelpers.Canceled<TOuterResult>();
				}
				if (task.Status == TaskStatus.RanToCompletion) {
					try {
						return continuation(task);
					}
					catch (Exception ex) {
						return TaskHelpers.FromError<TOuterResult>(ex);
					}
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return ThenImplContinuation(task, continuation, cancellationToken, runSynchronously);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		static Task<TOuterResult> ThenImplContinuation<TOuterResult, TTask>(TTask task, Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken, bool runSynchronously = false)
			where TTask : Task {
			var tcs = new TaskCompletionSource<Task<TOuterResult>>();

			task.ContinueWith(innerTask => {
				if (innerTask.IsFaulted) {
					tcs.TrySetException(innerTask.Exception.InnerExceptions);
				}
				else if (innerTask.IsCanceled || cancellationToken.IsCancellationRequested) {
					tcs.TrySetCanceled();
				}
				else {
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
		internal static void ThrowIfFaulted(this Task task) {
			RethrowWithNoStackLossDelegate(task);
		}

		// <summary>
		// Adapts any action into a Task (returning AsyncVoid, so that it's usable with Task{T} extension methods).
		// </summary>
		static Task<AsyncVoid> ToAsyncVoidTask(Action action) {
			return TaskHelpers.RunSynchronously(() => {
				action();
				return DefaultCompleted;
			});
		}

		// <summary>
		// Changes the return value of a task to the given result, if the task ends in the RanToCompletion state.
		// This potentially imposes an extra ContinueWith to convert a non-completed task, so use this with caution.
		// </summary>
		internal static Task<TResult> ToTask<TResult>(this Task task, CancellationToken cancellationToken = default(CancellationToken), TResult result = default(TResult)) {
			if (task == null) {
				return null;
			}

			// Stay on the same thread if we can
			if (task.IsCompleted) {
				if (task.IsFaulted) {
					return TaskHelpers.FromErrors<TResult>(task.Exception.InnerExceptions);
				}
				if (task.IsCanceled || cancellationToken.IsCancellationRequested) {
					return TaskHelpers.Canceled<TResult>();
				}
				if (task.Status == TaskStatus.RanToCompletion) {
					return TaskHelpers.FromResult(result);
				}
			}

			// Split into a continuation method so that we don't create a closure unnecessarily
			return ToTaskContinuation(task, result);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
		[SuppressMessage("Microsoft.Web.FxCop", "MW1201:DoNotCallProblematicMethodsOnTask", Justification = "The usages here are deemed safe, and provide the implementations that this rule relies upon.")]
		static Task<TResult> ToTaskContinuation<TResult>(Task task, TResult result) {
			var tcs = new TaskCompletionSource<TResult>();

			task.ContinueWith(innerTask => {
				if (task.Status == TaskStatus.RanToCompletion) {
					tcs.TrySetResult(result);
				}
				else {
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
		internal static bool TryGetResult<TResult>(this Task<TResult> task, out TResult result) {
			if (task.Status == TaskStatus.RanToCompletion) {
				result = task.Result;
				return true;
			}

			result = default(TResult);
			return false;
		}

		// <summary>
		// Used as the T in a "conversion" of a Task into a Task{T}
		// </summary>
		struct AsyncVoid {
		}
	}

	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Packaged as one file to make it easy to link against")]
	abstract class CatchInfoBase<TTask>
		where TTask : Task {
		readonly Exception _exception;
		readonly TTask _task;

		protected CatchInfoBase(TTask task, CancellationToken cancellationToken) {
			Contract.Assert(task != null);
			_task = task;
			if (task.IsFaulted) {
				_exception = _task.Exception.GetBaseException(); // Observe the exception early, to prevent tasks tearing down the app domain
			}
			else if (task.IsCanceled) {
				_exception = new TaskCanceledException(task);
			}
			else {
				Debug.Assert(cancellationToken.IsCancellationRequested);
				_exception = new OperationCanceledException(cancellationToken);
			}
		}

		protected TTask Task {
			get { return _task; }
		}

		// <summary>
		// The exception that was thrown to cause the Catch block to execute.
		// </summary>
		public Exception Exception {
			get { return _exception; }
		}

		// <summary>
		// Represents a result to be returned from a Catch handler.
		// </summary>
		internal struct CatchResult {
			// <summary>
			// Gets or sets the task to be returned to the caller.
			// </summary>
			internal TTask Task { get; set; }
		}
	}

	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Packaged as one file to make it easy to link against")]
	class CatchInfo : CatchInfoBase<Task> {
		static readonly CatchResult _completed = new CatchResult {Task = TaskHelpers.Completed()};

		public CatchInfo(Task task, CancellationToken cancellationToken)
			: base(task, cancellationToken) {
		}

		// <summary>
		// Returns a CatchResult that returns a completed (non-faulted) task.
		// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This would result in poor usability.")]
		public CatchResult Handled() {
			return _completed;
		}

		// <summary>
		// Returns a CatchResult that executes the given task and returns it, in whatever state it finishes.
		// </summary>
		// <param name="task">The task to return.</param>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This would result in poor usability.")]
		public CatchResult Task(Task task) {
			return new CatchResult {Task = task};
		}

		// <summary>
		// Returns a CatchResult that re-throws the original exception.
		// </summary>
		public CatchResult Throw() {
			if (base.Task.IsFaulted || base.Task.IsCanceled) {
				return new CatchResult {Task = base.Task};
			}
			// Canceled via CancelationToken
			return new CatchResult {Task = TaskHelpers.Canceled()};
		}

		// <summary>
		// Returns a CatchResult that throws the given exception.
		// </summary>
		// <param name="ex">The exception to throw.</param>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This would result in poor usability.")]
		public CatchResult Throw(Exception ex) {
			return new CatchResult {Task = TaskHelpers.FromError<object>(ex)};
		}
	}

	[SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Packaged as one file to make it easy to link against")]
	class CatchInfo<T> : CatchInfoBase<Task<T>> {
		public CatchInfo(Task<T> task, CancellationToken cancellationToken)
			: base(task, cancellationToken) {
		}

		// <summary>
		// Returns a CatchResult that returns a completed (non-faulted) task.
		// </summary>
		// <param name="returnValue">The return value of the task.</param>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This would result in poor usability.")]
		public CatchResult Handled(T returnValue) {
			return new CatchResult {Task = TaskHelpers.FromResult(returnValue)};
		}

		// <summary>
		// Returns a CatchResult that executes the given task and returns it, in whatever state it finishes.
		// </summary>
		// <param name="task">The task to return.</param>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This would result in poor usability.")]
		public CatchResult Task(Task<T> task) {
			return new CatchResult {Task = task};
		}

		// <summary>
		// Returns a CatchResult that re-throws the original exception.
		// </summary>
		public CatchResult Throw() {
			if (base.Task.IsFaulted || base.Task.IsCanceled) {
				return new CatchResult {Task = base.Task};
			}
			// Canceled via CancelationToken
			return new CatchResult {Task = TaskHelpers.Canceled<T>()};
		}

		// <summary>
		// Returns a CatchResult that throws the given exception.
		// </summary>
		// <param name="ex">The exception to throw.</param>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This would result in poor usability.")]
		public CatchResult Throw(Exception ex) {
			return new CatchResult {Task = TaskHelpers.FromError<T>(ex)};
		}
	}
}
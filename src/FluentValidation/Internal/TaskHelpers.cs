// --------------------------------------------------------------------------
//  <copyright file="TaskHelpers.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------
namespace FluentValidation.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class TaskHelpers
    {
        /// <summary>
        /// Used as the T in a "conversion" of a Task into a Task{T}
        /// </summary>
        private struct AsyncVoid
        {
        }

        const TaskContinuationOptions SyncSuccess = TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion;

        /// <summary>
        /// This class is a convenient cache for per-type cancelled tasks
        /// </summary>
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

        private static readonly Task defaultCompleted = FromResult(default(AsyncVoid));

        /// <summary>
        /// Returns a successful completed task with the given result.  
        /// </summary>        
        internal static Task<TResult> FromResult<TResult>(TResult result)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>
        /// Returns a canceled Task. The task is completed, IsCanceled = True, IsFaulted = False.
        /// </summary>
        internal static Task Canceled()
        {
            return CancelCache<AsyncVoid>.Canceled;
        }

        /// <summary>
        /// Returns a canceled Task of the given type. The task is completed, IsCanceled = True, IsFaulted = False.
        /// </summary>
        internal static Task<TResult> Canceled<TResult>()
        {
            return CancelCache<TResult>.Canceled;
        }

        /// <summary>
        /// Returns a completed task that has no result. 
        /// </summary>        
        internal static Task Completed()
        {
            return defaultCompleted;
        }

        /// <summary>
        /// Returns an error task. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        internal static Task FromError(Exception exception)
        {
            return FromError<AsyncVoid>(exception);
        }

        /// <summary>
        /// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        internal static Task<TResult> FromError<TResult>(Exception exception)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        public static Task<IEnumerable<TResult>> SelectManyAsync<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, Task<IEnumerable<TResult>>> projection)
        {
            var result = new List<TResult>();

            return
                items
                .Select(item => 
                    projection(item)
                    .ContinueWith(t =>
                    {
                        result.AddRange(t.Result);
                        return t.Result;
                    }, SyncSuccess)
                ).Iterate()
                .ContinueWith(_ => result.AsEnumerable(), SyncSuccess)
            ;
        }

        public static Task Iterate<TResult>(this IEnumerable<Task<TResult>> enumerable, Func<TResult, bool> breakCondition = null)
        {
            var enumerator = enumerable.GetEnumerator();

            return 
                enumerator
                .Iterate(breakCondition)
                .ContinueWith(_ => enumerator.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task Iterate<TResult>(this IEnumerator<Task<TResult>> enumerator, Func<TResult, bool> breakCondition = null)
        {
            try
            {
                while (true)
                {
                    // short-circuit: iteration complete
                    if (!enumerator.MoveNext())
                    {
                        return TaskHelpers.Completed();
                    }

                    // fast case: Task completed synchronously & successfully
                    var currentTask = enumerator.Current;
                    if (currentTask.Status == TaskStatus.RanToCompletion)
                    {
                        if (breakCondition != null && breakCondition(currentTask.Result))
                            return currentTask;

                        continue;
                    }

                    // fast case: Task completed synchronously & unsuccessfully
                    if (currentTask.IsCanceled || currentTask.IsFaulted)
                    {
                        return currentTask;
                    }

                    // slow case: Task isn't yet complete
                    return
                        currentTask
                        .ContinueWith(
                            t => breakCondition != null && breakCondition(t.Result) ? TaskHelpers.Completed() : Iterate(enumerator, breakCondition),
                            SyncSuccess
                        ).Unwrap();
                }
            }
            catch (Exception ex)
            {
                return TaskHelpers.FromError(ex);
            }
        }
    }
}
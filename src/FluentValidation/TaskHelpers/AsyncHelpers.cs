

namespace System.Threading.Tasks
{
	using Collections.Generic;
	using Runtime.ExceptionServices;

	/// <summary>
	/// Implementation taken from http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously/5097066#5097066
	/// with modified exception throwing, so that the original stacktrace is preserverd by using ExceptionDispatchInfo.Capture
	/// </summary>
	internal static class AsyncHelpers {

		/// <summary>
		/// Execute's an async <see cref="T:System.Threading.Tasks.Task" /> method which has a void return value synchronously
		/// </summary>
		/// <param name="task"><see cref="T:System.Threading.Tasks.Task" /> method to execute</param>
		public static void RunSync(Func<Task> task) {
			var oldContext = SynchronizationContext.Current;
			var synch = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(synch);
			synch.Post(async _ => {
				try {
					await task();
				}
				catch (Exception e) {
					synch.InnerException = e;
					throw;
				}
				finally {
					synch.EndMessageLoop();
				}
			}, null);
			synch.BeginMessageLoop();
			SynchronizationContext.SetSynchronizationContext(oldContext);
			if (synch.InnerException != null) {
				ExceptionDispatchInfo.Capture(synch.InnerException).Throw();
			}
		}

		/// <summary>
		/// Execute's an async <see cref="T:System.Threading.Tasks.Task`1" /> method which has a T return type synchronously
		/// </summary>
		/// <typeparam name="T">Return Type</typeparam>
		/// <param name="task"><see cref="T:System.Threading.Tasks.Task`1" /> method to execute</param>
		/// <returns></returns>
		public static T RunSync<T>(Func<Task<T>> task) {
			var oldContext = SynchronizationContext.Current;
			var synch = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(synch);
			var ret = default(T);
			synch.Post(async _ => {
				try {
					ret = await task();
				}
				catch (Exception e) {
					synch.InnerException = e;
					throw;
				}
				finally {
					synch.EndMessageLoop();
				}
			}, null);
			synch.BeginMessageLoop();
			SynchronizationContext.SetSynchronizationContext(oldContext);
			if (synch.InnerException != null) {
				ExceptionDispatchInfo.Capture(synch.InnerException).Throw();
			}
			return ret;
		}

		private class ExclusiveSynchronizationContext : SynchronizationContext {
			private bool done;
			public Exception InnerException { get; set; }
			readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
			readonly Queue<Tuple<SendOrPostCallback, object>> items = new Queue<Tuple<SendOrPostCallback, object>>();

			public override void Send(SendOrPostCallback d, object state) {
				throw new NotSupportedException("We cannot send to our same thread");
			}

			public override void Post(SendOrPostCallback d, object state) {
				lock (items) {
					items.Enqueue(Tuple.Create(d, state));
				}
				workItemsWaiting.Set();
			}

			public void EndMessageLoop() {
				Post(_ => done = true, null);
			}

			public void BeginMessageLoop() {
				while (!done) {
					Tuple<SendOrPostCallback, object> task = null;
					lock (items) {
						if (items.Count > 0) {
							task = items.Dequeue();
						}
					}
					if (task != null) {
						task.Item1(task.Item2);
					}
					else {
						workItemsWaiting.WaitOne();
					}
				}
			}

			public override SynchronizationContext CreateCopy() {
				return this;
			}
		}
	}
}

#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

#pragma warning disable 1591
namespace FluentValidation.Internal {
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class TrackingCollection<T> : IEnumerable<T> {
		readonly List<T> innerCollection = new List<T>();
		public event Action<T> ItemAdded;

		public void Add(T item) {
			innerCollection.Add(item);
			ItemAdded?.Invoke(item);
		}

		public void Remove(T item) {
			innerCollection.Remove(item);
		}

		public IDisposable OnItemAdded(Action<T> onItemAdded) {
			ItemAdded += onItemAdded;
			return new EventDisposable(this, onItemAdded);
		}

		public IEnumerator<T> GetEnumerator() {
			return innerCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		class EventDisposable : IDisposable {
			readonly TrackingCollection<T> parent;
			readonly Action<T> handler;

			public EventDisposable(TrackingCollection<T> parent, Action<T> handler) {
				this.parent = parent;
				this.handler = handler;
			}

			public void Dispose() {
				parent.ItemAdded -= handler;
			}
		}
	}
}
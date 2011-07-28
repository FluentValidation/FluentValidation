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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using Internal;
	using NUnit.Framework;
	using System.Linq;

	[TestFixture]
	public class TrackingCollectionTests {
		[Test]
		public void Add_AddsItem() {
			var items = new TrackingCollection<string>();
			items.Add("foo");
			items.Single().ShouldEqual("foo");
		}

		[Test]
		public void When_Item_Added_Raises_ItemAdded() {
			string addedItem = null;
			var items = new TrackingCollection<string>();

			using(items.OnItemAdded(x => addedItem = x)) {
				items.Add("foo");
			}

			addedItem.ShouldEqual("foo");
		}

		[Test]
		public void Should_not_raise_event_once_handler_detached() {
			var addedItems = new List<string>();
			var items = new TrackingCollection<string>();
			
			using(items.OnItemAdded(addedItems.Add)) {
				items.Add("foo");
			}
			items.Add("bar");

			addedItems.Count.ShouldEqual(1);
		}
	}
}
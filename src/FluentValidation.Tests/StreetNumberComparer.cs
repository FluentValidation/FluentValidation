#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	class StreetNumberComparer : IComparer<Address> {

		bool TryParseStreetNumber(string s, out int streetNumber) {
			var streetNumberStr = s.Substring(0, s.IndexOf(" "));
			return int.TryParse(streetNumberStr, out streetNumber);
		}

		int GetValue(object o) {
			return o is Address addr && TryParseStreetNumber(addr.Line1, out var streetNumber)
				? streetNumber
				: throw new ArgumentException("Can't convert", nameof(o));
		}
		public int Compare([AllowNull] Address x, [AllowNull] Address y) {
			if (x == y) {
				return 0;
			}
			if (x == null) {
				return -1;
			}
			if (y == null) {
				return 1;
			}
			return GetValue(x).CompareTo(GetValue(y));
		}
	}
}

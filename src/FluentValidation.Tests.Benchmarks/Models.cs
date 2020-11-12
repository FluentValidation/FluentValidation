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

namespace FluentValidation.Tests.Benchmarks {
	using System;
	using System.Collections.Generic;

	public class FullModel {
		public string Text1 { get; set; }
		public string Text2 { get; set; }
		public string Text3 { get; set; }
		public string Text4 { get; set; }
		public string Text5 { get; set; }

		public int Number1 { get; set; }

		public int Number2 { get; set; }

		public int Number3 { get; set; }

		public int Number4 { get; set; }

		public int Number5 { get; set; }

		public decimal? SuperNumber1 { get; set; }

		public decimal? SuperNumber2 { get; set; }

		public decimal? SuperNumber3 { get; set; }

		public NestedModel NestedModel1 { get; set; }

		public NestedModel NestedModel2 { get; set; }

		public IReadOnlyList<NestedModel> ModelCollection { get; set; }

		public IReadOnlyList<int> StructCollection { get; set; }
	}

	public class NestedModel {
		public string Text1 { get; set; }

		public string Text2 { get; set; }

		public int Number1 { get; set; }

		public int Number2 { get; set; }

		public decimal? SuperNumber1 { get; set; }

		public decimal? SuperNumber2 { get; set; }
	}

	public class FullModelValidator : AbstractValidator<FullModel> {
		public new CascadeMode CascadeMode {
			get => base.CascadeMode;
			set => base.CascadeMode = value;
		}
		public FullModelValidator() {
			RuleFor(x => x.Text1).NotNull();
			RuleFor(x => x.Text1).Must(m => m.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T1").When(m => m.Text1 != null);

			RuleFor(x => x.Text2).NotNull();
			RuleFor(x => x.Text2).Must(m => m.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T2").When(m => m.Text2 != null);

			RuleFor(x => x.Text3).NotNull();
			RuleFor(x => x.Text3).Must(m => m.Contains('c', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T3").When(m => m.Text3 != null);

			RuleFor(x => x.Text4).NotNull();
			RuleFor(x => x.Text4).Must(m => m.Contains('d', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T4").When(m => m.Text4 != null);

			RuleFor(x => x.Text5).NotNull();
			RuleFor(x => x.Text5).Must(m => m.Contains('e', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T5").When(m => m.Text5 != null);

			RuleFor(x => x.Number1).Must(m => m < 10).WithMessage("Message N1");
			RuleFor(x => x.Number2).Must(m => m < 10).WithMessage("Message N2");
			RuleFor(x => x.Number3).Must(m => m < 10).WithMessage("Message N3");
			RuleFor(x => x.Number4).Must(m => m < 10).WithMessage("Message N4");
			RuleFor(x => x.Number5).Must(m => m < 10).WithMessage("Message N5");

			RuleFor(x => x.SuperNumber1).NotNull();
			RuleFor(x => x.SuperNumber1).Must(m => m < 10).WithMessage("Message S1").When(m => m.SuperNumber1 != null);

			RuleFor(x => x.SuperNumber2).NotNull();
			RuleFor(x => x.SuperNumber2).Must(m => m < 10).WithMessage("Message S2").When(m => m.SuperNumber2 != null);

			RuleFor(x => x.SuperNumber3).NotNull();
			RuleFor(x => x.SuperNumber3).Must(m => m < 10).WithMessage("Message S3").When(m => m.SuperNumber3 != null);

			RuleFor(x => x.NestedModel1).NotNull();
			RuleFor(x => x.NestedModel1).SetValidator(new NestedModelValidator()).When(m => m.NestedModel1 != null);

			RuleFor(x => x.NestedModel2).NotNull();
			RuleFor(x => x.NestedModel2).SetValidator(new NestedModelValidator()).When(m => m.NestedModel2 != null);

			RuleFor(x => x.ModelCollection).NotNull();
			RuleFor(x => x.ModelCollection)
				.Must(x => x.Count <= 10).WithMessage("No more than 10 items are allowed")
				.When(m => m.ModelCollection != null);

			RuleForEach(x => x.ModelCollection).SetValidator(new NestedModelValidator()).When(m => m.ModelCollection != null);

			RuleFor(x => x.StructCollection).NotNull();
			RuleFor(x => x.StructCollection)
				.Must(x => x.Count <= 10).WithMessage("No more than 10 items are allowed")
				.When(m => m.StructCollection != null);

			RuleForEach(x => x.StructCollection).Must(m1 => m1 <= 10).WithMessage("Message C").When(m => m.StructCollection != null);
		}
	}

	public class NestedModelValidator : AbstractValidator<NestedModel> {
		public NestedModelValidator() {
			RuleFor(x => x.Text1).NotNull();
			RuleFor(x => x.Text1).Must(m => m.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T1").When(m => m.Text1 != null);

			RuleFor(x => x.Text2).NotNull();
			RuleFor(x => x.Text2).Must(m => m.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T2").When(m => m.Text2 != null);

			RuleFor(x => x.Number1).Must(m => m < 10).WithMessage("Nested Message N1");
			RuleFor(x => x.Number2).Must(m => m < 10).WithMessage("Nested Message N2");

			RuleFor(x => x.SuperNumber1).NotNull();
			RuleFor(x => x.SuperNumber1).Must(m => m < 10).WithMessage("Nested Message S1").When(m => m.SuperNumber1 != null);

			RuleFor(x => x.SuperNumber2).NotNull();
			RuleFor(x => x.SuperNumber2).Must(m => m < 10).WithMessage("Nested Message S2").When(m => m.SuperNumber2 != null);
		}
	}
}

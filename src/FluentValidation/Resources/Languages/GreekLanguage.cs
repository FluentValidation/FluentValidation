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

#pragma warning disable 618

namespace FluentValidation.Resources {
	using Validators;

	internal class GreekLanguage {
		public const string Culture = "el";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "Το πεδίο '{PropertyName}' δεν περιέχει μια έγκυρη διεύθυνση email.",
			nameof(GreaterThanOrEqualValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη ή ίση με '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη από '{ComparisonValue}'.",
			nameof(LengthValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει μήκος μεταξύ {MinLength} και {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.",
			nameof(MinimumLengthValidator) => "Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι τουλάχιστον {MinLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.",
			nameof(MaximumLengthValidator) => "Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι το πολύ {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.",
			nameof(LessThanOrEqualValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μικρότερη ή ίση με '{ComparisonValue}'.",
			nameof(LessThanValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μικρότερη από '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "Το πεδίο '{PropertyName}' δεν πρέπει να είναι κενό.",
			nameof(NotEqualValidator) => "Το πεδίο '{PropertyName}' δεν πρέπει να έχει τιμή ίση με '{ComparisonValue}'.",
			nameof(NotNullValidator) => "Το πεδίο '{PropertyName}' δεν πρέπει να είναι κενό.",
			nameof(PredicateValidator) => "Η ορισμένη συνθήκη δεν ικανοποιήθηκε για το πεδίο '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Η ορισμένη συνθήκη δεν ικανοποιήθηκε για το πεδίο '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "Η τιμή του πεδίου '{PropertyName}' δεν έχει αποδεκτή μορφή.",
			nameof(EqualValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή ίση με '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει μήκος ίσο με {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.",
			nameof(InclusiveBetweenValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεταξύ {From} και {To}. Καταχωρίσατε την τιμή {Value}.",
			nameof(ExclusiveBetweenValidator) => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη από {From} και μικρότερη από {To}. Καταχωρίσατε την τιμή  {Value}.",
			nameof(CreditCardValidator) => "Το πεδίο '{PropertyName}' δεν περιέχει αποδεκτό αριθμό πιστωτικής κάρτας.",
			nameof(ScalePrecisionValidator) => "'Το πεδίο '{PropertyName}' δεν μπορεί να έχει περισσότερα από {ExpectedPrecision} ψηφία στο σύνολο, με μέγιστο επιτρεπόμενο αριθμό δεκαδικών τα {ExpectedScale} ψηφία. Έχετε καταχωρίσει {Digits} ψηφία συνολικά με {ActualScale} δεκαδικά.",
			nameof(EmptyValidator) => "Το πεδίο '{PropertyName}' πρέπει να είναι κενό.",
			nameof(NullValidator) => "Το πεδίο '{PropertyName}' πρέπει να είναι κενό.",
			nameof(EnumValidator) => "Το πεδίο '{PropertyName}' επιτρέπει συγκεκριμένο εύρος τιμών που δεν περιλαμβάνουν την τιμή '{PropertyValue}' που καταχωρίσατε.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "Το πεδίο '{PropertyName}' πρέπει να έχει μήκος μεταξύ {MinLength} και {MaxLength} χαρακτήρες.",
			"MinimumLength_Simple" => "Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι τουλάχιστον {MinLength} χαρακτήρες.",
			"MaximumLength_Simple" => "Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι το πολύ {MaxLength} χαρακτήρες.",
			"ExactLength_Simple" => "Το πεδίο '{PropertyName}' πρέπει να έχει μήκος ίσο με {MaxLength} χαρακτήρες.",
			"InclusiveBetween_Simple" => "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεταξύ {From} και {To}.",
			_ => null,
		};
	}
}

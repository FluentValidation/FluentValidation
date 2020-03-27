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

namespace FluentValidation.Resources {
	using FluentValidation.Validators;

	internal class GreekLanguage : Language {
		public const string Culture = "el";
		public override string Name => Culture;

		public GreekLanguage() {
			Translate<EmailValidator>("Το πεδίο '{PropertyName}' δεν περιέχει μια έγκυρη διεύθυνση email.");
			Translate<GreaterThanOrEqualValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη ή ίση με '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη από '{ComparisonValue}'.");
			Translate<LengthValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει μήκος μεταξύ {MinLength} και {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
			Translate<MinimumLengthValidator>("Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι τουλάχιστον {MinLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
			Translate<MaximumLengthValidator>("Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι το πολύ {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
			Translate<LessThanOrEqualValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μικρότερη ή ίση με '{ComparisonValue}'.");
			Translate<LessThanValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μικρότερη από '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Το πεδίο '{PropertyName}' δεν πρέπει να είναι κενό.");
			Translate<NotEqualValidator>("Το πεδίο '{PropertyName}' δεν πρέπει να έχει τιμή ίση με '{ComparisonValue}'.");
			Translate<NotNullValidator>("Το πεδίο '{PropertyName}' δεν πρέπει να είναι κενό.");
			Translate<PredicateValidator>("Η ορισμένη συνθήκη δεν ικανοποιήθηκε για το πεδίο '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Η ορισμένη συνθήκη δεν ικανοποιήθηκε για το πεδίο '{PropertyName}'.");
			Translate<RegularExpressionValidator>("Η τιμή του πεδίου '{PropertyName}' δεν έχει αποδεκτή μορφή.");
			Translate<EqualValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή ίση με '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει μήκος ίσο με {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
			Translate<InclusiveBetweenValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεταξύ {From} και {To}. Καταχωρίσατε την τιμή {Value}.");
			Translate<ExclusiveBetweenValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη από {From} και μικρότερη από {To}. Καταχωρίσατε την τιμή  {Value}.");
			Translate<CreditCardValidator>("Το πεδίο '{PropertyName}' δεν περιέχει αποδεκτό αριθμό πιστωτικής κάρτας.");
			Translate<ScalePrecisionValidator>("'Το πεδίο '{PropertyName}' δεν μπορεί να έχει περισσότερα από {ExpectedPrecision} ψηφία στο σύνολο, με μέγιστο επιτρεπόμενο αριθμό δεκαδικών τα {ExpectedScale} ψηφία. Έχετε καταχωρίσει {Digits} ψηφία συνολικά με {ActualScale} δεκαδικά.");
			Translate<EmptyValidator>("Το πεδίο '{PropertyName}' πρέπει να είναι κενό.");
			Translate<NullValidator>("Το πεδίο '{PropertyName}' πρέπει να είναι κενό.");
			Translate<EnumValidator>("Το πεδίο '{PropertyName}' επιτρέπει συγκεκριμένο εύρος τιμών που δεν περιλαμβάνουν την τιμή '{PropertyValue}' που καταχωρίσατε.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "Το πεδίο '{PropertyName}' πρέπει να έχει μήκος μεταξύ {MinLength} και {MaxLength} χαρακτήρες.");
			Translate("MinimumLength_Simple", "Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι τουλάχιστον {MinLength} χαρακτήρες.");
			Translate("MaximumLength_Simple", "Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι το πολύ {MaxLength} χαρακτήρες.");
			Translate("ExactLength_Simple", "Το πεδίο '{PropertyName}' πρέπει να έχει μήκος ίσο με {MaxLength} χαρακτήρες.");
			Translate("InclusiveBetween_Simple", "Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεταξύ {From} και {To}.");
		}
	}
}

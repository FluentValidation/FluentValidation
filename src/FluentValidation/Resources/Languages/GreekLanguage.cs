namespace FluentValidation.Resources
{
    using FluentValidation.Validators;

    internal class GreekLanguage : Language
    {
        public override string Name => "el";

        public GreekLanguage()
        {
            Translate<EmailValidator>("Το πεδίο '{PropertyName}' δεν περιέχει μια έγκυρη διεύθυνση email.");
            Translate<GreaterThanOrEqualValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη ή ίση με '{ComparisonValue}'.");
            Translate<GreaterThanValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μεγαλύτερη από '{ComparisonValue}'.");
            Translate<LengthValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει μήκος μεταξύ {MinLength} και {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
            Translate<MinimumLengthValidator>("Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι τουλάχιστον {MinLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
            Translate<MaximumLengthValidator>("Το μήκος του πεδίου '{PropertyName}' πρέπει να είναι το πολύ {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
            Translate<LessThanOrEqualValidator>("Το πεδίο '{PropertyName}' πρέπει να έχει τιμή μικρότερη ή ίση με '{ComparisonValue}'.");
            Translate<LessThanValidator>("'Το πεδίο {PropertyName}' πρέπει να έχει τιμή μικρότερη από '{ComparisonValue}'.");
            Translate<NotEmptyValidator>("'Το πεδίο {PropertyName}' δεν πρέπει να είναι κενό.");
            Translate<NotEqualValidator>("'Το πεδίο {PropertyName}' δεν πρέπει να έχει τιμή ίση με '{ComparisonValue}'.");
            Translate<NotNullValidator>("'Το πεδίο {PropertyName}' πρέπει να είναι κενό.");
            Translate<PredicateValidator>("Η ορισμένη συνθήκη δεν είναι ικανοποιήθηκε για το πεδίο '{PropertyName}'.");
            Translate<AsyncPredicateValidator>("Η ορισμένη συνθήκη δεν είναι ικανοποιήθηκε για το πεδίο '{PropertyName}'.");
            Translate<RegularExpressionValidator>("Η τιμή του πεδίο '{PropertyName}' δεν έχει σωστή μορφοποίηση.");
            Translate<EqualValidator>("'Το πεδίο {PropertyName}' πρέπει να έχει τιμή ίση με '{ComparisonValue}'.");
            Translate<ExactLengthValidator>("'Το πεδίο {PropertyName}' πρέπει να έχει μήκος ίσο με {MaxLength} χαρακτήρες. Έχετε καταχωρίσει {TotalLength} χαρακτήρες.");
            Translate<InclusiveBetweenValidator>("'Το πεδίο {PropertyName}' πρέπει να έχει τιμή μεταξύ {From} και {To}. Καταχωρίσατε την τιμή {Value}.");
            Translate<ExclusiveBetweenValidator>("'Το πεδίο {PropertyName}' πρέπει να έχει τιμή μεγαλύτερη από {From} και μικρότερη από {To}. Καταχωρίσατε την τιμή  {Value}.");
            Translate<CreditCardValidator>("'Το πεδίο {PropertyName}' δεν περιέχει αποδεκτό αριθμό πιστωτικής κάρτας.");
            Translate<ScalePrecisionValidator>("'Το πεδίο {PropertyName}' δεν μπορεί να έχει περισσότερα από {expectedPrecision} ψηφία στο σύνολο, με μέγιστο επιτρεπόμενο αριθμό δεκαδικών τα {expectedScale} ψηφία. Έχετε καταχωρίσει {digits} ψηφία συνολικά με {actualScale} δεκαδικά.");
            Translate<EmptyValidator>("'Το πεδίο {PropertyName}' πρέπει να είναι κενό.");
            Translate<NullValidator>("'Το πεδίο {PropertyName}'  πρέπει να είναι κενό.");
            Translate<EnumValidator>("'Το πεδίο {PropertyName}' επιτρπέπει συγκεκριμένο εύρος τιμών που δεν περιλαμβάνουν την τιμή '{PropertyValue}' που καταχωρίσατε.");
        }
    }
}

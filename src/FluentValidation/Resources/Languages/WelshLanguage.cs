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
  using Validators;

  internal class WelshLanguage : Language {
	  public const string Culture = "cy";
    public override string Name => Culture;

    public WelshLanguage() {
      Translate<EmailValidator>("Nid yw '{PropertyName}' yn gyfeiriad e-bost dilys.");
      Translate<GreaterThanOrEqualValidator>("Rhaid i '{PropertyName}' fod yn fwy na '{ComparisonValue}', neu'n gyfartal ag o.");
      Translate<GreaterThanValidator>("Rhaid i '{PropertyName}' fod yn fwy na '{ComparisonValue}'.");
      Translate<LengthValidator>("Rhaid i '{PropertyName}' fod rhwng {MinLength} a {MaxLength} o nodau. Rydych wedi rhoi {TotalLength} nod.");
      Translate<MinimumLengthValidator>("Rhaid i '{PropertyName}' fod o leiaf {MinLength} nod o hyd. Rydych wedi rhoi {TotalLength} nod.");
      Translate<MaximumLengthValidator>("Rhaid i '{PropertyName}' fod yn {MaxLength} nod o hyd neu lai. Rydych wedi rhoi {TotalLength} nod.");
      Translate<LessThanOrEqualValidator>("Rhaid i '{PropertyName}' fod yn llai na '{ComparisonValue}', neu'n gyfartal ag o.");
      Translate<LessThanValidator>("Rhaid i '{PropertyName}' fod yn llai na '{ComparisonValue}'.");
      Translate<NotEmptyValidator>("Ni ddylai '{PropertyName}' fod yn wag.");
      Translate<NotEqualValidator>("Ni ddylai '{PropertyName}' fod yn gyfartal â '{ComparisonValue}'.");
      Translate<NotNullValidator>("Ni ddylai '{PropertyName}' fod yn wag.");
      Translate<PredicateValidator>("Ni chyflawnwyd y gofyniad penodol ar gyfer '{PropertyName}'.");
      Translate<AsyncPredicateValidator>("Ni chyflawnwyd y gofyniad penodol ar gyfer '{PropertyName}'.");
      Translate<RegularExpressionValidator>("Nid yw '{PropertyName}' yn y fformat cywir.");
      Translate<EqualValidator>("Mae'n rhaid i '{PropertyName}' fod yn gyfartal â '{ComparisonValue}'.");
      Translate<ExactLengthValidator>("Mae'n rhaid i '{PropertyName}' fod yn {MaxLength} nod o hyd. Rydych wedi rhoi {TotalLength} nod.");
      Translate<InclusiveBetweenValidator>("Rhaid i '{PropertyName}' fod rhwng {From} a {To}. Rydych wedi rhoi {Value}.");
      Translate<ExclusiveBetweenValidator>("Rhaid i '{PropertyName}' fod rhwng {From} a {To} (ddim yn gynwysedig). Rydych wedi rhoi {Value}.");
      Translate<CreditCardValidator>("Nid yw '{PropertyName}' yn rhif cerdyn credyd dilys.");
      Translate<ScalePrecisionValidator>("Ni ddylai '{PropertyName}' fod yn fwy na {ExpectedPrecision} digid i gyd  gan ganiatáu ar gyfer {ExpectedScale} degolyn. Canfuwyd {Digits} digid a {ActualScale} degolyn.");
      Translate<EmptyValidator>("Rhaid i '{PropertyName}' fod yn wag.");
      Translate<NullValidator>("Rhaid i '{PropertyName}' fod yn wag.");
      Translate<EnumValidator>("Mae gan '{PropertyName}' ystod o werthoedd nad ydynt yn cynnwys '{PropertyValue}'.");
      // Additional fallback messages used by clientside validation integration.
      Translate("Length_Simple", "Rhaid i '{PropertyName}' fod rhwng {MinLength} a {MaxLength} o nodau.");
      Translate("MinimumLength_Simple", "Rhaid i '{PropertyName}' fod o leiaf {MinLength} nod o hyd.");
      Translate("MaximumLength_Simple", "Rhaid i '{PropertyName}' fod yn {MaxLength} nod o hyd neu lai.");
      Translate("ExactLength_Simple", "Mae'n rhaid i '{PropertyName}' fod yn {MaxLength} nod o hyd.");
      Translate("InclusiveBetween_Simple", "Rhaid i '{PropertyName}' fod rhwng {From} a {To}.");
    }
  }
}

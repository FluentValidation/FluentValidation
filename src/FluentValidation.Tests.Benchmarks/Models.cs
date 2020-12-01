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
	using System.ComponentModel;

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

		public string Country { get; set; }

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

			RuleFor(x => x.Country).IsEnumName(typeof(FullModel));

			RuleFor(x => x.Country).IsEnumName<FullModel, Countries>();

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

	//taken from https://gist.github.com/jplwood/4f77b55cfedf2820dce0dfcd3ee0c3ea#file-iso-3166-1-alpha-2-country-codes-enum-cs
	public enum Countries {
		[Description("Afghanistan")] AF = 1,
		[Description("Åland Islands")] AX = 2,
		[Description("Albania")] AL = 3,
		[Description("Algeria")] DZ = 4,
		[Description("American Samoa")] AS = 5,
		[Description("Andorra")] AD = 6,
		[Description("Angola")] AO = 7,
		[Description("Anguilla")] AI = 8,
		[Description("Antarctica")] AQ = 9,
		[Description("Antigua and Barbuda")] AG = 10,
		[Description("Argentina")] AR = 11,
		[Description("Armenia")] AM = 12,
		[Description("Aruba")] AW = 13,
		[Description("Australia")] AU = 14,
		[Description("Austria")] AT = 15,
		[Description("Azerbaijan")] AZ = 16,
		[Description("Bahamas")] BS = 17,
		[Description("Bahrain")] BH = 18,
		[Description("Bangladesh")] BD = 19,
		[Description("Barbados")] BB = 20,
		[Description("Belarus")] BY = 21,
		[Description("Belgium")] BE = 22,
		[Description("Belize")] BZ = 23,
		[Description("Benin")] BJ = 24,
		[Description("Bermuda")] BM = 25,
		[Description("Bhutan")] BT = 26,
		[Description("Bolivia (Plurinational State of)")] BO = 27,
		[Description("Bonaire, Sint Eustatius and Saba")] BQ = 28,
		[Description("Bosnia and Herzegovina")] BA = 29,
		[Description("Botswana")] BW = 30,
		[Description("Bouvet Island")] BV = 31,
		[Description("Brazil")] BR = 32,
		[Description("British Indian Ocean Territory")] IO = 33,
		[Description("Brunei Darussalam")] BN = 34,
		[Description("Bulgaria")] BG = 35,
		[Description("Burkina Faso")] BF = 36,
		[Description("Burundi")] BI = 37,
		[Description("Cabo Verde")] CV = 38,
		[Description("Cambodia")] KH = 39,
		[Description("Cameroon")] CM = 40,
		[Description("Canada")] CA = 41,
		[Description("Cayman Islands")] KY = 42,
		[Description("Central African Republic")] CF = 43,
		[Description("Chad")] TD = 44,
		[Description("Chile")] CL = 45,
		[Description("China")] CN = 46,
		[Description("Christmas Island")] CX = 47,
		[Description("Cocos (Keeling) Islands")] CC = 48,
		[Description("Colombia")] CO = 49,
		[Description("Comoros")] KM = 50,
		[Description("Congo")] CG = 51,
		[Description("Congo (Democratic Republic of the)")] CD = 52,
		[Description("Cook Islands")] CK = 53,
		[Description("Costa Rica")] CR = 54,
		[Description("Côte d'Ivoire")] CI = 55,
		[Description("Croatia")] HR = 56,
		[Description("Cuba")] CU = 57,
		[Description("Curaçao")] CW = 58,
		[Description("Cyprus")] CY = 59,
		[Description("Czechia")] CZ = 60,
		[Description("Denmark")] DK = 61,
		[Description("Djibouti")] DJ = 62,
		[Description("Dominica")] DM = 63,
		[Description("Dominican Republic")] DO = 64,
		[Description("Ecuador")] EC = 65,
		[Description("Egypt")] EG = 66,
		[Description("El Salvador")] SV = 67,
		[Description("Equatorial Guinea")] GQ = 68,
		[Description("Eritrea")] ER = 69,
		[Description("Estonia")] EE = 70,
		[Description("Ethiopia")] ET = 71,
		[Description("Falkland Islands (Malvinas)")] FK = 72,
		[Description("Faroe Islands")] FO = 73,
		[Description("Fiji")] FJ = 74,
		[Description("Finland")] FI = 75,
		[Description("France")] FR = 76,
		[Description("French Guiana")] GF = 77,
		[Description("French Polynesia")] PF = 78,
		[Description("French Southern Territories")] TF = 79,
		[Description("Gabon")] GA = 80,
		[Description("Gambia")] GM = 81,
		[Description("Georgia")] GE = 82,
		[Description("Germany")] DE = 83,
		[Description("Ghana")] GH = 84,
		[Description("Gibraltar")] GI = 85,
		[Description("Greece")] GR = 86,
		[Description("Greenland")] GL = 87,
		[Description("Grenada")] GD = 88,
		[Description("Guadeloupe")] GP = 89,
		[Description("Guam")] GU = 90,
		[Description("Guatemala")] GT = 91,
		[Description("Guernsey")] GG = 92,
		[Description("Guinea")] GN = 93,
		[Description("Guinea-Bissau")] GW = 94,
		[Description("Guyana")] GY = 95,
		[Description("Haiti")] HT = 96,
		[Description("Heard Island and McDonald Islands")] HM = 97,
		[Description("Holy See")] VA = 98,
		[Description("Honduras")] HN = 99,
		[Description("Hong Kong")] HK = 100,
		[Description("Hungary")] HU = 101,
		[Description("Iceland")] IS = 102,
		[Description("India")] IN = 103,
		[Description("Indonesia")] ID = 104,
		[Description("Iran (Islamic Republic of)")] IR = 105,
		[Description("Iraq")] IQ = 106,
		[Description("Ireland")] IE = 107,
		[Description("Isle of Man")] IM = 108,
		[Description("Israel")] IL = 109,
		[Description("Italy")] IT = 110,
		[Description("Jamaica")] JM = 111,
		[Description("Japan")] JP = 112,
		[Description("Jersey")] JE = 113,
		[Description("Jordan")] JO = 114,
		[Description("Kazakhstan")] KZ = 115,
		[Description("Kenya")] KE = 116,
		[Description("Kiribati")] KI = 117,
		[Description("Korea (Democratic People's Republic of)")] KP = 118,
		[Description("Korea (Republic of)")] KR = 119,
		[Description("Kuwait")] KW = 120,
		[Description("Kyrgyzstan")] KG = 121,
		[Description("Lao People's Democratic Republic")] LA = 122,
		[Description("Latvia")] LV = 123,
		[Description("Lebanon")] LB = 124,
		[Description("Lesotho")] LS = 125,
		[Description("Liberia")] LR = 126,
		[Description("Libya")] LY = 127,
		[Description("Liechtenstein")] LI = 128,
		[Description("Lithuania")] LT = 129,
		[Description("Luxembourg")] LU = 130,
		[Description("Macao")] MO = 131,
		[Description("Macedonia (the former Yugoslav Republic of)")] MK = 132,
		[Description("Madagascar")] MG = 133,
		[Description("Malawi")] MW = 134,
		[Description("Malaysia")] MY = 135,
		[Description("Maldives")] MV = 136,
		[Description("Mali")] ML = 137,
		[Description("Malta")] MT = 138,
		[Description("Marshall Islands")] MH = 139,
		[Description("Martinique")] MQ = 140,
		[Description("Mauritania")] MR = 141,
		[Description("Mauritius")] MU = 142,
		[Description("Mayotte")] YT = 143,
		[Description("Mexico")] MX = 144,
		[Description("Micronesia (Federated States of)")] FM = 145,
		[Description("Moldova (Republic of)")] MD = 146,
		[Description("Monaco")] MC = 147,
		[Description("Mongolia")] MN = 148,
		[Description("Montenegro")] ME = 149,
		[Description("Montserrat")] MS = 150,
		[Description("Morocco")] MA = 151,
		[Description("Mozambique")] MZ = 152,
		[Description("Myanmar")] MM = 153,
		[Description("Namibia")] NA = 154,
		[Description("Nauru")] NR = 155,
		[Description("Nepal")] NP = 156,
		[Description("Netherlands")] NL = 157,
		[Description("New Caledonia")] NC = 158,
		[Description("New Zealand")] NZ = 159,
		[Description("Nicaragua")] NI = 160,
		[Description("Niger")] NE = 161,
		[Description("Nigeria")] NG = 162,
		[Description("Niue")] NU = 163,
		[Description("Norfolk Island")] NF = 164,
		[Description("Northern Mariana Islands")] MP = 165,
		[Description("Norway")] NO = 166,
		[Description("Oman")] OM = 167,
		[Description("Pakistan")] PK = 168,
		[Description("Palau")] PW = 169,
		[Description("Palestine, State of")] PS = 170,
		[Description("Panama")] PA = 171,
		[Description("Papua New Guinea")] PG = 172,
		[Description("Paraguay")] PY = 173,
		[Description("Peru")] PE = 174,
		[Description("Philippines")] PH = 175,
		[Description("Pitcairn")] PN = 176,
		[Description("Poland")] PL = 177,
		[Description("Portugal")] PT = 178,
		[Description("Puerto Rico")] PR = 179,
		[Description("Qatar")] QA = 180,
		[Description("Réunion")] RE = 181,
		[Description("Romania")] RO = 182,
		[Description("Russian Federation")] RU = 183,
		[Description("Rwanda")] RW = 184,
		[Description("Saint Barthélemy")] BL = 185,
		[Description("Saint Helena, Ascension and Tristan da Cunha")] SH = 186,
		[Description("Saint Kitts and Nevis")] KN = 187,
		[Description("Saint Lucia")] LC = 188,
		[Description("Saint Martin (French part)")] MF = 189,
		[Description("Saint Pierre and Miquelon")] PM = 190,
		[Description("Saint Vincent and the Grenadines")] VC = 191,
		[Description("Samoa")] WS = 192,
		[Description("San Marino")] SM = 193,
		[Description("Sao Tome and Principe")] ST = 194,
		[Description("Saudi Arabia")] SA = 195,
		[Description("Senegal")] SN = 196,
		[Description("Serbia")] RS = 197,
		[Description("Seychelles")] SC = 198,
		[Description("Sierra Leone")] SL = 199,
		[Description("Singapore")] SG = 200,
		[Description("Sint Maarten (Dutch part)")] SX = 201,
		[Description("Slovakia")] SK = 202,
		[Description("Slovenia")] SI = 203,
		[Description("Solomon Islands")] SB = 204,
		[Description("Somalia")] SO = 205,
		[Description("South Africa")] ZA = 206,
		[Description("South Georgia and the South Sandwich Islands")] GS = 207,
		[Description("South Sudan")] SS = 208,
		[Description("Spain")] ES = 209,
		[Description("Sri Lanka")] LK = 210,
		[Description("Sudan")] SD = 211,
		[Description("Suriname")] SR = 212,
		[Description("Svalbard and Jan Mayen")] SJ = 213,
		[Description("Swaziland")] SZ = 214,
		[Description("Sweden")] SE = 215,
		[Description("Switzerland")] CH = 216,
		[Description("Syrian Arab Republic")] SY = 217,
		[Description("Taiwan, Province of China[a]")] TW = 218,
		[Description("Tajikistan")] TJ = 219,
		[Description("Tanzania, United Republic of")] TZ = 220,
		[Description("Thailand")] TH = 221,
		[Description("Timor-Leste")] TL = 222,
		[Description("Togo")] TG = 223,
		[Description("Tokelau")] TK = 224,
		[Description("Tonga")] TO = 225,
		[Description("Trinidad and Tobago")] TT = 226,
		[Description("Tunisia")] TN = 227,
		[Description("Turkey")] TR = 228,
		[Description("Turkmenistan")] TM = 229,
		[Description("Turks and Caicos Islands")] TC = 230,
		[Description("Tuvalu")] TV = 231,
		[Description("Uganda")] UG = 232,
		[Description("Ukraine")] UA = 233,
		[Description("United Arab Emirates")] AE = 234,
		[Description("United Kingdom of Great Britain and Northern Ireland")] GB = 235,
		[Description("United States of America")] US = 236,
		[Description("United States Minor Outlying Islands")] UM = 237,
		[Description("Uruguay")] UY = 238,
		[Description("Uzbekistan")] UZ = 239,
		[Description("Vanuatu")] VU = 240,
		[Description("Venezuela (Bolivarian Republic of)")] VE = 241,
		[Description("Viet Nam")] VN = 242,
		[Description("Virgin Islands (British)")] VG = 243,
		[Description("Virgin Islands (U.S.)")] VI = 244,
		[Description("Wallis and Futuna")] WF = 245,
		[Description("Western Sahara")] EH = 246,
		[Description("Yemen")] YE = 247,
		[Description("Zambia")] ZM = 248,
		[Description("Zimbabwe")] ZW = 249,
	}
}

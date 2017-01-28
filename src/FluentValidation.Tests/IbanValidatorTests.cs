namespace FluentValidation.Tests
{
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Xunit;


    public class IbanValidatorTests
    {
        TestValidator validator;

        public IbanValidatorTests()
        {
            CultureScope.SetDefaultCulture();

            validator = new TestValidator {
                v => v.RuleFor(x => x.IbanAccount).IsIBAN()
            };
        }

        [Fact]
        public void IsValidTests()
        {
            // Empty values
            validator.Validate(new Person { IbanAccount = null }).IsValid.ShouldBeTrue(); // Optional values are always valid
            validator.Validate(new Person { IbanAccount = string.Empty }).IsValid.ShouldBeTrue(); // empty string
            validator.Validate(new Person { IbanAccount = "\n\r" }).IsValid.ShouldBeTrue(); // new line

            // Valid sample IBAN account numbers
            validator.Validate(new Person { IbanAccount = "AL90208110080000001039531801" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "AT611904300234573201" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "DZ4000400174401001050486" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "AD1200012030200359100100" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "AO06000600000100037131174" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "AT611904300234573201" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "AZ21NABZ00000000137010001944" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BH29BMAG1299123456BH00" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BA391290079401028494" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BE68539007547034" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BJ11B00610100400271101192591" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BR9700360305000010009795493P1" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BG80BNBG96611020345678" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BF1030134020015400945000643" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BI43201011067444" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CM2110003001000500000605306" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CV64000300004547069110176" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CR0515202001026284066" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CZ6508000000192000145399" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "DK5000400440116243" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "NL91ABNA0417164300" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "NO9386011117947" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "PL27114020040000300201355387" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "PT50000201231234567890154" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "SA0380000000608010167519" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CS73260005601001611379" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "TL380080012345678910157" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "SI56191000000123438" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "ES8023100001180000012345" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "SE3550000000054910000003" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CH3900700115201849173" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "TR330006100519786457841326" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "IL620108000000099999999" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "PK36SCBL0000001123456702" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "BE68844010370034" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CZ4201000000195505030267" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "DK5750510001322617" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "EE342200221034126658" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "FI9814283500171141" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "FR7630066100410001057380116" }).IsValid.ShouldBeTrue();
            validator.Validate(new Person { IbanAccount = "CH3908704016075473007" }).IsValid.ShouldBeTrue();

            // Corrupted accounts           
            validator.Validate(new Person { IbanAccount = "CH" }).IsValid.ShouldBeFalse();
            validator.Validate(new Person { IbanAccount = "CH39" }).IsValid.ShouldBeFalse();
            validator.Validate(new Person { IbanAccount = " DK5000400440116243" }).IsValid.ShouldBeFalse(); // space
            validator.Validate(new Person { IbanAccount = "EE382200221020145685!" }).IsValid.ShouldBeFalse(); // not alpha-numereic          
            validator.Validate(new Person { IbanAccount = "LU$2800194006444750000" }).IsValid.ShouldBeFalse(); // not alpha-numereic
            validator.Validate(new Person { IbanAccount = "MK07%300000000042425" }).IsValid.ShouldBeFalse(); // not alpha-numereic     
            validator.Validate(new Person { IbanAccount = "EE3422D555555555555555555555555555555555500221034126658" }).IsValid.ShouldBeFalse(); // too long
            validator.Validate(new Person { IbanAccount = "FI981428350017114555555556666666666666666666666666666666666666666666666666666666666661" }).IsValid.ShouldBeFalse(); // too long            
            validator.Validate(new Person { IbanAccount = "CZ4201000000195505030244" }).IsValid.ShouldBeFalse(); // bad check sum
        }

        [Fact]
        public void When_validation_fails_the_default_error_should_be_set()
        {
            string iban = "foo";
            var result = validator.Validate(new Person { IbanAccount = iban });
            result.Errors.Single().ErrorMessage.ShouldEqual("'Iban Account' is not a valid IBAN account number.");
        }

    }
}
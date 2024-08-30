using FluentAssertions;
using Tests.Src.UnitTests.Example2;

namespace Tests.UnitTests.Example2
{
    public class EmailValidatorTests
    {

        [Theory]
        [InlineData("thisisnabi@outlook.com", true)]
        [InlineData("b@.com", false)]
        [InlineData(".@.com", false)]
        [InlineData("b@.z", false)]
        [InlineData("b@sum#.com", true)]
        [InlineData("", false)]
        [InlineData(null, false)]

        public void IsEmailValid_ShouldReturnExpectResult(string email, bool expectedResult)
        {
            var sut = new EmailValidator();
        
            var result = sut.IsValidEmail(email);

            result.Should().Be(expectedResult);
        
        }

    }
}

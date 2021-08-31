using LibreCards.Core.Entities;
using Xunit;

namespace LibreCards.Tests
{
    public class TemplateTests
    {
        [Fact]
        public void Constructor_EmptyString_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new Template(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Constructor_EmptyOrWhitespaceString_ShouldThrow(string input)
        {
            Assert.Throws<ArgumentException>(() => new Template(input));
        }

        [Theory]
        [InlineData("Some text without a blank.")]
        [InlineData("?")]
        [InlineData("_")]
        [InlineData("<BLANK")]
        [InlineData("BLANK")]
        [InlineData("BLANK>")]
        [InlineData("<blank>")]
        public void Constructor_NoValidBlankContent_ShouldThrow(string input)
        {
            Assert.Throws<ArgumentException>(() => new Template(input));
        }

        [Theory]
        [InlineData("Single <BLANK>", 1)]
        [InlineData("<BLANK>", 1)]
        [InlineData("<BLANK><BLANK>", 2)]
        [InlineData("A card with <BLANK> 2 <BLANK> blanks", 2)]
        [InlineData("<BLANK> foo <BLANK> bar <BLANK>", 3)]
        public void CorrectInput_ShouldReturnCorrectBlankCount(string input, int expected)
        {
            var template = new Template(input);

            Assert.Equal(expected, template.BlankCount);
        }

        [Theory]
        [InlineData("Single <BLANK>")]
        [InlineData("<BLANK>")]
        [InlineData("<BLANK><BLANK>")]
        [InlineData("A card with <BLANK> 2 <BLANK> blanks")]
        [InlineData("<BLANK> foo <BLANK> bar <BLANK>")]
        public void CorrectInput_ShouldReturnTheSameTemplate(string expected)
        {
            var template = new Template(expected);

            Assert.Equal(expected, template.Content);
        }
    }
}

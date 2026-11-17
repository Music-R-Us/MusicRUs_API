using System.ComponentModel.DataAnnotations;
using MusicRUs.API.DTOs;
using Xunit;

namespace MusicRUs.API.Tests.Security;

// Runs the same [Required] and [RegularExpression] checks that [ApiController] runs before a controller action
public class DtoValidationTests
{
    private static bool IsValid(object dto)
        => Validator.TryValidateObject(dto, new ValidationContext(dto), new List<ValidationResult>(), validateAllProperties: true);

    [Theory]
    [InlineData("Correct1!", true)]
    [InlineData("password1!", false)]         // No uppercase letter
    [InlineData("PASSWORD1!", false)]         // No lowercase letter
    [InlineData("Password!!", false)]         // No number
    [InlineData("Password12", false)]         // No special character
    [InlineData("Pa1!", false)]               // Too short
    [InlineData("Password1!<script>", false)] // Characters outside the allowed set
    public void Password_must_follow_the_password_rules(string password, bool expected)
    {
        var dto = new CreateUserDto { Username = "mark", Email = "mark@example.dk", Password = password };

        Assert.Equal(expected, IsValid(dto));
    }

    [Theory]
    [InlineData("https://musicrus.dk", true)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("data:text/html;base64,PHNjcmlwdD5hbGVydCgxKTwvc2NyaXB0Pg==", false)]
    [InlineData("https://example.com\" onmouseover=\"alert(1)", false)] // Tries to break out of the href attribute
    [InlineData("example.com", false)]
    public void Profile_urls_must_start_with_http_or_https(string url, bool expected)
    {
        Assert.Equal(expected, IsValid(new ForumProfileUpdatedDto { WebsiteUrl = url }));
        Assert.Equal(expected, IsValid(new ForumProfileUpdatedDto { AvatarUrl = url }));
    }

    [Theory]
    [InlineData("123456", true)]
    [InlineData("", false)]
    [InlineData("12345", false)]
    [InlineData("1234567", false)]
    [InlineData("12a456", false)]
    public void TwoFactor_code_must_be_exactly_6_digits(string code, bool expected)
    {
        Assert.Equal(expected, IsValid(new TwoFactorCodeDto { Code = code }));
    }
}

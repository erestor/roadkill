using Roadkill.Core.Attachments;
using Roadkill.Tests.NETStandard.NUnitToXUnit;
using Xunit;

namespace Roadkill.Tests.Unit.Attachments
{
	[Category("Unit")]
	public class MimeMappingTests
	{
		[Fact]
		public void should_return_application_mimetype_for_empty_extension()
		{
			// Arrange
			string expected = "application/octet-stream";

			// Act
			string actual = MimeTypes.GetMimeType("");

			// Assert
			NUnitAssert.That(actual, Is.EqualTo(expected));
		}

		[Fact]
		public void should_return_application_mimetype_for_unknown_extension()
		{
			// Arrange
			string expected = "application/octet-stream";

			// Act
			string actual = MimeTypes.GetMimeType(".blah");

			// Assert
			NUnitAssert.That(actual, Is.EqualTo(expected));
		}

		[Fact]
		public void should_ignore_case_for_extension()
		{
			// Arrange
			string expected = "image/jpeg";

			// Act
			string actual = MimeTypes.GetMimeType(".JPEG");

			// Assert
			NUnitAssert.That(actual, Is.EqualTo(expected));
		}

		[Theory]
		[InlineData("image/jpeg", ".jpg")]
		[InlineData("image/png", ".png")]
		[InlineData("image/gif", ".gif")]
		[InlineData("application/x-shockwave-flash", ".swf")]
		[InlineData("application/pdf", ".pdf")]
		public void Should_Return_Known_Types_Common_Extension(string expectedMimeType, string extension)
		{
			// Arrange

			// Act
			string actual = MimeTypes.GetMimeType(extension);

			// Assert
			NUnitNUnitAssert.That(actual, Is.EqualTo(expectedMimeType));
		}
	}
}
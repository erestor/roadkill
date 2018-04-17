using System;
using System.IO;
using Roadkill.Core.Attachments;
using Roadkill.Core.Configuration;
using Roadkill.Core.Services;
using Roadkill.Tests.NETStandard.NUnitToXUnit;
using Roadkill.Tests.NETStandard.Unit.StubsAndMocks;
using Roadkill.Tests.NETStandard.Unit.StubsAndMocks.Database;
using Xunit;

namespace Roadkill.Tests.NETStandard.Unit.Attachments
{
	[Category("Unit")]
	public class AttachmentFileHandlerTests
	{
		private readonly ApplicationSettings _applicationSettings;
		private readonly IFileService _fileService;

		public AttachmentFileHandlerTests()
		{
			_applicationSettings = new ApplicationSettings();
			_applicationSettings.AttachmentsFolder = Path.Combine(Helpers.GetCurrentDirectory(), "Unit", "Attachments");
			_applicationSettings.AttachmentsRoutePath = "Attachments";

			_fileService = new LocalFileService(_applicationSettings, new SettingsService(new RepositoryFactoryMock(), _applicationSettings));
		}

		[Fact]
		public void writeresponse_should_set_200_status_and_mimetype_and_write_bytes()
		{
			// Arrange
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings, _fileService);

			string fullPath = Path.Combine(Helpers.GetCurrentDirectory(), "Unit", "Attachments", "afile.jpg");
			File.WriteAllText(fullPath, "fake content");
			byte[] expectedBytes = File.ReadAllBytes(fullPath);
			string expectedMimeType = "image/jpeg";

			string localPath = "/wiki/Attachments/afile.jpg";
			string applicationPath = "/wiki";
			string modifiedSince = "";

			ResponseWrapperMock wrapper = new ResponseWrapperMock();

			// Act
			handler.WriteResponse(localPath, applicationPath, modifiedSince, wrapper, null);

			// Assert
			NUnitAssert.That(wrapper.StatusCode, Is.EqualTo(200));
			NUnitAssert.That(wrapper.ContentType, Is.EqualTo(expectedMimeType));
			NUnitAssert.That(wrapper.Buffer, Is.EqualTo(expectedBytes));
		}

		[Fact]
		public void writeresponse_should_throw_404_exception_for_missing_file()
		{
			// Arrange
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings, _fileService);

			string localPath = "/wiki/Attachments/doesntexist404.jpg";
			string applicationPath = "/wiki";
			string modifiedSince = "";

			ResponseWrapperMock wrapper = new ResponseWrapperMock();

			try
			{
				// Act + Assert
				handler.WriteResponse(localPath, applicationPath, modifiedSince, wrapper, null);

				NUnitAssert.Fail("No 404 HttpException thrown");
			}
			catch (Exception e)
			{
				NUnitAssert.Fail("TODO: NETStandard - this no longer throws an exception");
			}
		}

		[Fact]
		public void writeresponse_should_throw_404_exception_for_bad_application_path()
		{
			// Arrange
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings, _fileService);

			string fullPath = Path.Combine(Helpers.GetCurrentDirectory(), "Unit", "Attachments", "afile.jpg");
			File.WriteAllText(fullPath, "fake content");

			string localPath = "/wiki/Attachments/afile.jpg";
			string applicationPath = "/wookie";
			string modifiedSince = "";

			ResponseWrapperMock wrapper = new ResponseWrapperMock();

			try
			{
				// Act + Assert
				handler.WriteResponse(localPath, applicationPath, modifiedSince, wrapper, null);

				NUnitAssert.Fail("No 500 HttpException thrown");
			}
			catch (Exception e)
			{
				NUnitAssert.Fail("TODO: NETStandard - this no longer throws an exception");
			}
		}

		[Fact]
		public void translatelocalpathtofilepath_should_be_case_sensitive()
		{
			// Arrange
			_applicationSettings.AttachmentsFolder = @"C:\attachments\";
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings, _fileService);

			// Act
			string actualPath = handler.TranslateUrlPathToFilePath("/Attachments/a.jpg", "/");

			// Assert
			NUnitAssert.That(actualPath, Is.Not.EqualTo(@"c:\Attachments\a.jpg"), "TranslateLocalPathToFilePath does a case sensitive url" +
																			 " replacement (this is for Apache compatibility");
		}

		[Theory]
		[InlineData("/Attachments/a.jpg", "", @"C:\Attachments\a.jpg")] // should tolerate 'bad' application paths
		[InlineData("Attachments/a.jpg", "/", @"C:\Attachments\a.jpg")] // should tolerate url not beginning with /
		[InlineData("/Attachments/a.jpg", "/", @"C:\Attachments\a.jpg")]
		[InlineData("/Attachments/folder1/folder2/a.jpg", "/wiki/", @"C:\Attachments\folder1\folder2\a.jpg")]
		[InlineData("/wiki/Attachments/a.jpg", "/wiki/", @"C:\Attachments\a.jpg")]
		[InlineData("/wiki/Attachments/a.jpg", "/wiki", @"C:\Attachments\a.jpg")]
		[InlineData("/wiki/wiki2/Attachments/a.jpg", "/wiki/wiki2/", @"C:\Attachments\a.jpg")]
		public void TranslateLocalPathToFilePath(string localPath, string appPath, string expectedPath)
		{
			// Arrange
			_applicationSettings.AttachmentsFolder = @"C:\Attachments\";
			AttachmentFileHandler handler = new AttachmentFileHandler(_applicationSettings, _fileService);

			// Act
			string actualPath = handler.TranslateUrlPathToFilePath(localPath, appPath);

			// Assert
			NUnitAssert.That(actualPath, Is.EqualTo(expectedPath), "Failed with {0} {1} {2}", localPath, appPath, expectedPath);
		}
	}
}
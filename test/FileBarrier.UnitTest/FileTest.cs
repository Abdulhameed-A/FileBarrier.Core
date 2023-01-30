using FileBarrier.Core;
using FileBarrier.Core.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FileBarrier.UnitTest
{
    public class FileTest
    {
        private readonly ITestOutputHelper _output;
        public FileTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("test.zip", 
            "application/x-zip-compressed", 
            1L, 
            "txt,pdf,doc,docx,docm,xls,xlsx,csv,png,jpg,jpeg,mp4,m4a,m4p,m4b,m4r,m4v,m3u8,m3u,avi,wmv,webm,ogg,mov,wav,mp3,zip",
            "text/plain,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/vnd.ms-word.document.macroEnabled.12,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,text/csv,image/png,image/jpeg,image/jpeg,video/mp4,audio/m4a,audio/x-m4a,audio/mp3,audio/m4p,audio/m4b,audio/x-m4r,video/x-m4v,audio/x-mpegurl,audio/mpegurl,video/x-msvideo,video/x-ms-wmv,video/avi,video/webm,video/x-msvideo,audio/ogg,video/quicktime,audio/wav,audio/mpeg,application/x-zip-compressed",
            5L, 
            FileCheckLayers.AllLayers)]
        [InlineData("test.png", 
            "image/png", 
            1L, 
            "txt,pdf,doc,docx,docm,xls,xlsx,csv,png,jpg,jpeg,mp4,m4a,m4p,m4b,m4r,m4v,m3u8,m3u,avi,wmv,webm,ogg,mov,wav,mp3,zip",
            "text/plain,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/vnd.ms-word.document.macroEnabled.12,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,text/csv,image/png,image/jpeg,image/jpeg,video/mp4,audio/m4a,audio/x-m4a,audio/mp3,audio/m4p,audio/m4b,audio/x-m4r,video/x-m4v,audio/x-mpegurl,audio/mpegurl,video/x-msvideo,video/x-ms-wmv,video/avi,video/webm,video/x-msvideo,audio/ogg,video/quicktime,audio/wav,audio/mpeg,application/x-zip-compressed",
            5L, 
            FileCheckLayers.AllLayers)]
        public void UploadFileTest(string fileName, string fileContentType, long fileSize, string allowedExtensions, string allowedContentType, long? maxSize, FileCheckLayers layers)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(fileSize);
            fileMock.Setup(_ => _.ContentType).Returns(fileContentType);

            var file = fileMock.Object;

            //Act
            var isAllowed = file.IsFileAllowed(allowedExtensions, allowedContentType, maxSize, layers);

            //Assert
            Assert.True(isAllowed);
        }


        [Theory]
        [InlineData("test.zip", 
            "application/x-zip-compressed", 
            1L, 
            "txt,pdf,doc,docx,docm,xls,xlsx,csv,png,jpg,jpeg,mp4,m4a,m4p,m4b,m4r,m4v,m3u8,m3u,avi,wmv,webm,ogg,mov,wav,mp3,zip", 
            "text/plain,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/vnd.ms-word.document.macroEnabled.12,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,text/csv,image/png,image/jpeg,image/jpeg,video/mp4,audio/m4a,audio/x-m4a,audio/mp3,audio/m4p,audio/m4b,audio/x-m4r,video/x-m4v,audio/x-mpegurl,audio/mpegurl,video/x-msvideo,video/x-ms-wmv,video/avi,video/webm,video/x-msvideo,audio/ogg,video/quicktime,audio/wav,audio/mpeg", 
            5L, 
            FileCheckLayers.AllLayers)]
        [InlineData("test.png", 
            "image/png", 
            6L, 
            "txt,pdf,doc,docx,docm,xls,xlsx,csv,png,jpg,jpeg,mp4,m4a,m4p,m4b,m4r,m4v,m3u8,m3u,avi,wmv,webm,ogg,mov,wav,mp3,zip", 
            "text/plain,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/vnd.ms-word.document.macroEnabled.12,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,text/csv,image/png,image/jpeg,image/jpeg,video/mp4,audio/m4a,audio/x-m4a,audio/mp3,audio/m4p,audio/m4b,audio/x-m4r,video/x-m4v,audio/x-mpegurl,audio/mpegurl,video/x-msvideo,video/x-ms-wmv,video/avi,video/webm,video/x-msvideo,audio/ogg,video/quicktime,audio/wav,audio/mpeg,application/x-zip-compressed",
            5L, 
            FileCheckLayers.AllLayers)]
        public void UploadFileFaildTest(string fileName, string fileContentType, long fileSize, string allowedExtensions, string allowedContentType, long? maxSize, FileCheckLayers layers)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(fileSize);
            fileMock.Setup(_ => _.ContentType).Returns(fileContentType);

            var file = fileMock.Object;

            //Act
            var response = file.IsAllowed(allowedExtensions, allowedContentType, maxSize, layers);

            //Assert
            Assert.False(response.IsAllowed);

            _output.WriteLine(response.ErrorMessage);
        }
    }
}

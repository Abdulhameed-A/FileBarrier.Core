using FileBarrier.Core.Exceptions;
using FileBarrier.Core.Helpers;
using FileBarrier.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FileBarrier.Core
{
    /// <summary>
    /// A set of extensions that help you to do specific things to an IFormFile.
    /// </summary>
    public static class IFormFileExtensions
    {
        /// <summary>
        /// Checks if the file is allowed based on specific conditions.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="allowedExtensions">Comma separated, e.g: "txt,pdf,doc,docx". You need to pass FileCheckLayers.CheckExtensions as one of the layers.</param>
        /// <param name="allowedContentTypes">Comma separated, e.g: "text/plain,application/pdf,application/msword". You need to pass FileCheckLayers.CheckContentType as one of the layers.</param>
        /// <param name="maxSingleFileSize">Accepted file size in bytes. You need to pass FileCheckLayers.CheckFileSize as one of the layers.</param>
        /// <param name="layers">The check-points that you want the file to go through.</param>
        /// <returns>true if the file is passed; otherwise false.</returns>
        public static bool IsFileAllowed(this IFormFile file, string allowedExtensions, string allowedContentTypes, long? maxSingleFileSize, params FileCheckLayers[] layers)
        {

            try
            {
                //Build the layers dictionay (since we will check the layers more than once).
                Dictionary<FileCheckLayers, bool> foundLayers = SetupLayersDictionary(layers);

                //Check null values.
                CheckNulls(file, allowedExtensions, allowedContentTypes, maxSingleFileSize, foundLayers);

                var allowedContentTypesArray = new string[] { };

                if (!string.IsNullOrWhiteSpace(allowedContentTypes))
                    allowedContentTypesArray = allowedContentTypes.Trim().Split(",");

                bool foundAllLayers = foundLayers.TryGetValue(FileCheckLayers.AllLayers, out foundAllLayers);


                //FileCheckLayers.CheckExtensions
                bool foundCheckExtensions = foundLayers.TryGetValue(FileCheckLayers.CheckExtensions, out foundCheckExtensions);
                if (foundCheckExtensions || foundAllLayers)
                {
                    var fileExtensionsAttribute = new FileExtensionsAttribute() { Extensions = allowedExtensions };
                    if (!fileExtensionsAttribute.IsValid(file?.FileName))
                    {
                        throw new BusinessException(string.Format($"File extension {file?.FileName} could not be found in the {nameof(allowedExtensions)}."));
                    };
                }


                //Layer2 checker, check if the recieved content-type matches the given file name.
                bool foundCheckFileContentTypeToFileExtension = foundLayers.TryGetValue(FileCheckLayers.CheckFileContentTypeToFileExtension, out foundCheckFileContentTypeToFileExtension);
                if (foundCheckFileContentTypeToFileExtension || foundAllLayers)
                    MatchContentTypeToFileName(file?.FileName, file?.ContentType, allowedContentTypesArray, foundLayers, foundAllLayers);


                //Layer3 checker, check if given content-type is allowed
                bool foundCheckContentType = foundLayers.TryGetValue(FileCheckLayers.CheckContentType, out foundCheckContentType);
                if (foundCheckContentType || foundAllLayers)
                    IsContentTypeAllowed(file?.ContentType, allowedContentTypesArray);

                //Check file size.
                bool foundCheckFileSize = foundLayers.TryGetValue(FileCheckLayers.CheckFileSize, out foundCheckFileSize);
                if (foundCheckFileSize || foundAllLayers)
                {
                    if (file?.Length > maxSingleFileSize)
                    {
                        throw new BusinessException(string.Format($"File size {file?.Length} exceeded the {nameof(maxSingleFileSize)}."));
                    }
                }
            }
            catch (BusinessException ex)
            {
                //TODO: add throw exception option.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the file is allowed based on specific conditions.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="allowedExtensions">Comma separated, e.g: "txt,pdf,doc,docx". You need to pass FileCheckLayers.CheckExtensions as one of the layers.</param>
        /// <param name="allowedContentTypes">Comma separated, e.g: "text/plain,application/pdf,application/msword". You need to pass FileCheckLayers.CheckContentType as one of the layers.</param>
        /// <param name="maxSingleFileSize">Accepted file size in bytes. You need to pass FileCheckLayers.CheckFileSize as one of the layers.</param>
        /// <param name="layers">The check-points that you want the file to go through.</param>
        /// <returns>A Barrier Response that indicates if the file is allowed or not.</returns>
        public static BarrierResponse IsAllowed(this IFormFile file, string allowedExtensions, string allowedContentTypes, long? maxSingleFileSize, params FileCheckLayers[] layers)
        {
            //TODO: enhance the code bellow and IsFileAllowed to share same code.
            var response = new BarrierResponse();
            try
            {
                //Build the layers dictionay (since we will check the layers more than once).
                Dictionary<FileCheckLayers, bool> foundLayers = SetupLayersDictionary(layers);

                //Check null values.
                CheckNulls(file, allowedExtensions, allowedContentTypes, maxSingleFileSize, foundLayers);

                var allowedContentTypesArray = new string[] { };

                if (!string.IsNullOrWhiteSpace(allowedContentTypes))
                    allowedContentTypesArray = allowedContentTypes.Trim().Split(",");

                bool foundAllLayers = foundLayers.TryGetValue(FileCheckLayers.AllLayers, out foundAllLayers);


                //FileCheckLayers.CheckExtensions
                bool foundCheckExtensions = foundLayers.TryGetValue(FileCheckLayers.CheckExtensions, out foundCheckExtensions);
                if (foundCheckExtensions || foundAllLayers)
                {
                    var fileExtensionsAttribute = new FileExtensionsAttribute() { Extensions = allowedExtensions };
                    if (!fileExtensionsAttribute.IsValid(file?.FileName))
                    {
                        throw new BusinessException(string.Format($"File extension {file?.FileName} could not be found in the {nameof(allowedExtensions)}."), ErrorType.InvalidExtension);
                    };
                }


                //Layer2 checker, check if the recieved content-type matches the given file name.
                bool foundCheckFileContentTypeToFileExtension = foundLayers.TryGetValue(FileCheckLayers.CheckFileContentTypeToFileExtension, out foundCheckFileContentTypeToFileExtension);
                if (foundCheckFileContentTypeToFileExtension || foundAllLayers)
                    MatchContentTypeToFileName(file?.FileName, file?.ContentType, allowedContentTypesArray, foundLayers, foundAllLayers);


                //Layer3 checker, check if given content-type is allowed
                bool foundCheckContentType = foundLayers.TryGetValue(FileCheckLayers.CheckContentType, out foundCheckContentType);
                if (foundCheckContentType || foundAllLayers)
                    IsContentTypeAllowed(file?.ContentType, allowedContentTypesArray);

                //Check file size.
                bool foundCheckFileSize = foundLayers.TryGetValue(FileCheckLayers.CheckFileSize, out foundCheckFileSize);
                if (foundCheckFileSize || foundAllLayers)
                {
                    if (file?.Length > maxSingleFileSize)
                    {
                        throw new BusinessException(string.Format($"File size {file?.Length} exceeded the {nameof(maxSingleFileSize)}."), ErrorType.FileSizeExceeded);
                    }
                }
            }
            catch (BusinessException ex)
            {
                response.SetIsAllowed(false, ex.ErrorType);
                response.SetErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                response.SetIsAllowed(false, ErrorType.Unknown);
                response.SetErrorMessage(ex.Message);
            }

            return response;
        }

        private static void MatchContentTypeToFileName(string fileName, string givenContentType, string[] allowedContentTypes, Dictionary<FileCheckLayers, bool> layers, bool foundAllLayers)
        {
            var extention = "";
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var splittedName = fileName.Split(".");
                extention = splittedName[splittedName.Length - 1];
            }

            var expectedContentType = MimeTypeMapper.GetMimeType(extention);

            if (!string.IsNullOrWhiteSpace(expectedContentType) && !expectedContentType.Split(',').ToList().Any(a => a.Equals(givenContentType, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new BusinessException(string.Format($"Content-type of the file name {expectedContentType} does not match the content-type of the given file {givenContentType}."), ErrorType.ContentTypeToExtensionMismatch);
            }

            bool foundCheckContentType = layers.TryGetValue(FileCheckLayers.CheckContentType, out foundCheckContentType);
            if (foundCheckContentType || foundAllLayers)
            {
                IsContentTypeAllowed(expectedContentType, allowedContentTypes);
            }

        }

        private static void IsContentTypeAllowed(string givenContentType, string[] allowedContentTypes)
        {
            var extentionFound = false;
            var splittedExtionions = givenContentType.Split(',').ToList();
            foreach (var type in allowedContentTypes)
            {
                if(splittedExtionions.Any(a => a.Equals(type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    extentionFound = true;
                    break;
                }
            }
            
            if (!extentionFound)
            {
                throw new BusinessException(string.Format($"File content-type {givenContentType} could not be found in the {nameof(allowedContentTypes)}."), ErrorType.InvalidContentType);
            }
        }

        private static Dictionary<FileCheckLayers, bool> SetupLayersDictionary(FileCheckLayers[] layers)
        {
            var layersDictionary = new Dictionary<FileCheckLayers, bool>();
            foreach (var layer in layers)
            {
                layersDictionary.Add(layer, true);
            }

            return layersDictionary;
        }

        private static void CheckNulls(IFormFile file, string allowedExtensions, string allowedContentTypes, long? maxSingleFileSize, Dictionary<FileCheckLayers, bool> layers)
        {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }
            if (layers == null || layers.Count == 0) { throw new ArgumentNullException(nameof(layers)); }

            bool foundAllLayers = layers.TryGetValue(FileCheckLayers.AllLayers, out foundAllLayers);

            if (string.IsNullOrWhiteSpace(allowedExtensions))
            {
                bool foundLayer = layers.TryGetValue(FileCheckLayers.CheckExtensions, out foundLayer);

                if (foundLayer || foundAllLayers)
                    throw new ArgumentNullException(nameof(allowedExtensions));

            }

            if (string.IsNullOrWhiteSpace(allowedContentTypes))
            {
                bool foundLayer = layers.TryGetValue(FileCheckLayers.CheckContentType, out foundLayer);

                if (foundLayer || foundAllLayers)
                    throw new ArgumentNullException(nameof(allowedContentTypes));

            }

            if (!maxSingleFileSize.HasValue)
            {
                bool foundLayer = layers.TryGetValue(FileCheckLayers.CheckFileSize, out foundLayer);

                if (foundLayer || foundAllLayers)
                    throw new ArgumentNullException(nameof(maxSingleFileSize));
            }
        }
    }
}
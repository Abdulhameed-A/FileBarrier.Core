using System;
using System.Collections.Generic;
using System.Text;

namespace FileBarrier.Core.Models
{
    /// <summary>
    /// The possible check-points that you want the file to go through.
    /// </summary>
    public enum FileCheckLayers
    {
        CheckExtensions = 0,
        CheckFileContentTypeToFileExtension,
        CheckContentType,
        CheckFileSize,
        AllLayers
    }

    public enum ErrorType
    {
        InvalidExtension = 0,
        ContentTypeToExtensionMismatch,
        InvalidContentType,
        FileSizeExceeded,
        Unknown
    }
}

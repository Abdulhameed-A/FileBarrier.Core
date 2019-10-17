# FileBarrier.Core
FileBarrier.Core is a simple little library (for <b>.NET Core >= 2.1</b>) built to help you to check if `the uploaded file` is meeting the validation requirements you have by providing whitelist to it.
It is an extension method on <b>`IFormFile`</b>.

# Getting Started
Download the package form NuGet [FileBarrier.Core](https://www.nuget.org/packages/FileBarrier.Core/). Current release is 1.0.5.

# Usage
This library gives you different ways to check if the file is valid or not. You can check for:
- <b>CheckExtensions</b>: When you want to check for `the file extension` against a whitelist of the allowed file extensions (provided by you the developer).
- <b>CheckFileContentTypeToFileExtension</b>: This will check if the `MIME-type` of the provided file is correct for `the file extension`, then it will check against a whitelist of the allowed MIME-types (provided by you the developer).
- <b>CheckContentType</b>: will check if the `MIME-type` of the provided file is in the a whitelist of the allowed MIME-types (provided by you the developer).
- <b>CheckFileSize</b>: will check if the file size is <= a specific size (provided by you the developer).
- <b>AllLayers</b>: will check for all above layers.

Things you need to provide depends on the layers you want:
- If you want to use the <b>CheckExtensions</b> layer: then you need to provide `allowedExtensions` <b>a string contains the allowed extensions `comma-separated, e.g: "txt,pdf,doc,docx"`, otherwise an `ArgumentNullException` will be thrown.</b>
- If you want to use the <b>CheckFileContentTypeToFileExtension</b> layer: then you need to provide `allowedContentTypes` <b>a string contains the allowed extensions `comma-separated, e.g: "text/plain,application/pdf,application/msword"`.</b>
- If you want to use the <b>CheckContentType</b> layer: then you need to provide `allowedContentTypes` <b>a string contains the allowed extensions `comma-separated, e.g: "text/plain,application/pdf,application/msword"`, otherwise an `ArgumentNullException` will be thrown.</b>
- If you want to use the <b>CheckFileSize</b> layer: then you need to provide `maxSingleFileSize` <b>the maximun file sieze, otherwise an `ArgumentNullException` will be thrown.</b>
- If you want to check for <b>AllLayers</b>: then you need to provide `allowedExtensions, allowedContentTypes, and maxSingleFileSize`.

After providing the required information, then you can check if the file is allowed or not by calling either:
- <b>IsFileAllowed</b>: returns `boolean` to indicates if the file is allowed or not against the selected layers. E.g.:

```c#
...
private const string ALLOWED_MIME_TYPES = "image/png,image/jpeg";
private const string ALLOWED_EXTENSIONS = "png,jpeg";
public IFormFile File { get; set; }
...

public void OnlyPngOrJpeg(){
    var isAllowed = File.IsFileAllowed(ALLOWED_EXTENSIONS, ALLOWED_MIME_TYPES, null, FileCheckLayers.CheckExtensions, FileCheckLayers.CheckExtensions, FileCheckLayers.CheckFileContentTypeToFileExtension, FileCheckLayers.CheckContentType);
    if(!isAllowed) {
      //do whaterver you need to do.
    }
}

```

- <b>IsAllowed</b>: returns `BarrierResponse` to give you more information about on which layer the checking has occurred (if possible). E.g.:


```c#
...
private const string ALLOWED_MIME_TYPES = "image/png,image/jpeg";
private const string ALLOWED_EXTENSIONS = "png,jpeg";
public IFormFile File { get; set; }
...

public void OnlyPngOrJpeg(){
    var response = File.IsAllowed(ALLOWED_EXTENSIONS, ALLOWED_MIME_TYPES, null, FileCheckLayers.CheckExtensions, FileCheckLayers.CheckExtensions, FileCheckLayers.CheckFileContentTypeToFileExtension, FileCheckLayers.CheckContentType);
    if(!response.IsAllowed) {
      //do whaterver you need to do, e.g.:
      if(response.ErrorType == ErrorType.InvalidExtension)
        throw new Exception("Invalid file, only PNG or JPEG files are allowed.");
      
      //or e.g.:
      throw new Exception(response.ErrorMessage);
    }
}

```

# Models

The `BarrierResponse`:

```c#

    public class BarrierResponse
    {
        public string ErrorMessage { get; set; }
        public bool IsAllowed { get; set; } = true;
        public ErrorType? ErrorType { get; set; }
        
        ...
    }
```

The `ErrorType`:

```c#

    public enum ErrorType
    {
        InvalidExtension = 0,
        ContentTypeToExtensionMismatch,
        InvalidContentType,
        FileSizeExceeded,
        Unknown
    }

```

# Acknowledgements
The MimeType mapper is mainly built by @samuelneff: [https://github.com/samuelneff/MimeTypeMap](https://github.com/samuelneff/MimeTypeMap).

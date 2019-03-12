using System.Collections.Generic;
using System.Web;

namespace MovieDatabase
{
    public static class ImageHelper
    {
        public static readonly List<string> AllowedFileExtensions =
            new List<string> { ".jpg", ".jpeg", ".png" };

        public static readonly string UploadFolder = "/upload/";

        public static readonly string MappedUploadFolder = HttpContext.Current.Server.MapPath(UploadFolder);
    }
}
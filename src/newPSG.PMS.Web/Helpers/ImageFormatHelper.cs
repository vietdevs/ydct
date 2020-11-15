using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace newPSG.PMS.Web.Helpers
{
    public class ImageFormatHelper
    {
        public static ImageFormat GetRawImageFormat(byte[] fileBytes)
        {
            using (var ms = new MemoryStream(fileBytes))
            {
                var fileImage = Image.FromStream(ms);
                var imageFormat = fileImage.RawFormat;
                fileImage.Dispose();
                return imageFormat;
            }
        }
    }
}

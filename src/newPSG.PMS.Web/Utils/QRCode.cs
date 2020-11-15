using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace newPSG.PMS.Web.Utils
{
    public static class QRCode
    {
        public static Image Encode(string content, int width, int height)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    Height = width,
                    Width = height,
                    Margin = 0,
                    CharacterSet = "UTF-8"
                }
            };
           
            using (var bitmap = barcodeWriter.Write(content))
            {                 
                using (var stream = new MemoryStream())
                {
                    bitmap.MakeTransparent();
                    bitmap.Save(stream, ImageFormat.Png);
                    bitmap.MakeTransparent(); 
                    var image = Image.FromStream(stream);

                    // return stream.ToArray();
                    // pictureBox.Image = image;

                    return image;
                }
            }
        }

        public static byte[] EncodeToByte(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 150,
                    Width = 150,
                    Margin = 0
                }
            };

            using (var bitmap = barcodeWriter.Write(content))
            {
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }
}
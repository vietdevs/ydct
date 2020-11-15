
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using ViettelFileSigner;

namespace newPSG.PMS.Web.Utils
{
    public class HashFilePDF
    {

        public static String GetHashTypeImage_ExistedSignatureField(String src, X509Certificate[] certChain, int numberPage, string urlPath, float coorX, float coorY, float width, float height)
        {
            SignPdfFile pdfSig = new SignPdfFile();
            //DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigImage_ExistedSignatureField(numberPage
            //    , coorX, coorY, width, height, "", "", "", urlPath);

            DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigImageDefault(numberPage
             , coorX, coorY, width, height, urlPath);

            String base64Hash = pdfSig.createHash(src, certChain, displayConfig);
            var session = System.Web.HttpContext.Current.Session;
            session["pdfSig"] = pdfSig;
            return base64Hash;
        }
        public static String GetHashTypeRectangleTextDisplayConfigTableDefault(int numberPage, String src, X509Certificate[] certChain, out string file_dest)
        {
            var lst = new List<string>();
            lst.Add("321312321323");
            SignPdfFile pdfSig = new SignPdfFile();
            DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigTableDefault(numberPage, lst.ToArray());

            file_dest = src.Replace(".pdf", "");
            file_dest = file_dest + "table" + ".pdf";
            pdfSig.addPageEmpty(src, file_dest, displayConfig);
            String base64Hash = pdfSig.createHash(src, certChain, displayConfig);
            var session = System.Web.HttpContext.Current.Session;
            session["pdfSig"] = pdfSig;

            return base64Hash;
        }


        public static String GetHashTypeRectangleText(int numberPage, String src, X509Certificate[] certChain)
        {
            SignPdfFile pdfSig = new SignPdfFile();
            DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigRectangleText(numberPage, 10, 10, 200, 80,
                    null, DisplayConfig.SIGN_TEXT_FORMAT_4, CertUtils.GetCN(certChain[0]), "", "", DisplayConfig.DATE_FORMAT_1);

            String base64Hash = pdfSig.createHash(src, certChain, displayConfig);
            var session = System.Web.HttpContext.Current.Session;
            session["pdfSig"] = pdfSig;
            return base64Hash;
        }

        public static String GetHashTypeRectangleText2_ExistedSignatureField(String src, X509Certificate[] certChain, String displayText, String fieldName)
        {
            SignPdfFile pdfSig = new SignPdfFile();
            //DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigRectangleText(1, 10, 10, 200, 80,
            //        DisplayConfig.SIGN_TEXT_FORMAT_4, "Dương Ngọc Khánh", "Kiểm tra", "Hà Nội", DisplayConfig.DATE_FORMAT_1);
            DisplayConfig displayConfig = DisplayConfig.generateDisplayConfigRectangleText_ExistedSignatureField(1, 10, 10, 200, 80,
                    displayText, null, CertUtils.GetCN(certChain[0]), "", "", DisplayConfig.DATE_FORMAT_1);
            String base64Hash = pdfSig.createHashExistedSignatureField(src, certChain, displayConfig, fieldName);
            var session = System.Web.HttpContext.Current.Session;
            session["pdfSig"] = pdfSig;
            return base64Hash;
        }
    }
}
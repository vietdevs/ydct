using iTextSharp.text;
using iTextSharp.text.pdf;
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
    public static class PdfUtil
    {
        #region Functions Common

        public static string getPathPDFSizeA4(string filePDF, string strFooter)
        {
            var filePDFInfo = new FileInfo(filePDF);
            var _folder = filePDFInfo.Directory.FullName;
            var _filePdfSizeA4 = "SizeA4_" + filePDFInfo.Name;

            //Delete old file temp
            //AppFileHelper.DeleteFilesInFolderIfExists(_folder, _filePdfSizeA4);

            var _filePdfSizeA4Path = Path.Combine(_folder, _filePdfSizeA4);

            PdfReader reader = new PdfReader(filePDF);
            Document doc = new Document(PageSize.LETTER);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(_filePdfSizeA4Path, FileMode.OpenOrCreate));
            doc.Open();

            PdfContentByte cb = writer.DirectContent;
            writer.PageEvent = new Footer(strFooter);

            for (int i = 1; i < reader.NumberOfPages + 1; i++)
            {
                PdfImportedPage page = writer.GetImportedPage(reader, i);
                cb.AddTemplate(page, PageSize.LETTER.Width / reader.GetPageSize(i).Width, 0, 0, PageSize.LETTER.Height / reader.GetPageSize(i).Height, 0, 0);
                doc.NewPage();
            }

            doc.Close();


            return _filePdfSizeA4Path;
        }
        #endregion  
    }
}
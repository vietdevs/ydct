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
    public partial class Footer : PdfPageEventHelper
    {
        private string StrFooter { get; set; }
        public Footer() : base()
        {
        }
        public Footer(string strFooter) : base()
        {
            StrFooter = strFooter;
        }
        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            string _strFooter = !string.IsNullOrEmpty(this.StrFooter) ? this.StrFooter : "";

            Paragraph footer = new Paragraph(_strFooter, FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.NORMAL));
            footer.Alignment = Element.ALIGN_LEFT;
            PdfPTable footerTbl = new PdfPTable(1);
            footerTbl.TotalWidth = 300;
            footerTbl.HorizontalAlignment = Element.ALIGN_LEFT;
            PdfPCell cell = new PdfPCell(footer);
            cell.Border = 0;
            cell.PaddingLeft = 10;
            footerTbl.AddCell(cell);
            footerTbl.WriteSelectedRows(0, -1, 35, 35, writer.DirectContent);
        }
    }
}
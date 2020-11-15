using System;
using System.Collections.Generic;
using System.IO;
using Abp.Collections.Extensions;
using Abp.Dependency;
using newPSG.PMS.Dto;
using newPSG.PMS.Net.MimeTypes;
using OfficeOpenXml;

namespace newPSG.PMS.DataExporting.Excel.EpPlus
{
    public abstract class EpPlusExcelExporterBase : PMSServiceBase, ITransientDependency
    {
        public IAppFolders AppFolders { get; set; }

        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                Save(excelPackage, file);
            }

            return file;
        }

        protected void AddHeader(ExcelWorksheet sheet, int startRowIndex = 1, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                AddHeader(sheet, i + 1, headerTexts[i], startRowIndex);
            }
        }

        protected void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
            sheet.Cells[1, columnIndex].Style.Font.Color.SetColor(System.Drawing.Color.White);
            sheet.Cells[1, columnIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[1, columnIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
        }

        protected void AddHeader(ExcelWorksheet sheet, int columnIndex, string headerText, int rowIndex = 1)
        {
            sheet.Cells[rowIndex, columnIndex].Value = headerText;
            sheet.Cells[rowIndex, columnIndex].Style.Font.Bold = true;
            sheet.Cells[rowIndex, columnIndex].Style.Font.Color.SetColor(System.Drawing.Color.White);
            sheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[rowIndex, columnIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
        }

        protected void AddObjects<T>(ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        protected void Save(ExcelPackage excelPackage, FileDto file)
        {
            var filePath = Path.Combine(AppFolders.TempFileDownloadFolder, file.FileToken);
            excelPackage.SaveAs(new FileInfo(filePath));
        }
    }
}
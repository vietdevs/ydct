namespace newPSG.PMS
{
    public interface IAppFolders
    {
        string TempFileDownloadFolder { get; }
        string TempFileBaoCaoFolder { get; set; }
        string SampleProfileImagesFolder { get; }

        string WebLogsFolder { get; set; }
        string PDFTemplate { get; set; }
    }
}
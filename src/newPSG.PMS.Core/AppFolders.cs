using Abp.Dependency;

namespace newPSG.PMS
{
    public class AppFolders : IAppFolders, ISingletonDependency
    {
        public string TempFileDownloadFolder { get; set; }
        public string TempFileBaoCaoFolder { get; set; }
        public string SampleProfileImagesFolder { get; set; }

        public string WebLogsFolder { get; set; }
        public string PDFTemplate { get; set; }
    }
}
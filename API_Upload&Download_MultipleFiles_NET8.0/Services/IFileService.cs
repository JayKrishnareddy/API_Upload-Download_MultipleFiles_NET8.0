namespace API_Upload_Download_MultipleFiles_NET8._0.Services
{
    public interface IFileService
    {
        void UploadUsingFormFileCollection(IFormFileCollection files, string subDirectory);
        void UploadUsingFormFile(IFormFile files, string subDirectory);
        (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory);
        string SizeConverter(long bytes);
    }
}

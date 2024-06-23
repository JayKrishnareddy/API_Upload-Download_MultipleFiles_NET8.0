
using API_Upload_Download_MultipleFiles_NET8._0.Services;
using System.ComponentModel.DataAnnotations;

namespace API_Upload_Download_MultipleFiles_NET8._0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddTransient<IFileService, FileService>();
            builder.Services.AddAntiforgery(options =>
            {
                // Configure Antiforgery options, if needed
            });
            var app = builder.Build();
         
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Add UseAntiforgery between UseAuthentication and UseEndpoints
            app.UseAntiforgery();
            
            app.UseAuthorization();

            /// <summary>
            /// Uploads a file using IFormFile to a specified folder.
            /// </summary>
            /// <param name="formFile">The collection of files to upload.</param>
            /// <param name="folderName">The name of the folder to store the uploaded files.</param>
            /// <returns>An object containing information about the uploaded files.</returns>
            app.MapPost("/upload", (IFileService fileService, [Required] IFormFile formFile, [Required] string folderName) =>
            {
                try
                {
                    fileService.UploadUsingFormFile(formFile, folderName);
                    var size = fileService.SizeConverter(formFile.Length);
                    return Results.Ok(new { Size = size });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            }).DisableAntiforgery();

            /// <summary>
            /// Uploads a collection of files to a specified folder.
            /// </summary>
            /// <param name="formFiles">The collection of files to upload.</param>
            /// <param name="folderName">The name of the folder to store the uploaded files.</param>
            /// <returns>An object containing information about the uploaded files.</returns>
            app.MapPost("/uploadMultipleFiles", async (IFileService fileService, IFormFileCollection formFiles, [Required] string folderName) =>
            {
                if (formFiles.Count == 0 || string.IsNullOrEmpty(folderName))
                {
                    return Results.BadRequest("Please provide files and a subdirectory.");
                }
                try
                {
                    fileService.UploadUsingFormFileCollection(formFiles, folderName);
                    var size = fileService.SizeConverter(formFiles.Sum(f => f.Length));
                    return Results.Ok(new { FileCount = formFiles.Count, Size = size });
                }
                catch (Exception)
                {
                    return Results.BadRequest("Issue in upload API");
                }
            }).DisableAntiforgery();
            /// <summary>
            /// Download a collection of files to a specified folder.
            /// </summary>
            /// <param name="folderName">The name of the folder to download files.</param>
            /// <returns>Download the files from the folder.</returns>
            app.MapGet("/downloadFiles", (IFileService fileService, string folderName) =>
            {
                if (string.IsNullOrEmpty(folderName))
                {
                    return Results.BadRequest("Please provide folderName.");
                }
                var (fileType, archiveData, archiveName) = fileService.DownloadFiles(folderName);
                return Results.File(archiveData, fileType, archiveName);
            }).DisableAntiforgery();
            app.Run();
        }
    }
}


using ChatAppServer.Services.Interfaces;

namespace ChatAppServer.Services
{
    public class FileManagerServices : IFileManagerServices
    {

        // this is for uploading files to server
        public async Task<string> UploadFile(IFormFile file,string folderName = "medias")
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"uploads/{folderName}");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var fileExtension = file.FileName.Split('.').Last();
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString() + "." + fileExtension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Path.Combine($"uploads/{folderName}", uniqueFileName);
        }


        // this method is for delete file
        public void DeleteFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

    }
}

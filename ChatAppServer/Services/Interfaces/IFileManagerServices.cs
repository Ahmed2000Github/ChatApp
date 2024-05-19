using ChatAppCore.DTOs;

namespace ChatAppServer.Services.Interfaces
{
    public interface IFileManagerServices
    {
        public Task<string> UploadFile(IFormFile file, string folderName = "medias");
        public void DeleteFile(string fileName);
    }
}

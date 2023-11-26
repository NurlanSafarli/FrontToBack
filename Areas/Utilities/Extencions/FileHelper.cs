using FronyToBack.Models;
using FronyToBack.Utilities.Enums;

namespace FronyToBack.Utilities.Extencions
{
    public static class FileHelper
    {
        
        public static bool CheckFileType(this IFormFile file, FileType type)
        {
            if (type == FileType.Image)
            {
                if (file.ContentType.Contains("image/")) return true;
                return false;
            }


            return false;

        }

        public static async Task<string> CreateFileAsync(this IFormFile file, string root, params string[] folders)
        {
            string fileName  = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string path = root;

            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }

            path = Path.Combine(path, fileName);

            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;


            
        }

        public static void DeleteFile(this string fileName, string root, params string[] folders)
        {
            string path = root;

            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }

            path = Path.Combine(path, fileName);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

        }


    }
}

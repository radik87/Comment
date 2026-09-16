using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Commnents.Services
{
    public class FileService
    {
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };

        public async Task<string?> ProcessUploadedFileAsync(Stream fileStream, string fileName, long fileSize)
        {
            string ext = Path.GetExtension(fileName).ToLower();

            // 1. Обработка текстового файла
            if (ext == ".txt")
            {
                if (fileSize > 100 * 1024) // 100 KB
                    throw new Exception("Текстовый файл не должен превышать 100 КБ.");

                string txtPath = Path.Combine("wwwroot", "uploads", $"{Guid.NewGuid()}.txt");
                using FileStream fs = new FileStream(txtPath, FileMode.Create);
                await fileStream.CopyToAsync(fs);
                return txtPath;
            }

            // 2. Обработка изображения
            if (_allowedImageExtensions.Contains(ext))
            {
                using Image image = await Image.LoadAsync(fileStream);

                if (image.Width > 320 || image.Height > 240)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(320, 240),
                        Mode = ResizeMode.Max // Пропорциональное уменьшение
                    }));
                }

                string imgPath = Path.Combine("wwwroot", "uploads", $"{Guid.NewGuid()}{ext}");
                await image.SaveAsync(imgPath);
                return imgPath;
            }

            throw new Exception("Неподдерживаемый формат файла.");
        }
    }
}

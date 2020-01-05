using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LetsEncrypt.Core.IO
{
    public class LocalFileHandler
    {
        private const string FOLDER = "Output//";

        public LocalFileHandler()
        {
        }

        // Public Methods

        public async Task<string> ReadAsync(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"File on path '{path}' does not exists!");
            }

            using (var stream = File.OpenRead(path))
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task WriteAsync(string path, string text)
        {
            await WriteAsync(path, Encoding.UTF8.GetBytes(text));
        }

        public async Task WriteAsync(string path, byte[] data)
        {
            var fullPath = Path.GetFullPath(path);
            var dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var stream = File.Create(fullPath))
            {
                await stream.WriteAsync(data, 0, data.Length);
            }
        }

        //public async Task WriteAsync(string fileName, byte[] data)
        //{
        //    var path = GetOutputPath(fileName); // ".pfx"
        //    await WriteAsync(path, data);
        //}

        // Private Helper Methods

        private string GetOutputPath(string fileName)
        {
            var directoryPath = GetOutputDirectoryPath();
            return directoryPath + fileName;
        }

        private string GetOutputDirectoryPath()
        {
            var directoryPath = AppDomain.CurrentDomain.BaseDirectory + FOLDER;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(directoryPath));
            }

            return directoryPath;
        }
    }
}
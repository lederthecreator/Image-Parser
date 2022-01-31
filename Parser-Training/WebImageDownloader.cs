using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser_Training
{
    class WebImageDownloader : IDisposable
    {
        public static readonly HttpClient _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };

        public static async Task SaveImageAsync(string imageUrl)
        {
            try
            {
                Uri imageUri;
                
                if (Uri.TryCreate(imageUrl, UriKind.Absolute, out imageUri))
                {
                    using (HttpResponseMessage imageResponse = await _httpClient.GetAsync(imageUri))
                    {
                        // HTTP result != 200 OK -> HttpRequestException
                        imageResponse.EnsureSuccessStatusCode();
                        using (Stream imageStream = await imageResponse.Content.ReadAsStreamAsync())
                        {
                            await SaveImageAsync(imageUri, imageStream);
                        }
                    }
                }
                else
                {
                    //обрабатываем неверные url
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Not an absolute URI: {imageUrl}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            catch (HttpRequestException e)
            {
                //обрабатываем ошибки запросов
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{e.Message} : {imageUrl}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private static async Task SaveImageAsync(Uri imageUri, Stream imageStream)
        {
            /*берем Path сегмент 

              и пытаемся вычленить из него последнюю часть ресурса(до слеша) и его расширение

              https://i.stack.imgur.com/pnAAg.jpg?s=32&g=1 -> /pnAAg.jpg (path) -> pnAAg.jpg (regexp)

            */
            Match fileExtensionMatch = Regex.Match(imageUri.AbsolutePath, @"(?!/)[\w\d]+\.\w+", RegexOptions.RightToLeft);
            if (fileExtensionMatch.Success)
            {
                //создаем дерикторию для данного хоста картинки, чтобы хоть как-то их сгруппировать
                string imageDirectory = Path.Combine(Environment.CurrentDirectory, $"Images_{imageUri.Host.Replace('.', '_')}");

                if (!Directory.Exists(imageDirectory))
                    Directory.CreateDirectory(imageDirectory);

                string fileName = fileExtensionMatch.Value;
                string fullPathForFile = Path.Combine(imageDirectory, fileName);

                using (FileStream newFile = File.Create(fullPathForFile))
                {
                    await imageStream.CopyToAsync(newFile);
                    Console.WriteLine($"{imageUri.AbsoluteUri} ----> {fullPathForFile}");
                }
            }
            else
            {
                //обрабатываем отсутствие расширения у файла
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"No match for file name and extension in URL {imageUri.AbsoluteUri}");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

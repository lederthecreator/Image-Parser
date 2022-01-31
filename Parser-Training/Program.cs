using System;
using System.Net;
using System.Text;
using AngleSharp;
using AngleSharp.Html.Parser;
using Flurl.Http;

namespace Parser_Training
{
    class MyClass
    {
        static async Task Main()
        {
            var url = Console.ReadLine();
            try
            {
                if (url != null)
                {
                    await FindImagesAsync(url);
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task FindImagesAsync(string uri)
        {
            var html = await Parser.GetHtmlSourceCodeAsync(uri);
            if (!string.IsNullOrWhiteSpace(html))
            {
                var parser = new HtmlParser();
                var document = await parser.ParseDocumentAsync(html);

                var imagesList = document.QuerySelectorAll("img").ToArray();

                if (imagesList.Length > 4)
                {
                    var srcList = new List<string>();

                    for (int i = 0; i < 5; i += 1)
                    {
                        if (imagesList[i] != null && imagesList[i].GetAttribute("src") != null)
                        {
                            srcList.Add(imagesList[i].GetAttribute("src"));
                        }
                    }

                    foreach(var link in srcList)
                    {
                        Console.WriteLine(link);
                    }

                    SaveSourceCodeToFile(html);

                    SaveLinksToFile(srcList);

                    DownloadImages(srcList);
                }
                else
                {
                    throw new Exception("Cannot find 5 images.");
                }

                
            }
            else
            {
                throw new Exception("Source is empty.");
            }
        }

        public static void SaveSourceCodeToFile(string source)
        {
            File.WriteAllText("Results/index.html", source);
        }
        public static void SaveLinksToFile(List<string> links)
        {
            File.WriteAllLines("Results/links.txt", links);
        }

        public static async void DownloadImages(List<string> links)
        {
            var path = "C:\\Users\\User\\Pictures";
            foreach(var link in links)
            {
                var filename = Path.GetFileName(link);
                await Parser.DownloadImageAsync(path, filename, new Uri("https:"+link));
            }
        }

 
    }
}
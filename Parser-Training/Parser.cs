using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser_Training
{
    internal class Parser
    {
        public static async Task<string> GetHtmlSourceCodeAsync(string uri)
        {
            var httpClient = new HttpClient();
            try
            {
                var htmlSource = await httpClient.GetStringAsync(uri);
                return htmlSource;
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{nameof(HttpRequestException)}: {e.Message}");
                return null;
            }
        }
    }
}

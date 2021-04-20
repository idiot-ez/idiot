using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace IdiotBot.Services
{
    public sealed class WikiFeetService
    {
        private readonly HttpClient http;
        private readonly Random random = new Random();

        public WikiFeetService(HttpClient http)
            => this.http = http;

        public async Task<(bool some, Stream stream)> GetPictureAsync(string first, string last)
        {
            var resp = await http.GetAsync(string.Format("https://www.wikifeet.com/{0}_{1}", first, last));
            var stream = await resp.Content.ReadAsStreamAsync();

            var jsonPhotosDescriptors = string.Empty;
            using (var reader = new StreamReader(stream))
            {
                jsonPhotosDescriptors = reader.ReadToEnd().Split(new[] { '\n' }).FirstOrDefault(x => x.Contains("messanger['gdata']"));
            }

            if (string.IsNullOrWhiteSpace(jsonPhotosDescriptors))
                return (false, null);

            var line = jsonPhotosDescriptors.Trim(new[] { '\t' }).Split(new[] { '=' })[1].Trim();
            if (line.Count() > 1)
            {
                var objects = JsonArrayObjects.Parse(line);
                var index = random.Next() % objects.Count();
                var pid = objects[index].First(x => x.Key == "pid").Value;
                var pic = await http.GetAsync(string.Format("https://pics.wikifeet.com/{0}-{1}-Feet-{2}.jpg", first, last, pid));
                return (true, await pic.Content.ReadAsStreamAsync());
            }

            return (false, null);
        }
    }
}

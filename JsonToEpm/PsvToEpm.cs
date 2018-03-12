using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JsonToEpm
{
    class PsvToEpm
    {
        static Task Main(string[] args) => new PsvToEpm().StartAsync(args);

        public async Task StartAsync(string[] startupPath)
        {
            var path = startupPath.FirstOrDefault();
            Directory.EnumerateFiles(path, "course-info.json");
        }
    }
}

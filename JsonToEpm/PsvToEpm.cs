using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JsonToEpm
{
    public class PsvToEpm
    {
        private static LogService _logService;
        private static void Main(string[] args)
        {
            _logService = new LogService(new LoggerFactory());
            Parser.Default.ParseArguments<Options>(args).WithParsed(async x =>
            {
                var result = await new PsvToEpm().StartAsync(x);
            });
        }

        public async Task<Result> StartAsync(Options options)
        {
            var pathTest = PathTest(options.WorkDirectory);
            if (!pathTest.IsSuccess) return Result.FromFailure("Invalid file directory.", pathTest.Exception);

            var courseFiles = Directory.EnumerateFiles(options.WorkDirectory, "course-info.json",
                SearchOption.AllDirectories);
            foreach (var courseFile in courseFiles)
            {
                var path = Path.GetPathRoot(courseFile);
                if (File.Exists(Path.Combine(path, "show.metadata")) &&
                    File.Exists(Path.Combine(path, "show.summary")) &&
                    !options.EnforceScan) continue;
                var courseFileContent = await File.ReadAllTextAsync(courseFile);
                var courseInfo = JsonConvert.DeserializeObject<Course>(courseFileContent);
                await CreateMetadataAsync(path, courseInfo);
                await CreateSummaryAsync(path, courseInfo);
            }

            return Result.FromSuccess();
        }

        public Task CreateSummaryAsync(string path, Course course)
        {
            return File.WriteAllTextAsync(path, course.Description);
        }

        public Task CreateMetadataAsync(string path, Course course)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[metadata]");
            sb.AppendLine($"release={course.ReleaseDate:yyyy-MM-dd}");
            sb.AppendLine("studio=Pluralsight");
            sb.AppendLine($"genre={course.Level}");
            return File.WriteAllTextAsync(path, sb.ToString());
        }

        public (bool IsSuccess, Exception Exception) PathTest(string filePath)
        {
            try
            {
                Path.GetFullPath(filePath);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }

            return (true, null);
        }
    }
}
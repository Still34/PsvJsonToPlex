using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PsvJsonToPlex.Model;

namespace PsvJsonToPlex
{
    public class MainProgram
    {
        private static LogService _logService;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(async x =>
            {
                _logService = new LogService(new LoggerFactory(), x);
                _logService.Log(LogLevel.Information, $"Work path: {x.WorkDirectory}");
                var result = await new MainProgram().StartAsync(x);
                _logService.Log(result);
            });
        }

        public async Task<Result> StartAsync(Options options)
        {
            var pathTest = PathTest(options.WorkDirectory);
            if (!pathTest.IsSuccess) return Result.FromFailure("Invalid file directory.", pathTest.Exception);

            var courseFiles = Directory.GetFiles(options.WorkDirectory, "course-info.json",
                SearchOption.AllDirectories);
            foreach (var courseFile in courseFiles)
            {
                var path = Path.GetPathRoot(courseFile);
                _logService.Log(LogLevel.Debug, $"Processing in {path}...");
                if (File.Exists(Path.Combine(path, "show.metadata")) &&
                    File.Exists(Path.Combine(path, "show.summary")) &&
                    !options.EnforceScan)
                {
                    _logService.Log(LogLevel.Debug,
                        "Both metadata and summary exists and force scan is not specified, skipping...");
                    continue;
                }
                var courseFileContent = await File.ReadAllTextAsync(courseFile);
                var courseInfo = JsonConvert.DeserializeObject<Course>(courseFileContent);
                await CreateMetadataAsync(path, courseInfo);
                await CreateSummaryAsync(path, courseInfo);
            }

            return Result.FromSuccess();
        }

        public Task CreateSummaryAsync(string path, Course course)
        {
            _logService.Log(LogLevel.Information, $"Writing summary for {course.Title}...");
            return File.WriteAllTextAsync(path, course.Description);
        }

        public Task CreateMetadataAsync(string path, Course course)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[metadata]");
            sb.AppendLine($"release={course.ReleaseDate:yyyy-MM-dd}");
            sb.AppendLine("studio=Pluralsight");
            sb.AppendLine($"genre={course.Level}");
            _logService.Log(LogLevel.Information, $"Writing metadata for {course.Title}...");
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
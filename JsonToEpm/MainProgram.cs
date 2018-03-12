using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
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

        private static async Task Main(string[] args)
        {
            Options options = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(x => options = x)
                .WithNotParsed(x => Environment.Exit(0));
            _logService = new LogService(new LoggerFactory(), options);
            var result = await new MainProgram().StartAsync(options);
            _logService.Log(result);
            _logService.Log(LogLevel.Information, "Finished. Press any key to exit.");
            Console.ReadKey();
        }

        public async Task<Result> StartAsync(Options options)
        {
            var pathInfo = PathTest(options.WorkDirectory);
            if (!pathInfo.IsSuccess) return Result.FromFailure("Invalid file directory.", pathInfo.Exception);
            _logService.Log(LogLevel.Information, $"Work path: {pathInfo.FullPath}");
            try
            {
                var courseFiles = Directory.GetFiles(pathInfo.FullPath, "course-info.json",
                    SearchOption.AllDirectories);
                foreach (string courseFile in courseFiles)
                {
                    string path = Path.GetDirectoryName(courseFile);
                    _logService.Log(LogLevel.Debug, $"Processing in {path}...");
                    if (File.Exists(Path.Combine(path, "show.metadata")) &&
                        File.Exists(Path.Combine(path, "show.summary")) &&
                        !options.EnforceScan)
                    {
                        _logService.Log(LogLevel.Debug,
                            "Both metadata and summary exists and force scan is not specified, skipping...");
                        continue;
                    }

                    string courseFileContent = await File.ReadAllTextAsync(courseFile);
                    string trimmedCourse = courseFileContent.Replace("\\\"", "'");
                    if (trimmedCourse.StartsWith('[') && trimmedCourse.EndsWith(']'))
                        trimmedCourse = trimmedCourse.Substring(1, trimmedCourse.Length - 2);
                    _logService.Log(LogLevel.Debug, $"Text to be deserialized: {trimmedCourse}");
                    var courseInfo = JsonConvert.DeserializeObject<Course>(trimmedCourse);
                    await CreateMetadataAsync(path, courseInfo, options);
                    await CreateSummaryAsync(path, courseInfo, options);
                }
            }
            catch (Exception ex)
            {
                return Result.FromFailure(null, ex);
            }

            return Result.FromSuccess();
        }

        public Task CreateSummaryAsync(string path, Course course, Options options)
        {
            _logService.Log(LogLevel.Information, $"Writing summary for {course.Title}...");
            _logService.Log(LogLevel.Debug, $"Summary output:\n{course.Description}");
            return options.IsSimulation
                ? Task.CompletedTask
                : File.WriteAllTextAsync(Path.Combine(path, "show.summary"), course.Description);
        }

        public Task CreateMetadataAsync(string path, Course course, Options options)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[metadata]");
            sb.AppendLine($"release={course.ReleaseDate:yyyy-MM-dd}");
            sb.AppendLine("studio=Pluralsight");
            sb.AppendLine($"genres={course.Level}");
            _logService.Log(LogLevel.Information, $"Writing metadata for {course.Title}...");
            _logService.Log(LogLevel.Debug, $"Metadata output:\n{sb}");
            return options.IsSimulation
                ? Task.CompletedTask
                : File.WriteAllTextAsync(Path.Combine(path, "show.metadata"), sb.ToString());
        }

        public (string FullPath, bool IsSuccess, Exception Exception) PathTest(string filePath)
        {
            string fullpath = null;
            try
            {
                fullpath = Path.GetFullPath(filePath);
                if (!Directory.Exists(fullpath))
                    throw new DirectoryNotFoundException(
                        "Work directory is not found. Please make sure you entered a correct path.");
            }
            catch (Exception ex)
            {
                return (fullpath, false, ex);
            }

            return (fullpath, true, null);
        }
    }
}
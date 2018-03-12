using CommandLine;

namespace PsvJsonToPlex.Model
{
    public class Options
    {
        [Option('f', "force", Default = false, HelpText = "Force rebuild all summary files.")]
        public bool EnforceScan { get; set; }

        [Option('w', Required = true, HelpText = "The directory to scan in.")]
        public string WorkDirectory { get; set; }

        [Option('d', "debug", Default = false, HelpText = "Enables verbose logging.")]
        public bool IsDebug { get; set; }

        [Option('s', Default = false, HelpText = "Enables simulation (no files will be affected).")]
        public bool IsSimulation { get; set; }
    }
}
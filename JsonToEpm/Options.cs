using CommandLine;

namespace JsonToEpm
{
    public class Options
    {
        [Option('f', "force", Default = false, HelpText = "Force rebuild all summary files.")]
        public bool EnforceScan { get; set; }

        [Option(Required = true, HelpText = "The directory to scan in.")]
        public string WorkDirectory { get; set; }
    }
}
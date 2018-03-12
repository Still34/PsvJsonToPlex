using System.Threading.Tasks;
using CommandLine;

namespace JsonToEpm
{
    public class PsvToEpm
    {
        private static async Task Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(async x => await new PsvToEpm().StartAsync(x));
        }

        public async Task StartAsync(Options options)
        {
            
        }
    }

    public class Options
    {
        [Option('f', "force", Default = false, HelpText = "Force rebuild all summary files.")]
        public bool EnforceScan { get; set; }

        [Option(Required = true, HelpText = "The directory to scan in.")]
        public string ScanDirectory { get; set; }
    }
}
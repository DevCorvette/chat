using System.Threading.Tasks;
using Corvette.Chat.WebService.Configuration;
using Curiosity.Hosting;
using Curiosity.Hosting.Web;

namespace Corvette.Chat.WebService
{
    public class CliArgs : CuriosityCLIArguments
    {
        public CliArgs() : base("Chat Web Service")
        {
        }
    }
    
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            var bootstrapper = new CuriosityWebAppBootstrapper<CliArgs, WebServiceConfiguration, Startup>();
            return bootstrapper.RunAsync(args);
        }
    }
}
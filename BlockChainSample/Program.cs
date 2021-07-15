using System;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace BlockChainSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Get port
            if (!int.TryParse(args.Length > 0 ? args[0] : null, out var port))
            {
                Console.Write("Port:");
                var portStr = Console.ReadLine();

                if (!int.TryParse(portStr, out port))
                {
                    Console.WriteLine("Wrong Input!\nPort is 8080 as default.");
                    port = 8080;
                }
            }

            #region Host Init

            var config = new HttpSelfHostConfiguration($"http://localhost:{port}");
            config.MapHttpAttributeRoutes();

            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();

            Console.WriteLine($"Web API Server has started at {config.BaseAddress}");
          
            #endregion

            Console.ReadLine();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LocalApp
{
    internal class Program
    {
        // This holds all your registered tools and services
        public static IServiceProvider ServiceProvider { get; private set; }
        [STAThread]
        static void Main(string[] args)
        {
            // 1. Initialize the "Brain" (Dependency Injection)
            ServiceProvider = Startup.ConfigureServices();

            // 2. Resolve the Connection Manager to start the SignalR pipe
            var connectionManager = ServiceProvider.GetRequiredService<ConnectionManager>();

            // Start the background connection task
            try
            {
                // 3. Start the connection synchronously to avoid the 'async Main' error.
                // We use .GetAwaiter().GetResult() to block until it connects.
                // Note: Passing a dummy token since your StartAsync requires a B2B token string.
                connectionManager.StartAsync("dev_testing_token").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start connection: {ex.Message}");
            }

            // 4. Keep the application running in the background
            Console.WriteLine("Tekla Bridge is active and listening.");
            Console.WriteLine("Press ENTER to exit the application...");
            Console.ReadLine();
        }
    }
}

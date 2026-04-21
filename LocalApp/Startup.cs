using LocalApp.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp
{
    internal class Startup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 1. Register the Tekla Model as a Singleton
            // This creates one 'handle' to Tekla that stays alive as long as your app is open.
            services.AddSingleton<Tekla.Structures.Model.Model>();
            services.AddSingleton<TeklaDispatcher>();

            // 2. Register your Router and ConnectionManager
            services.AddSingleton<CommandRouter>();
            services.AddSingleton<ConnectionManager>();

            // 3. Auto-register all types that implement ITeklaCommandHandler
            var handlers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ITeklaCommandHandler).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var h in handlers) services.AddTransient(h);

            return services.BuildServiceProvider();
        }
    }
}

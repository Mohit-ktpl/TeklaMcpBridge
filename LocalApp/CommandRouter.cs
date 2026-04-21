using Contracts.Envelopes;
using LocalApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace LocalApp
{
    internal class CommandRouter
    {
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, Type> _routeTable;

        public CommandRouter(IServiceProvider services)

        {
            this._services = services;
            _routeTable = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<TeklaToolAttribute>() != null)
                .ToDictionary(
                    t => t.GetCustomAttribute<TeklaToolAttribute>().CommandName,
                    t => t,
                    StringComparer.OrdinalIgnoreCase);

        }
        public async Task<GenericEnvelope> RouteAsync(GenericEnvelope request)
        {
            if (!_routeTable.TryGetValue(request.CommandName, out var type))
                throw new Exception($"Tool {request.CommandName} not found.");

            var handler = (ITeklaCommandHandler)_services.GetRequiredService(type);
            var resultJson = await handler.ExecuteAsync(request.Payload);

            return new GenericEnvelope { CommandName = request.CommandName + "Result", Payload = resultJson };
        }
    }
}

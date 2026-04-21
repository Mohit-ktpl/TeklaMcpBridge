using Contracts.Envelopes;
using Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR.Client;

namespace LocalApp
{
    internal class ConnectionManager
    {
        private HubConnection _connection;
        private readonly CommandRouter _router;
        private readonly string _hubUrl = "https://localhost:7268/teklahub"; // Replace with AWS URL

        public ConnectionManager(CommandRouter router)
        {
            _router = router;
        }

        public async Task StartAsync(string b2bToken)
        {
            // 1. Configure the Connection
            var hubUrlWithUser = $"{_hubUrl}?userId=dev_user";
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrlWithUser, options =>
                {
                    // Attach your B2B JWT Token for security
                    options.AccessTokenProvider = () => Task.FromResult(b2bToken);
                })
                // CRITICAL: Must match the Server's protocol to avoid serialization errors
                .AddNewtonsoftJsonProtocol()
                .WithAutomaticReconnect(new[] {
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                })
                .Build();

            // 2. The Core Bridge: Listening for "ExecuteGenericCommand"
            // This matches the ITeklaClient interface method name exactly
            _connection.On<GenericEnvelope, GenericEnvelope>(
                nameof(ITeklaClient.ExecuteGenericCommand),
                async (requestEnvelope) =>
                {
                    try
                    {
                        // Pass the envelope to the Router (O(1) lookup)
                        return await _router.RouteAsync(requestEnvelope);
                    }
                    catch (Exception ex)
                    {
                        // Return a failure envelope so the Cloud/Claude knows what went wrong
                        return new GenericEnvelope
                        {
                            CommandName = requestEnvelope.CommandName + "_Error",
                            Payload = $"Local Error: {ex.Message}"
                        };
                    }
                });

            // 3. Lifecycle Events (Useful for UI Status Icons)
            _connection.Closed += (error) => {
                Console.WriteLine("Connection Lost. Trying to recover...");
                return Task.CompletedTask;
            };

            // 4. Fire it up
            try
            {
                await _connection.StartAsync();
                Console.WriteLine("SignalR Bridge Connected to AWS.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initial connection failed: {ex.Message}");
            }
        }

        public async Task StopAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}

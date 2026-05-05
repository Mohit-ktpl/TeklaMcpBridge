using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalApp.Core
{
    internal class TeklaDispatcher
    {
        private readonly BlockingCollection<Action> _executionQueue;
        public TeklaDispatcher()
        {
            _executionQueue = new BlockingCollection<Action>();

            // Spin up a dedicated background thread specifically for Tekla
            var teklaThread = new Thread(RunLoop)
            {
                IsBackground = true,
                Name = "Tekla_STA_Dispatcher"
            };

            // CRITICAL: Set to STA (Single-Threaded Apartment) for COM interop stability
            teklaThread.SetApartmentState(ApartmentState.STA);
            teklaThread.Start();

        }
        private void RunLoop()
        {
            // This loop runs forever, picking up tasks one by one
            foreach (var action in _executionQueue.GetConsumingEnumerable())
            {
                try
                {
                    action(); // Execute the Tekla API code
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Tekla Dispatcher Error: {ex.Message}");
                }
            }
        }
        // The method, Handlers will use to queue work
        public Task<T> InvokeAsync<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            Console.WriteLine($"[Dispatcher] Received request of type {typeof(T).Name}");

            _executionQueue.Add(() =>
            {
                try
                {
                    // Run the function and send the result back to the Task
                    Console.WriteLine("[Dispatcher] Executing on STA thread...");
                    T result = func();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Dispatcher] Execution Error: {ex}");
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}

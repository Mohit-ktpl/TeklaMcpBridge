using Contracts.Models;
using LocalApp.Core;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace LocalApp.Handlers
{
    [TeklaTool("UpdateBeamProfile")]
    internal class UpdateBeamProfile:ITeklaCommandHandler
    {
        private readonly Model model;
        private readonly TeklaDispatcher dispatcher;

        public UpdateBeamProfile(Model model, TeklaDispatcher dispatcher)
        {
            this.model = model;
            this.dispatcher = dispatcher;
        }

        public async Task<string> ExecuteAsync(string jsonPayload)
        {
            var data = JsonConvert.DeserializeObject<UpdateBeamsByGuidDto>(jsonPayload);

            var result = await dispatcher.InvokeAsync(() =>
            {
                int count = 0;
                foreach (var guidString in data.TargetGuids)
                {
                    // Find the exact object by its GUID
                    var obj = model.SelectModelObject(new Tekla.Structures.Identifier(new Guid(guidString)));

                    if (obj is Beam beam)
                    {
                        beam.Class = data.NewClass.ToString();
                        beam.Modify();
                        count++;
                    }
                }
                model.CommitChanges();
                return new SharedResult { Success = true, Message = $"Updated {count} beams." };
            });
            return JsonConvert.SerializeObject(result);
        }
    }
}

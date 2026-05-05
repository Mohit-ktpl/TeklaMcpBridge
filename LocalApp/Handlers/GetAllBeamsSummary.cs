using LocalApp.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

using Contracts.Models;
namespace LocalApp.Handlers
{
    [TeklaTool("GetAllBeamsSummary")]
    internal class GetAllBeamsSummaryHandler : ITeklaCommandHandler
    {
        private readonly Model _model;
        private readonly TeklaDispatcher _dispatcher;

        public GetAllBeamsSummaryHandler(Model model, TeklaDispatcher dispatcher) {
            this._model = model;
            this._dispatcher = dispatcher;
        }

        public async Task<string> ExecuteAsync(string jsonPayload)
        {
            var result = await _dispatcher.InvokeAsync(() =>
            {
                var summaries = new List<BeamSummaryDto>();
                var enumerator = _model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is Beam beam)
                    {
                        summaries.Add(new BeamSummaryDto
                        {
                            Guid = beam.Identifier.GUID.ToString(),
                            Profile = beam.Profile.ProfileString
                        });
                    }
                }
                //  return the raw data
                return summaries;
            });

            return JsonConvert.SerializeObject(result);
        }
    }
}

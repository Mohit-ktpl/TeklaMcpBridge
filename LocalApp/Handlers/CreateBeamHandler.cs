using Contracts.Models;
using LocalApp.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;

namespace LocalApp.Handlers
{
    [TeklaTool("CreateBeam")]
    internal class CreateBeamHandler : ITeklaCommandHandler

    {
        private readonly Model _model;
        private readonly TeklaDispatcher dispatcher;

        public CreateBeamHandler(Tekla.Structures.Model.Model model, TeklaDispatcher dispatcher)
        {
            _model = model;
            this.dispatcher = dispatcher;
        }
        public async Task<string> ExecuteAsync(string jsonPayload)
        {
            var data = JsonConvert.DeserializeObject<BeamCreateDto>(jsonPayload);
            // SAFELY jump to the Tekla STA Thread
            var result = await dispatcher.InvokeAsync(() =>
            {
                if (!_model.GetConnectionStatus())
                    return new SharedResult { Success = false, Message = "Not connected" };

                // Now we are safe to call the Tekla API!
                var startPoint = new Point(data.StartX, 0, 0);
                var endPoint = new Point(data.EndX, 0, 0);

                var beam = new Beam(startPoint, endPoint);
                beam.Profile.ProfileString = data.Profile;
                beam.Material.MaterialString = data.Material;

                bool isInserted = beam.Insert();
                _model.CommitChanges();

                if (isInserted)
                    return new SharedResult { Success = true, CreatedObjectGuid = beam.Identifier.GUID.ToString() };
                else
                    return new SharedResult { Success = false, Message = "Failed to insert beam" };
            });

            return JsonConvert.SerializeObject(result);
        }
    }

    [TeklaTool("CreateSimpleBeam")]
    internal class CreateSimpleBeamHandler : ITeklaCommandHandler

    {
        private readonly Model _model;
        private readonly TeklaDispatcher dispatcher;

        public CreateSimpleBeamHandler(Tekla.Structures.Model.Model model, TeklaDispatcher dispatcher)
        {
            _model = model;
            this.dispatcher = dispatcher;
        }
        public async Task<string> ExecuteAsync(string jsonPayload)
        {
            var data = JsonConvert.DeserializeObject<SimpleBeamCreateDto>(jsonPayload);
            // SAFELY jump to the Tekla STA Thread
            var result = await dispatcher.InvokeAsync(() =>
            {
                if (!_model.GetConnectionStatus())
                    return new SharedResult { Success = false, Message = "Not connected" };

                // Now we are safe to call the Tekla API!
                var startPoint = new Point(data.StartX, data.StartY, data.StartH);
                var endPoint = new Point(data.EndX, data.EndY, data.EndH);

                var beam = new Beam(startPoint, endPoint);
                beam.Profile.ProfileString = data.Profile;
                beam.Material.MaterialString = data.Material;

                bool isInserted = beam.Insert();
                _model.CommitChanges();

                if (isInserted)
                    return new SharedResult { Success = true, CreatedObjectGuid = beam.Identifier.GUID.ToString() };
                else
                    return new SharedResult { Success = false, Message = "Failed to insert beam" };
            });

            return JsonConvert.SerializeObject(result);
        }
    }

    }

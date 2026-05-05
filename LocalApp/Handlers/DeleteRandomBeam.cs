using Contracts.Models;
using LocalApp.Core;
using Newtonsoft.Json;
using RenderData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace LocalApp.Handlers
{
    [TeklaTool("DeleteRandomBeam")]
    internal class DeleteRandomBeam: ITeklaCommandHandler
    {
        private readonly Model model;
        private readonly TeklaDispatcher dispatcher;

        public DeleteRandomBeam(Tekla.Structures.Model.Model model, TeklaDispatcher dispatcher)
        {
            this.model = model;
            this.dispatcher = dispatcher;
        }

        public async Task<string> ExecuteAsync(string jsonPayload)
        {
            var result = await dispatcher.InvokeAsync(() =>
            {
                if (!model.GetConnectionStatus())
                    return new SharedResult { Success = false, Message = "Not connected" };

                var enumerator = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
                var list = new System.Collections.Generic.List<ModelObject>();
                while (enumerator.MoveNext()) list.Add(enumerator.Current);

                if (list.Count > 0)
                {
                    list[new Random().Next(list.Count)].Delete();
                    model.CommitChanges();
                    return new SharedResult { Success = true, Message = "Beam Deleted Successfuly" };
                }
                else
                {
                    return new SharedResult { Success = false, Message = "Failed to delete beam" };
                }

              
            });

            return JsonConvert.SerializeObject(result);
        }
    }
}

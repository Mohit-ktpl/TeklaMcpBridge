using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp.Core
{
    internal interface ITeklaCommandHandler
    {
        Task<string> ExecuteAsync(string jsonPayload);
    }
}

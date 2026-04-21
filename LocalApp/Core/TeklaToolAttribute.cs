using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class TeklaToolAttribute: Attribute
    {
        public string CommandName { get; }
        public TeklaToolAttribute(string commandName)
        {
            CommandName = commandName;
        }
    }
}

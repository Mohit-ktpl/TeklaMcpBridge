using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Envelopes
{
    public class GenericEnvelope
    {
        public string CommandName { get; set; }  // "GetBeamData" or "CreateBeam"
        public string Payload { get; set; }     // json string of actual tool data
    }
}

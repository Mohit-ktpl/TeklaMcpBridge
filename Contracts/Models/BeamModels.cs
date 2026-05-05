using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Models
{
    public class BeamCreateDto
    {
        public string Profile { get; set; }
        public string Material { get; set; }
        public double StartX { get; set; }
        public double EndX { get; set; }
    }
    public class SimpleBeamCreateDto
    {
        public string Profile { get; set; }
        public string Material { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double StartH { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double EndH { get; set; }
    }
    public class UpdateBeamClass
    {
        public string Profile { get; set; }
        public int Class { get; set; }
      
    }
    // Used to pass data from Local App to Server
    public class BeamSummaryDto
    {
        public string Guid { get; set; }
        public string Profile { get; set; }
    }

    // Used to command the Local App from the Server
    public class UpdateBeamsByGuidDto
    {
        public List<string> TargetGuids { get; set; }
        public int NewClass { get; set; }
    }

    public class SharedResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string CreatedObjectGuid { get; set; }
    }
}

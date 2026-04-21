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

    public class SharedResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string CreatedObjectGuid { get; set; }
    }
}

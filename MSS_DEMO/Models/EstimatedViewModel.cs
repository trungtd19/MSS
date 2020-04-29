using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSS_DEMO.Models
{
    public class EstimatedViewModel
    {
        public string Roll { get; set; }
        public string Email { get; set; }
        public string Campus { get; set; }
        public double TotalEstimated { get; set; }
        public double Compulsory { get; set; }
        public double NonCompulsory { get; set; }
        public List<EstimatedViewModel> EstimatedModel { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class EntrantDto
    {
        public Guid PlayerId { get; set; }
        public string Tag { get; set; }
        //public string Name { get; set; }
        public int Seed { get; set; }
        public int Placing { get; set; }
    }
}

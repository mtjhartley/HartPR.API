using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public double Trueskill { get; set; }
        public int SggPlayerId { get; set; }
    }
}

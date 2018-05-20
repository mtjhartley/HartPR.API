using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class PlayerPMDto : PlayerDto
    {
        public double? Trueskill { get; set; }
    }
}
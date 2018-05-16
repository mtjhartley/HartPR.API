using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class SetDtoForTournament
    {
        public string Winner { get; set; }
        public string Loser { get; set; }
        public int? WinnerScore { get; set; }
        public int? LoserScore { get; set; }
        public Guid WinnerId { get; set; }
        public Guid LoserId { get; set; }
    }
}

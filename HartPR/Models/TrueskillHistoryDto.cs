using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class TrueskillHistoryDto
    {
        public double Trueskill { get; set; }
        public string TournamentName { get; set; }
        public string PlayerName { get; set; }
        public Guid PlayerId { get; set; }
        public DateTimeOffset TournamentDate { get; set; }
    }
}

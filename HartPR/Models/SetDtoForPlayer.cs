using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class SetDtoForPlayer
    {
        public string Opponent { get; set; }
        public Guid OpponentId { get; set; }
        public bool isWin { get; set; }
        public int? WinnerScore { get; set; }
        public int? LoserScore { get; set; }
        public string Tournament { get; set; }
        public Guid TournamentId { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}

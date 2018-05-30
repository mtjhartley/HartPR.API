using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Entities
{
    public class PlayerTournament
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid TournamentId { get; set; }
        public int Seed { get; set; }
        public int Placing { get; set; }
    }
}

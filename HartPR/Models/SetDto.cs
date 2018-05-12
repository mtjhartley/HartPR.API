using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class SetDto
    {
        public Guid Id { get; set; }

        public Guid WinnerId { get; set; }

        public Guid LoserId { get; set; }

        public Guid TournamentId { get; set; }
    }
}

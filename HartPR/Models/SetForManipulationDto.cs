using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public abstract class SetForManipulationDto
    {
        [Required]
        public Guid Entrant1Id { get; set; }

        [Required]
        public Guid Entrant2Id { get; set; }

        [Required]
        public Guid WinnerId { get; set; }

        [Required]
        public Guid LoserId { get; set; }

        [Required]
        public Guid TournamentId { get; set; }
    }
}

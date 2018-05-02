using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace HartPR.Entities
{
    public class Set
    {
        [Key]
        public Guid Id { get; set; }
        
        //Could these be SGG if I choose to only support SGG tournaments?

        //Should these be my Database Player Ids? 

        //Should these be the actual entrantids on challonge/sgg, and then use a lookup table for my Guid to this entrantId? pros/cons?
        [Required]
        public Guid Entrant1Id { get; set; }

        [Required]
        public Guid Entrant2Id { get; set; }

        [Required]
        public Guid WinnerId { get; set; }

        [Required]
        public Guid LoserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        [ForeignKey("TournamentId")]
        public Tournament Tournament { get; set; }

        public Guid TournamentId { get; set; }
    }
}

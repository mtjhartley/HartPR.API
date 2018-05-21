using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HartPR.Helpers;

namespace HartPR.Entities
{
    //TODO Get rid of player guids? Rely solely on SmashGG Player ids?
    public class Player
    {
        [Key]
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Tag { get; set; }

        [Required]
        public string State { get; set; }

        //[Required]
        public double? Trueskill { get; set; }

        //public double? MeleeTrueskill { get; set; }
        //public double? Smash4Trueskill { get; set; }
        //public double? PMTrueskill { get; set; }
        //public double? Smash5Trueskill { get; set; }

        public int SggPlayerId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public DateTimeOffset? LastActive { get; set; }

        public Guid? UserId { get; set; }

    }
}

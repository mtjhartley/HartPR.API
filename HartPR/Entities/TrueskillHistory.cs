using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HartPR.Entities
{
    public class TrueskillHistory
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public double Trueskill { get; set; }
        //public string TournamentName { get; set; }
        public Guid TournamentId {get;set;}
        public DateTimeOffset TournamentDate { get; set; }

    }
}

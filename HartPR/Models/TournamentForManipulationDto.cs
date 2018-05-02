using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public abstract class TournamentForManipulationDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Website { get; set; }
        public string Subdomain { get; set; }
        [Required]
        public string URL { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Date { get; set; }
    }
}
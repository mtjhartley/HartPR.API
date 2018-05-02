using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Entities
{
    public class Tournament
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Website { get; set; }
        public string Subdomain { get; set; }
        [Required]
        public string URL { get; set; }
        [Required]
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        //TODO: Implement Sets and Players within the tournament later? With an option to grab them or not?
        //    public ICollection<Set> Sets { get; set; }
        //= new List<Set>();

        //    public ICollection<Player> Players { get; set; }
        //= new List<Player>();

    }
}

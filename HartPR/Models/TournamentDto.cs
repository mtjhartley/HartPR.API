using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class TournamentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Subdomain { get; set; }
        public string URL { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}

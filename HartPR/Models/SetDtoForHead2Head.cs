using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Models
{
    public class SetDtoForHead2Head
    {
        public string Winner { get; set; }
        public string Loser { get; set; }
        public Guid WinnerId { get; set; }
        public Guid LoserId { get; set; }
        public string Tournament { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}

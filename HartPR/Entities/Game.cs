using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HartPR.Helpers;

namespace HartPR.Entities
{
    public class Game
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Byte Enum { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Event { get; set; }
    }
}
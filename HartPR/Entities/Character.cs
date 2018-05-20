using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HartPR.Entities
{
    public class Character
    {
        [Key]
        public Byte Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

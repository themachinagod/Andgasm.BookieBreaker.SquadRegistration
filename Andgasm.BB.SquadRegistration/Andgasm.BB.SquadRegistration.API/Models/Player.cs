using System;
using System.ComponentModel.DataAnnotations;

namespace Andgasm.BB.SquadRegistration.API
{
    public class Player
    {
        [Key]
        public string Key { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public DateTime? DoB { get; set; }
        public string Positions { get; set; }
        public string CountryKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Andgasm.BB.SquadRegistration.API
{
    public class PlayerSquadRegistrationModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public DateTime? DoB { get; set; }
        public string Positions { get; set; }
        public string FullName { get { return string.Format("{0} {1}", Name, Surname); } }

        public string PlayerCode { get; set; }
        public string CountryCode { get; set; }
        public string ClubCode { get; set; }
        public string SeasonCode { get; set; }
    }
}

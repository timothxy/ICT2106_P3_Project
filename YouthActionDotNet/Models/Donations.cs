using System;
using Newtonsoft.Json;

namespace YouthActionDotNet.Models{

    public class Donations {
        public string DonationId { get; set; }

        public string DonationType  { get; set; }

        public string DonationAmount { get; set; }

        public string DonationContstraint { get; set; } 

        public DateTime DonationDate { get; set; }

        public string DonorId { get; set; }

        public string ProjectId { get; set; }

        [JsonIgnore]
        public virtual Donor donor { get; set; }
        [JsonIgnore]
        public virtual Project project { get; set; }
    }
}
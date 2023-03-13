using System;
using Newtonsoft.Json;

namespace YouthActionDotNet.Models
{
    public class Budget
    {
        public Budget()
        {
            //this.ServiceCenterId = Guid.NewGuid().ToString();
            this.BudgetId = Guid.NewGuid().ToString();
        }
        // public string ServiceCenterId { get; set; }
        // public string ServiceCenterName { get; set; }
        // public string ServiceCenterAddress { get; set; }
        // public string RegionalDirectorId { get; set; }

        // [JsonIgnore]
        // public virtual Employee RegionalDirector { get; set; }
        public double ActualExpenses{get;set;}

        //temp creation of ID
        public string BudgetId{get;set;}

    }
}
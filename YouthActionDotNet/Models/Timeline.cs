using System;
using Newtonsoft.Json;

namespace YouthActionDotNet.Models
{
    public class Timeline
    {
        public Timeline()
        {
            //Temporary Creation of ID
            this.ProjectTimelineId = Guid.NewGuid().ToString();
            //this.ServiceCenterId = Guid.NewGuid().ToString();
        }
        // public string ServiceCenterId { get; set; }
        // public string ServiceCenterName { get; set; }
        // public string ServiceCenterAddress { get; set; }
        // public string RegionalDirectorId { get; set; }

        // [JsonIgnore]
        // public virtual Employee RegionalDirector { get; set; }
        public string ProjectTimelineId{get;set;}
        public string ProjectStatus{get;set;}
        public DateTime projectStartDate{get;set;}
        public DateTime projectEndDate{get;set;}
        public DateTime projectCompletionDate{get;set;}
    }
}
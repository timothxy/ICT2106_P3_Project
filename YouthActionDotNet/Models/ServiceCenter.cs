namespace YouthActionDotNet.Models
{
    public class ServiceCenter
    {
        public string ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; }
        public string ServiceCenterAddress { get; set; }
        public string RegionalDirectorId { get; set; }

        public virtual string RegionalDirectorName { get; set; }
        public virtual Employee RegionalDirector { get; set; }
    }
}
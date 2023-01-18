namespace YouthActionDotNet.Models
{
    public class ServiceCenter
    {
        public int id { get; set; }
        public string ServiceCenterName { get; set; }
        public string ServiceCenterAddress { get; set; }
        public string RegionalDirectorId { get; set; }

        public virtual Employee RegionalDirector { get; set; }
    }
}
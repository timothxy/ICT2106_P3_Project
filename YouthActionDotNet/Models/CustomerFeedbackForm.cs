using System;
using Newtonsoft.Json;

namespace YouthActionDotNet.Models{

    public class CustomerFeedbackForm {
        public CustomerFeedbackForm(){
            this.customerFeedbackID = Guid.NewGuid().ToString();
        }

        public string customerFeedbackID { get; set; }

        public DateTime  customerFeedbackDate  { get; set; }

        public string customerContact { get; set; }

        public Question questions { get; set; } 

    }
}

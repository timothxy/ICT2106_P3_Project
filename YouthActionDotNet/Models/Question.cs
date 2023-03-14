using System;
using Newtonsoft.Json;

namespace YouthActionDotNet.Models{

    public abstract class Question {
        public Question(){
            this.questionID = Guid.NewGuid().ToString();
        }

        public string questionID { get; set; }

        public string questionContent  { get; set; }
        
        public abstract void setAnswer();
    }
}

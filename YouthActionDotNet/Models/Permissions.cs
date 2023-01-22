using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace YouthActionDotNet.Models{

    public class Permissions{
        public Permissions(){
            this.Id = Guid.NewGuid().ToString();
            using (StreamReader r = new StreamReader("./PermissionTemplate/permission.json")){
                string json = r.ReadToEnd();
                List<Permission> items = JsonConvert.DeserializeObject<List<Permission>>(json);
                this.Permission = JsonConvert.SerializeObject(items);
            }
            
        }

        public string Id { get; set; }

        public string Role { get; set; }

        public string Permission { get; set; }

    }

    public class Permission{
        public string Module { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
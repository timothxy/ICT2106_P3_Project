using System.Collections.Generic;

namespace YouthActionDotNet.Models
{
    public class Settings
    {
        public Dictionary<string, ColumnHeader> ColumnSettings;

        public Dictionary<string, InputType> FieldSettings;
    }


    public class InputType{
        public string type { get; set; }
        public string displayLabel { get; set; }
        public bool editable { get; set; }   
        public bool primaryKey { get; set; }
        
    }

    public class DropdownInputType : InputType{
        public List<DropdownOption> options { get; set; }
    }

    public class DropdownOption{
        public string value { get; set; }
        public string label { get; set; }
    }

    public class ColumnHeader{
        public string displayHeader { get; set; }
    }


}
using System;
using System.Collections.Generic;

namespace PriveSportsEmployees.Controls
{
    public class PostModify
    {
        public string file { get; set; }
        public string system { get; set; }
        public int record { get; set; }
        public List<ModifyField> contents { get; set; }
    }

    public class ModifyField
    {
        public string id { get; set; }
        public object value { get; set; }
    }
}


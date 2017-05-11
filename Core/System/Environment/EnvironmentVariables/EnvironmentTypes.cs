using System;
using System.Collections.Generic;

namespace CFHelper.Types
{
    public class VCAP_APPLICATION
    {
        public string           application_id { get; set; }
        public string           application_name { get; set; } 
        public IList<string>    application_uris { get; set; } 
        public string           application_version { get; set; } 
        public string           cf_api { get; set; } 
        public string           host { get; set; } 
        public string           instance_id { get; set; } 
        public int              instance_index { get; set; } 
        public IDictionary<string, int>  limits { get; set; } 
        public string           name { get; set; } 
        public int              port { get; set; } 
        public string           space_id { get; set; } 
        public string           space_name { get; set; } 
        public IList<string>    uris { get; set; } 
        public string           version { get; set; } 
    };

}
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class Stock
    {
        private Stock() { }

        [DataMember(Name="available")]
        public int Available { get; set; }

        [DataMember(Name="unit")]
        public string Unit { get; set; }
    }
}

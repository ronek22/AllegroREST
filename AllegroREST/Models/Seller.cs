using System;
using System.Runtime.Serialization;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class Seller
    {
        private Seller() {}

        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name="company")]
        public bool IsCompany { get; set; }

        [DataMember(Name="superSeller")]
        public bool IsSuperSeller { get; set; }
    }
}
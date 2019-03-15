using System;
using System.Runtime.Serialization;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class Category {
        private Category() {}
        
        [DataMember(Name="id")]
        public string Id {get; set;}

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="count")]
        public int Count { get; set; }

    }
}
using System;
using System.Runtime.Serialization;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class SellingMode
    {
        private SellingMode() {}

        [DataMember(Name="format")]
        public string Format { get; set; }

        [DataMember(Name="price")]
        public Price Price { get; set; }

    }
}
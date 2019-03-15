using System;
using System.Runtime.Serialization;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class Item {
        private Item() {}
        
        [DataMember(Name="id")]
        public string Id {get; set;}

        [DataMember(Name="seller")]
        public Seller Seller {get; set;}

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="sellingMode")]
        public SellingMode SellingMode { get; set; }

        public override string ToString() {
            return string.Format("{0, -10} | {1, -50} | {2,-30}", Seller.Id, Name, SellingMode.Price.Value);
        }

    }
}
using System;
using System.Runtime.Serialization;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class Price
    {
        private Price() {}

        [DataMember(Name="amount")]
        public string Amount { get; set; }

        [DataMember(Name="currency")]
        public string Currency { get; set; }

        [IgnoreDataMember]
        public decimal Value =>  decimal.Parse(Amount);

    }
}
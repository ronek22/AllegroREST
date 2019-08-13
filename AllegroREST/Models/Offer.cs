using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class Offer
    {
        private Offer() { }

        [DataMember(Name="id")]
        public String Id { get; set; }

        [DataMember(Name="name")]
        public String Name { get; set; }

        [IgnoreDataMember]
        public String CategoryId => CategoryObject["id"];

        [DataMember(Name = "ean")]
        public String EAN { get; set; }

        [DataMember(Name = "parameters")]
        public List<dynamic> Parameters { get; set; }

        [IgnoreDataMember]
        public String Description => DescriptionObject["sections"][0]["items"][0]["content"];

        [DataMember(Name = "images")]
        public List<dynamic> Images { get; set; }

        [DataMember(Name = "sellingMode")]
        public SellingMode SellingMode { get; set; }


        #region Privates for further processing
        [DataMember(Name = "category")]
        private dynamic CategoryObject { get; set; }

        [DataMember(Name = "description")]
        private dynamic DescriptionObject { get; set; }
        #endregion
    }
}

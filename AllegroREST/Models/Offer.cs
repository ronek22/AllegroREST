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

        [DataMember(Name="id", Order = 1)]
        public string Id { get; set; }

        [DataMember(Name="name", Order = 2)]
        public string Name { get; set; }

        [IgnoreDataMember]
        public string CategoryId => _category["id"];

        [DataMember(Name = "ean", Order = 6)]
        public string EAN { get; set; }

        [DataMember(Name = "parameters", Order = 5)]
        public List<dynamic> Parameters { get; set; }

        [IgnoreDataMember]
        public string Description => _description["sections"][0]["items"][0]["content"];

        [DataMember(Name = "images", Order = 10)]
        public List<dynamic> Images { get; set; }

        [DataMember(Name = "sellingMode", Order = 11)]
        public SellingMode SellingMode { get; set; }

        [DataMember(Name="compatibilityList", Order = 8)]
        public dynamic CompatibilityList { get; set; }

        [DataMember(Name="tecdocSpecification", Order = 9)]
        public dynamic TecDocSpecification { get; set; }

        [DataMember(Name="additionalServices", Order = 17)]
        public dynamic AddidtionalServices { get; set; }

        [DataMember(Name="contact", Order = 23)]
        public dynamic Contact { get; set; }

        [DataMember(Name="createdAt", Order = 25)]
        public dynamic CreatedAt { get; set; }

        [DataMember(Name="delivery", Order = 14)]
        public dynamic Delivery { get; set; }

        [DataMember(Name="external", Order = 21)]
        public dynamic External { get; set; }

        [DataMember(Name="attachments", Order = 22)]
        public List<dynamic> Attachments { get; set; }

        [DataMember(Name="location", Order = 20)]
        public dynamic Location { get; set; }

        [DataMember(Name = "payments", Order = 15)]
        public dynamic Payments { get; set; }

        [DataMember(Name = "product", Order = 4)]
        public dynamic Product { get; set; }

        [DataMember(Name = "promotion", Order = 19)]
        public dynamic Promotion { get; set; }

        [DataMember(Name = "publication", Order = 13)]
        public dynamic Publication { get; set; }

        [DataMember(Name = "sizeTable", Order = 18)]
        public dynamic SizeTable { get; set; }

        [DataMember(Name = "stock", Order = 12)]
        public Stock Stock { get; set; }

        [DataMember(Name = "updatedAt", Order = 26)]
        public dynamic updatedAt { get; set; }

        [DataMember(Name = "validation", Order = 24)]
        public dynamic Validation { get; set; }

        [DataMember(Name="afterSalesServices", Order = 16)]
        public dynamic AfterSalesServies { get; set; }

        #region Privates for further processing
        [DataMember(Name = "category", Order = 3)]
        private dynamic _category { get; set; }

        [DataMember(Name = "description", Order = 7)]
        private dynamic _description { get; set; }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class AuthorizationData
    {
        private AuthorizationData() { }

        [DataMember(Name = "user_code")]
        public string UserCode { get; set; }

        [DataMember(Name = "device_code")]
        public string DeviceCode { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        [DataMember(Name = "interval")]
        public int Interval { get; set; }

        [DataMember(Name = "verification_uri")]
        public string VerificationUri { get; set; }

        [DataMember(Name = "verification_uri_complete")]
        public string VerificationUriComplete { get; set; }
    }
}

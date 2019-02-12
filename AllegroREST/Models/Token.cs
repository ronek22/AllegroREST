using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AllegroREST.Models
{
    [DataContract]
    [Serializable]
    public class Token
    {
        private Token() { }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "expires_in")]
        public long ExpiresIn { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "jti")]
        public string TokenId { get; set; }

        [IgnoreDataMember]
        public string AuthorizationHeader => "Bearer " + AccessToken;

        [IgnoreDataMember]
        public string RefreshHeader => "refresh_token=" + RefreshToken;

        public override string ToString()
        {
            return "TokenOauth" + "\n" + ExpiresIn + "\n" + AuthorizationHeader;
        }

    }
}

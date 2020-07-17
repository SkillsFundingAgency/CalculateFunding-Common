﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.Identity.Authentication.Models
{
    public class MsClientPrincipal
    {
        [JsonProperty("auth_typ")]
        public string AuthenticationType { get; set; }
        [JsonProperty("claims")]
        public IEnumerable<UserClaim> Claims { get; set; }
        [JsonProperty("name_typ")]
        public string NameType { get; set; }
        [JsonProperty("role_typ")]
        public string RoleType { get; set; }
    }
}

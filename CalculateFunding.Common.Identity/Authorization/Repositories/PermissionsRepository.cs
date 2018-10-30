﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authorization.Models;
using CalculateFunding.Common.Utility;
using Newtonsoft.Json;

namespace CalculateFunding.Common.Identity.Authorization.Repositories
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly HttpClient _httpClient;

        public PermissionsRepository(IHttpClientFactory httpClientFactory)
        {
            Guard.ArgumentNotNull(httpClientFactory, nameof(httpClientFactory));

            _httpClient = httpClientFactory.CreateClient(Constants.PermissionsHttpClientName);
        }

        public async Task<EffectiveSpecificationPermission> GetPermissionForUserBySpecificationId(string userId, string specificationId)
        {
            return await ExecuteHttpRequest<EffectiveSpecificationPermission>($"api/users/{userId}/effectivepermissions/{specificationId}");
        }

        public async Task<IEnumerable<FundingStreamPermission>> GetPermissionsForUser(string userId)
        {
            return await ExecuteHttpRequest<IEnumerable<FundingStreamPermission>>($"api/users/{userId}/permissions");
        }

        private async Task<T> ExecuteHttpRequest<T>(string endpoint)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();

                T permissions = JsonConvert.DeserializeObject<T>(body);
                return permissions;
            }
            else
            {
                throw new Exception($"Error calling the permissions service - {response.ReasonPhrase} ({response.StatusCode})");
            }
        }
    }
}

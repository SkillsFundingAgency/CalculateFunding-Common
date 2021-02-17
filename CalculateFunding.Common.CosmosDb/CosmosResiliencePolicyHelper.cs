using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Polly;
using Polly.Caching;
using Polly.Wrap;

namespace CalculateFunding.Common.CosmosDb
{
    public static class CosmosResiliencePolicyHelper
    {
        public static AsyncPolicy GenerateCosmosPolicy(IAsyncPolicy chainedPolicy)
        {
            return GenerateCosmosPolicy(new[] { chainedPolicy });
        }

        public static AsyncPolicy GenerateCosmosPolicyWithNoOCCRetry(IAsyncPolicy chainedPolicy)
        {
            return GenerateCosmosPolicyWithNoOCCRetry(new[]
            {
                chainedPolicy
            });
        }

        public static AsyncPolicy GenerateCosmosPolicyWithNoOCCRetry(IAsyncPolicy[] chainedPolicies = null)
        {
            AsyncPolicy documentClientExceptionRetry = Policy.Handle<CosmosException>(e => (int)e.StatusCode != 429 && (int)e.StatusCode != 412)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30) });

            return GenerateCosmosPolicyInternal(documentClientExceptionRetry, chainedPolicies);  
        } 

        public static AsyncPolicy GenerateCosmosPolicy(IAsyncPolicy[] chainedPolicies = null)
        {
            AsyncPolicy documentClientExceptionRetry = Policy.Handle<CosmosException>(e => (int)e.StatusCode != 429)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30) });

            return GenerateCosmosPolicyInternal(documentClientExceptionRetry, chainedPolicies);
        }

        private static AsyncPolicy GenerateCosmosPolicyInternal(AsyncPolicy documentClientExceptionRetry,
            IAsyncPolicy[] chainedPolicies = null)
        {
            AsyncPolicy requestRateTooLargeExceptionRetry = Policy.Handle<CosmosException>(e => (int)e.StatusCode == 429)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(120) });

            AsyncPolicy operationInProgressExceptionRetry = Policy.Handle<CosmosException>(e => (int)e.StatusCode == 423)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60) });

            AsyncPolicy circuitBreaker = Policy.Handle<CosmosException>().CircuitBreakerAsync(1000, TimeSpan.FromMinutes(1));

            List<IAsyncPolicy> policies = new List<IAsyncPolicy>(8)
            {
                documentClientExceptionRetry,
                requestRateTooLargeExceptionRetry,
                operationInProgressExceptionRetry,
                circuitBreaker,
            };

            if (chainedPolicies != null && chainedPolicies.Any())
            {
                policies.AddRange(chainedPolicies);
            }

            AsyncPolicyWrap policyWrap = Policy.WrapAsync(policies.ToArray());

            return policyWrap;   
        }

        public static Policy GenerateNonAsyncCosmosPolicy(Policy chainedPolicy)
        {
            return GenerateNonAsyncCosmosPolicy(new[] { chainedPolicy });
        }

        public static Policy GenerateNonAsyncCosmosPolicy(Policy[] chainedPolicies = null)
        {
            Policy documentClientExceptionRetry = Policy.Handle<CosmosException>(e => (int)e.StatusCode != 429)
                .WaitAndRetry(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30) });

            Policy requestRateTooLargeExceptionRetry = Policy.Handle<CosmosException>(e => (int)e.StatusCode == 429)
                .WaitAndRetry(new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(120) });

            Policy opertionInProgressExceptionRetry = Policy.Handle<CosmosException>(e => (int)e.StatusCode == 423)
                .WaitAndRetry(new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60) });

            Policy circuitBreaker = Policy.Handle<CosmosException>().CircuitBreaker(1000, TimeSpan.FromMinutes(1));

            List<Policy> policies = new List<Policy>(8)
            {
                documentClientExceptionRetry,
                requestRateTooLargeExceptionRetry,
                opertionInProgressExceptionRetry,
                circuitBreaker,
            };

            if (chainedPolicies != null && chainedPolicies.Any())
            {
                policies.AddRange(chainedPolicies);
            }

            PolicyWrap policyWrap = Policy.Wrap(policies.ToArray());

            return policyWrap;
        }
    }
}

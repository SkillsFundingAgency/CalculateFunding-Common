using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CalculateFunding.Common.Sql.Interfaces;
using Polly;

namespace CalculateFunding.Common.Sql
{
    public class SqlPolicyFactory : ISqlPolicyFactory
    {
        private static readonly HashSet<int> TransientErrorCodes = new HashSet<int>
        {
            40197,
            40501,
            40613,
            49918,
            49919,
            49920,
            4221
        };

        public Policy CreateConnectionOpenPolicy()
        {
            Policy circuitBreaker = Policy.Handle<SqlException>().CircuitBreaker(1000, DurationMinutes(1));
            Policy cannotOpenDatabase = Policy.Handle<SqlException>(_ => _.ErrorCode == 4060)
                .WaitAndRetry(retryCount: 3, ExponentialBackOff);
            Policy timeoutExpired = Policy.Handle<SqlException>(_ => _.Number == -2)
                .WaitAndRetry(retryCount: 3, ExponentialBackOff);
            
            return Policy.Wrap(cannotOpenDatabase, timeoutExpired, circuitBreaker);
        }

        public AsyncPolicy CreateQueryAsyncPolicy()
        {
            AsyncPolicy circuitBreaker = Policy.Handle<SqlException>().CircuitBreakerAsync(1000, DurationMinutes(1));
            AsyncPolicy tooBusy = Policy.Handle<SqlException>(_ => _.ErrorCode == 111)
                .WaitAndRetryAsync(retryCount: 3, _ => DurationSeconds(10));
            AsyncPolicy transientError = Policy.Handle<SqlException>(_ => IsTransientError(_.ErrorCode))
                .WaitAndRetryAsync(retryCount: 3, ExponentialBackOff);
            
            return Policy.WrapAsync(tooBusy, transientError, circuitBreaker);
        }
        
        public Policy CreateExecutePolicy()
        {
            Policy circuitBreaker = Policy.Handle<SqlException>().CircuitBreaker(1000, DurationMinutes(1));
            Policy tooBusy = Policy.Handle<SqlException>(_ => _.ErrorCode == 111)
                .WaitAndRetry(retryCount: 3, _ => DurationSeconds(10));
            Policy transientError = Policy.Handle<SqlException>(_ => IsTransientError(_.ErrorCode))
                .WaitAndRetry(retryCount: 3, ExponentialBackOff);
            
            return Policy.Wrap(tooBusy, transientError, circuitBreaker);
        }

        private static bool IsTransientError(int errorCode) => TransientErrorCodes.Contains(errorCode);

        private TimeSpan DurationMinutes(int minutes) => TimeSpan.FromMinutes(minutes);
        
        private TimeSpan DurationSeconds(int seconds) => TimeSpan.FromSeconds(seconds);

        private TimeSpan ExponentialBackOff(int attempt) => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1));
    }
}
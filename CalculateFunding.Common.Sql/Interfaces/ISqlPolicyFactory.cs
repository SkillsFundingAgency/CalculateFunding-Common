using Polly;

namespace CalculateFunding.Common.Sql.Interfaces
{
    public interface ISqlPolicyFactory
    {
        Policy CreateConnectionOpenPolicy();
        
        AsyncPolicy CreateQueryAsyncPolicy();
        
        Policy CreateExecutePolicy();
    }
}
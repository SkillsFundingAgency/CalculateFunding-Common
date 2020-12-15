using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly.Wrap;

namespace CalculateFunding.Common.Sql.UnitTests
{
    [TestClass]
    public class SqlPolicyFactoryTests
    {
        private SqlPolicyFactory _factory;

        [TestInitialize]
        public void SetUp()
        {
            _factory = new SqlPolicyFactory();
        }
        
        [TestMethod]
        public void CreatesAsyncPolicyForQuery()
        {
            _factory.CreateQueryAsyncPolicy()
                .Should()
                .BeOfType<AsyncPolicyWrap>();
        }

        [TestMethod]
        public void CreatesPolicyForOpenConnection()
        {
            _factory.CreateConnectionOpenPolicy()
                .Should()
                .BeOfType<PolicyWrap>();
        }
        
        [TestMethod]
        public void CreatesPolicyForExecute()
        {
            _factory.CreateExecutePolicy()
                .Should()
                .BeOfType<PolicyWrap>();
        }
    }
}
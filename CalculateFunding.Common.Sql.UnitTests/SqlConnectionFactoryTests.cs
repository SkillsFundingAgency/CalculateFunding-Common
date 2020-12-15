using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using CalculateFunding.Common.Sql.Interfaces;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Polly;

namespace CalculateFunding.Common.Sql.UnitTests
{
    [TestClass]
    public class SqlConnectionFactoryTests
    {
        private Mock<ISqlSettings> _settings;
        private SqlConnectionFactory _connectionFactory;
        private string _expectedConnectionString;

        [TestInitialize]
        public void SetUp()
        {
            _settings = new Mock<ISqlSettings>();
            
            _expectedConnectionString = NewRandomConnectionString();
            
            _settings.Setup(_ => _.ConnectionString)
                .Returns(_expectedConnectionString);
            
            _connectionFactory = new SqlConnectionFactory(_settings.Object);
        }
        
        [TestMethod]
        public void CreatesSqlConnectionsWithDetailsFromSettings()
        {
            using IDbConnection connection = WhenTheConnectionIsCreated();

            connection
                .Should()
                .BeOfType<SqlConnection>();

            connection
                .ConnectionString
                .Should()
                .Be(_expectedConnectionString);
        }

        private IDbConnection WhenTheConnectionIsCreated()
            => _connectionFactory.CreateConnection();

        private string NewRandomConnectionString() 
            => $"Data Source={NewRandomString()}.database.windows.net; User Id={NewRandomString()};Password={NewRandomString()};Initial Catalog={NewRandomString()}";
        
        private string NewRandomString() => new RandomString();
    }
}
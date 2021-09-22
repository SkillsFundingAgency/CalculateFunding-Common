﻿using CalculateFunding.Common.Sql.Interfaces;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using Moq.Protected;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Sql.UnitTests
{
    [TestClass]
    public class SqlRepositoryTests
    {
        private Mock<ISqlConnectionFactory> _connectionFactory;
        private Mock<IDbConnection> _connection;
        private Mock<DbTransaction> _transaction;
        private Mock<SqlTransaction> _sqlTransaction;
        private Mock<DbCommand> _command;
        private Mock<ISqlPolicyFactory> _sqlPolicyFactory;

        [TestInitialize]
        public void SetUp()
        {
            _connectionFactory = new Mock<ISqlConnectionFactory>();
            _connection = new Mock<IDbConnection>();
            _command = new Mock<DbCommand>();
            _transaction = new Mock<DbTransaction>();
            _sqlTransaction = new Mock<SqlTransaction>(_connection.Object);
            _sqlPolicyFactory = new Mock<ISqlPolicyFactory>();
            _sqlPolicyFactory.Setup(_ => _.CreateConnectionOpenPolicy()).Returns(Policy.NoOp);
        }

        [TestMethod]
        [DataRow(true, false, false)]
        [DataRow(false, false, false)]
        [DataRow(true, false, true)]
        [DataRow(false, false, true)]
        [DataRow(false, true, true)]
        public async Task InsertsEntity(bool supplyTransactions, bool rollback, bool useBulkOperation)
        {
            int identity = rollback ? - 1 : 1;

            SqlRepositoryTest sqlRepository = new SqlRepositoryTest(_connectionFactory.Object,
                _sqlPolicyFactory.Object);

            TestEntity testEntity = new TestEntity { name = "TestEntity" };
            TestEntity expectedEntity  = new TestEntity { id = identity, name = "TestEntity" };

            SetupCommandAsync(_connection);

            _command.Protected()
                           .Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", ItExpr.IsAny<CommandBehavior>(), ItExpr.IsAny<CancellationToken>())
                           .ReturnsAsync(() => (new[] { expectedEntity }).ToDataTable(typeof(TestEntity)).ToDataTableReader());
            
            IEnumerable<TestEntity> entities = null;
            int retValue = 0;

            if (supplyTransactions)
            {
                if (useBulkOperation)
                {
                    entities = await sqlRepository.InsertAll(new[] { testEntity }, _sqlTransaction.Object);
                }
                else
                {
                    retValue = await sqlRepository.InsertOne(testEntity, _sqlTransaction.Object);
                }

            }
            else
            {
                if (useBulkOperation)
                {
                    entities = await sqlRepository.InsertAll(new[] { testEntity });
                }
                else
                {
                    retValue = await sqlRepository.InsertOne(testEntity);
                }
            }

            if (rollback)
            {
                _transaction.Verify(_ => _.Rollback(), Times.Once);
            }

            if (useBulkOperation)
            {
                entities
                    .First()
                    .id
                    .Should()
                    .Be(identity);
            }
            else
            {
                retValue
                    .Should()
                    .Be(identity);
            }
        }

        [TestMethod]
        [DataRow(true, false, false)]
        [DataRow(false, false, false)]
        [DataRow(true, false, true)]
        [DataRow(false, false, true)]
        [DataRow(false, true, true)]
        public async Task DeletesEntity(bool supplyTransactions, bool rollback, bool useBulkOperation)
        {
            SqlRepositoryTest sqlRepository = new SqlRepositoryTest(_connectionFactory.Object,
                _sqlPolicyFactory.Object);

            TestEntity testEntity = new TestEntity { name = "TestEntity" };

            SetupCommandAsync(_connection);

            _command.Setup(_ => _.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(rollback ? 0 : 1);

            bool success;

            if (supplyTransactions)
            {
                success = useBulkOperation ? await sqlRepository.DeleteAll(new[] { testEntity }, _sqlTransaction.Object) : await sqlRepository.DeleteOne(testEntity, _sqlTransaction.Object);
            }
            else
            {
                success = useBulkOperation ? await sqlRepository.DeleteAll(new[] { testEntity }) : await sqlRepository.DeleteOne(testEntity);
            }

            if (rollback)
            {
                _transaction.Verify(_ => _.Rollback(), Times.Once);
            }

            success
                .Should()
                .Be(rollback ? false : true);
        }

        [TestMethod]
        [DataRow(true, false, false)]
        [DataRow(false, false, false)]
        [DataRow(true, false, true)]
        [DataRow(false, false, true)]
        [DataRow(false, true, true)]
        public async Task UpdatesEntity(bool supplyTransactions, bool rollback, bool useBulkOperation)
        {
            SqlRepositoryTest sqlRepository = new SqlRepositoryTest(_connectionFactory.Object,
                _sqlPolicyFactory.Object);

            TestEntity testEntity = new TestEntity { name = "TestEntity" };

            SetupCommandAsync(_connection);

            _command.Setup(_ => _.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(rollback ? 0 : 1);

            bool success;

            if (supplyTransactions)
            {
                success = useBulkOperation ? await sqlRepository.UpdateAll(new[] { testEntity }, _sqlTransaction.Object) : await sqlRepository.UpdateOne(testEntity, _sqlTransaction.Object);
            }
            else
            {
                success = useBulkOperation ? await sqlRepository.UpdateAll(new[] { testEntity }) : await sqlRepository.UpdateOne(testEntity);
            }

            if (rollback)
            {
                _transaction.Verify(_ => _.Rollback(), Times.Once);
            }

            success
                .Should()
                .Be(rollback ? false: true);
        }

        public void SetupCommandAsync<TConnection>(Mock<TConnection> mock)
            where TConnection : class, IDbConnection
        {
            _command.SetupAllProperties();

            _command.Protected()
                       .SetupGet<DbParameterCollection>("DbParameterCollection")
                       .Returns(new Mock<DbParameterCollection>().Object);

            _command.Protected()
                       .Setup<DbParameter>("CreateDbParameter")
                       .Returns(new Mock<DbParameter>().Object);

            _command.Protected()
                       .Setup<DbTransaction>("DbTransaction")
                       .Returns(_transaction.Object);

            var iDbConnectionMock = mock.As<IDbConnection>();

            iDbConnectionMock.Setup(m => m.CreateCommand())
                             .Returns(_command.Object);

            iDbConnectionMock.SetupGet(m => m.State)
                             .Returns(ConnectionState.Open);

            iDbConnectionMock.Setup(m => m.BeginTransaction())
                            .Returns(_transaction.Object);

            _command.Protected()
                    .Setup<IDbConnection>("DbConnection")
                       .Returns(iDbConnectionMock.Object);

            _connectionFactory.Setup(m => m.CreateConnection())
                            .Returns(iDbConnectionMock.Object);

            _sqlTransaction.Protected()
                .SetupGet<IDbConnection>("InternalConnection")
                .Returns(iDbConnectionMock.Object);

            _sqlTransaction.Protected()
                .SetupGet<IDbTransaction>("InternalTransaction")
                .Returns(_transaction.Object);
        }
    }
}

using CalculateFunding.Common.Sql.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Sql.UnitTests
{
    public class SqlRepositoryTest : SqlRepository
    {
        public SqlRepositoryTest(ISqlConnectionFactory connectionFactory,
            ISqlPolicyFactory sqlPolicyFactory) : base(connectionFactory, sqlPolicyFactory)
        {

        }

        public async Task<IEnumerable<TestEntity>> InsertAll(IEnumerable<TestEntity> testEntities)
        {
            await BulkInsert(testEntities.ToList());

            return testEntities;
        }

        public async Task<IEnumerable<TestEntity>> InsertAll(IEnumerable<TestEntity> testEntities, IDbTransaction transaction, IDbConnection connection)
        {
            await BulkInsert(testEntities.ToList(), connection, transaction);

            return testEntities;
        }

        public async Task<bool> UpdateAll(IEnumerable<TestEntity> testEntities)
        {
            return await BulkUpdate(testEntities.ToList());
        }

        public async Task<bool> UpdateAll(IEnumerable<TestEntity> testEntities, IDbTransaction transaction, IDbConnection connection)
        {
            return await BulkUpdate(testEntities.ToList(), connection, transaction);
        }

        public async Task<bool> DeleteAll(IEnumerable<TestEntity> testEntities)
        {
            return await BulkDelete(testEntities.ToList());
        }

        public async Task<bool> DeleteAll(IEnumerable<TestEntity> testEntities, IDbTransaction transaction, IDbConnection connection)
        {
            return await BulkDelete(testEntities.ToList(), connection, transaction);
        }
    }
}

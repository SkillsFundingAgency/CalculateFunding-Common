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

        public async Task<IEnumerable<TestEntity>> InsertAll(IEnumerable<TestEntity> testEntities, ISqlTransaction transaction)
        {
            await BulkInsert(testEntities.ToList(), transaction);

            return testEntities;
        }

        public async Task<int> InsertOne(TestEntity testEnty, ISqlTransaction transaction = null)
        {
            return await Insert(testEnty, transaction);
        }

        public async Task<bool> UpdateAll(IEnumerable<TestEntity> testEntities)
        {
            return await BulkUpdate(testEntities.ToList());
        }

        public async Task<bool> UpdateAll(IEnumerable<TestEntity> testEntities, ISqlTransaction transaction)
        {
            return await BulkUpdate(testEntities.ToList(), transaction);
        }
        public async Task<bool> UpdateOne(TestEntity testEnty, ISqlTransaction transaction = null)
        {
            return await Update(testEnty, transaction);
        }

        public async Task<bool> DeleteAll(IEnumerable<TestEntity> testEntities)
        {
            return await BulkDelete(testEntities.ToList());
        }

        public async Task<bool> DeleteAll(IEnumerable<TestEntity> testEntities, ISqlTransaction transaction)
        {
            return await BulkDelete(testEntities.ToList(), transaction);
        }

        public async Task<bool> DeleteOne(TestEntity testEnty, ISqlTransaction transaction = null)
        {
            return await Delete(testEnty, transaction);
        }
    }
}

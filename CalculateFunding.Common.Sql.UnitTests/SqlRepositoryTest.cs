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

        public async Task<IEnumerable<TestEntity>> InsertAll(IEnumerable<TestEntity> testEntities, bool transaction = false)
        {
            if (transaction)
            {
                await BulkInsert(testEntities.ToList(), BeginTransaction());
            }
            else
            {
                await BulkInsert(testEntities.ToList());
            }

            return testEntities;
        }

        public async Task<int> InsertOne(TestEntity testEnty, bool transaction = false)
        {
            if (transaction)
            {
                return await Insert(testEnty, BeginTransaction());
            }
            else
            {
                return await Insert(testEnty);
            }
        }

        public async Task<bool> UpdateAll(IEnumerable<TestEntity> testEntities, bool transaction = false)
        {
            if (transaction)
            {
                return await BulkUpdate(testEntities.ToList(), BeginTransaction());
            }
            else
            {
                return await BulkUpdate(testEntities.ToList());
            }
        }

        public async Task<bool> UpdateOne(TestEntity testEnty, bool transaction = false)
        {
            if (transaction)
            {
                return await Update(testEnty, BeginTransaction());
            }
            else
            {
                return await Update(testEnty);
            }
        }

        public async Task<bool> DeleteAll(IEnumerable<TestEntity> testEntities, bool transaction = false)
        {
            if (transaction)
            {
                return await BulkDelete(testEntities.ToList(), BeginTransaction());
            }
            else
            {
                return await BulkDelete(testEntities.ToList());
            }
        }

        public async Task<bool> DeleteOne(TestEntity testEnty, bool transaction = false)
        {
            if (transaction)
            {
                return await Delete(testEnty, BeginTransaction());
            }
            else
            {
                return await Delete(testEnty);
            }
        }
    }
}

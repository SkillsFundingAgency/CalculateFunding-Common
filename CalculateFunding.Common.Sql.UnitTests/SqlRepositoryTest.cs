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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CalculateFunding.Common.Sql.UnitTests
{
    [Table("TestEntities")]
    public class TestEntity
    {

        [Dapper.Contrib.Extensions.Key]
        public int id { get; set; }

        public string name { get; set; }
    }
}

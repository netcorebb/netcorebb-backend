/**
 * This file is part of the NetCoreBB forum software package.
 * @license GNU General Public License, version 3 (GNU GPLv3)
 * @copyright © 2019 Roman Volkov
 */

using System.Threading.Tasks;
using Dapper;
using LanguageExt;
using NetCoreBB.Persistence;
using NetCoreBB.Persistence.Interfaces;
using ServiceStack;
using Shouldly;
using Tomlyn;
using Tomlyn.Model;
using Xunit;
using Xunit.Abstractions;
using MyS = NetCoreBB.Domain.Model.SystemConfig.MySql;

namespace NetCoreBB.UnitTests.Persistence
{
    public partial class TableTests
    {
        [Fact]
        public void Name_is_correct()
        {
            Table.Name.ShouldBe("Test");
        }

        [Fact]
        public void Exists_is_true()
        {
            ((TestTable)Table).CreateTable();
            Table.Exists().ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsAsync_is_true()
        {
            ((TestTable)Table).CreateTable();
            (await Table.ExistsAsync()).ShouldBeTrue();
        }

        [Fact]
        public void Exists_is_false_without_create()
        {
            Table.Exists().ShouldBeFalse();
        }

        [Fact]
        public async Task ExistsAsync_is_false_without_create()
        {
            (await Table.ExistsAsync()).ShouldBeFalse();
        }


        // --- Setup ---

        public TableTests(ITestOutputHelper helper)
        {
            Output = helper;
        }

        private ITestOutputHelper Output { get; }
        private ITable<TestEntity> Table { get; } = new TestTable();

        private static string GetConnectionString()
        {
            var toml = Toml.Parse("../../../etc/unit_tests.toml".ReadAllText());
            toml.HasErrors.ShouldBeFalse();
            return (string)((TomlTable)toml.ToModel()["MySQL"])["ConnectionString"];
        }

        internal class TestEntity : Record<TestEntity>
        {
            public string? Str { get; set; }
            public int Int { get; set; }
            public long Lng { get; set; }
            public double Dbl { get; set; }
        }

        internal class TestTable : TableBase<TestEntity>
        {
            public override string Name => "Test";

            public TestTable() : base(GetConnectionString(), "prefix_")
            {
                Use(db => db.Execute("DROP TABLE IF EXISTS `prefix_test`"));
            }

            public void CreateTable()
            {
                Use(db => db.Execute(@"CREATE TABLE IF NOT EXISTS `prefix_test`(
                    `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT, PRIMARY KEY (`id`))
                    ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_unicode_ci"));
            }
        }
    }
}
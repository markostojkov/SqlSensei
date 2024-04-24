using SqlSensei.Api.Insights;
using SqlSensei.Api.Storage;

namespace SqlSensei.Api.Test
{
    public class SqlServerIndexIssuesTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetSqlServerChecks_ReturnsCorrectIndexRecommendations()
        {
            var indexUsage = new List<MonitoringJobIndexUsageLog>
            {
                new(0, 0, "TestDB", false, "Index2", "TestTable", "Column1 {int 4}, Column2 {datetime 8} : IncludeColumn1 {int 4}"                          , "Whatever", 10, 0),
                new(0, 0, "TestDB", false, "Index1", "TestTable", "Column1 {int 4} : IncludeColumn1 {int 4}"                                                , "Whatever", 10, 0),
                new(0, 0, "TestDB", false, "Index3", "TestTable", "Column1 {int 4}, Column2 {datetime 8}, Column3 {varchar 8} : IncludeColumn1 {int 4}"     , "Whatever", 15, 0),

                new(0, 0, "TestDB", false, "Index4", "TestTable", "Column4 {varchar 100} : IncludeColumn3 {int 4}"                                          , "Whatever", 20, 0),
                new(0, 0, "TestDB", false, "Index5", "TestTable", "Column5 {int 4} :"                                                                       , "Whatever", 25, 0),
                new(0, 0, "TestDB", false, "Index6", "TestTable", "Column6 {varchar 200} :"                                                                 , "Whatever", 30, 0),
                new(0, 0, "TestDB", false, "Index7", "TestTable", "Column7 {datetime 8} : IncludeColumn4 {int 4}"                                           , "Whatever", 35, 0),
                new(0, 0, "TestDB", false, "Index8", "TestTable", "Column8 {int 4} :"                                                                       , "Whatever", 40, 0),
                new(0, 0, "TestDB", false, "Index9", "TestTable", "Column9 {datetime 8} : IncludeColumn5 {varchar 100}"                                     , "Whatever", 0, 0),
            };

            var missingIndexes = new List<MonitoringJobIndexMissingLog>
            {
                new(0, 0, "TestDB", "TestTable", 95, "High", "Column9 {datetime 8} : IncludeColumn5 {varchar 100}")
            };

            var result = SqlServerIndexIssues.GetSqlServerChecks(indexUsage, missingIndexes);

            Assert.IsTrue(result.RemoveIndices.Any(x => x.Index == "Index1" && x.Message.Contains(SqlServerIndexIssues.IndexIsSmallerSubset)));
            Assert.IsTrue(result.RemoveIndices.Any(x => x.Index == "Index2" && x.Message.Contains(SqlServerIndexIssues.IndexIsSmallerSubset)));
            Assert.IsTrue(result.RemoveIndices.Any(x => x.Index == "Index9" && x.Message.Contains(SqlServerIndexIssues.IndexNotUsedMessage)));

            Assert.IsTrue(result.AddIndices.Any(x => x.MagicBenefit == 95 && x.IndexDetails == "Column9 {datetime 8} : IncludeColumn5 {varchar 100}"));
        }
    }
}

namespace SqlSensei.Core.Tests
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public class IndexLogicTests
    {
        [Fact]
        public void GetIndexesWithIssues_NoIndexes_ReturnsEmptyList()
        {
            // Arrange
            var indexLogUsages = new List<IMonitoringJobIndexUsageLog>();

            // Act
            var result = indexLogUsages.GetIndexesWithIssues();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetIndexesWithIssues_IndexWithZeroReads_ReturnsIndexWithIssue()
        {
            // Arrange
            var indexLogUsages = new List<IMonitoringJobIndexUsageLog>
            {
                new TestIndexLogUsage("Db1", "Table1_Index2", "Table1", "Id {int 4} : ",                                                                   "Reads: 0 Writes: 8",    0, 8,   true),
                new TestIndexLogUsage("Db1", "Table1_Index7", "Table1", "CustomerFK {int 4} : ",                                                           "Reads: 10 Writes: 8",   10, 8,  false),
                new TestIndexLogUsage("Db1", "Table1_Index1", "Table1", "CustomerFK {int 4}, SerialNumber {nvarchar (64)} : ",                             "Reads: 10 Writes: 8",   10, 8,  false),
                new TestIndexLogUsage("Db1", "Table1_Index3", "Table1", "CustomerFK {int 4} : ProductName {nvarchar (255)}",                               "Reads: 10 Writes: 8",   10, 8,  false),
                new TestIndexLogUsage("Db1", "Table1_Index5", "Table1", "UserFk {int 4} : ",                                                               "Reads: 0 Writes: 8",    0, 8,   false),
                new TestIndexLogUsage("Db1", "Table1_Index3", "Table2", "CustomerFK {int 4} : ProductName {nvarchar (255)}",                               "Reads: 10 Writes: 8",   10, 8,  false)
            };

            // Act
            var result = indexLogUsages.GetIndexesWithIssues();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(IndexLogic.IndexNotUsedMessage, result[0].UserMessage);
            Assert.Contains(IndexLogic.IndexIsSmallerSubset, result[1].UserMessage);
        }
    }

#pragma warning restore CA1707 // Identifiers should not contain underscores

    public class TestIndexLogUsage : IMonitoringJobIndexUsageLog
    {
        public TestIndexLogUsage(string databaseName, string indexName, string tableName, string indexDetails, string usage, long readsUsage, long writeUsage, bool isClusteredIndex)
        {
            DatabaseName = databaseName;
            IndexName = indexName;
            TableName = tableName;
            IndexDetails = indexDetails;
            Usage = usage;
            ReadsUsage = readsUsage;
            WriteUsage = writeUsage;
            IsClusteredIndex = isClusteredIndex;
            UserMessage = string.Empty;
        }

        public string DatabaseName { get; }

        public string IndexName { get; }

        public string TableName { get; }

        public string IndexDetails { get; }

        public string Usage { get; }

        public long ReadsUsage { get; }

        public long WriteUsage { get; }
        public bool IsClusteredIndex { get; }
        public string UserMessage { get; set; }

        public List<string> IndexColumns { get; set; } = new List<string>();
        public List<string> IndexIncludeColumns { get; set; } = new List<string>();
    }

}
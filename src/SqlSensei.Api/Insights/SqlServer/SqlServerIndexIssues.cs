using SqlSensei.Api.Storage;

namespace SqlSensei.Api.Insights
{
    public class SqlServerRemoveIndex(string databaseName, string tableName, string index, string message, string shortMessage)
    {
        public string DatabaseName { get; } = databaseName;
        public string TableName { get; } = tableName;
        public string Index { get; } = index;
        public string Message { get; } = message;
        public string ShortMessage { get; } = shortMessage;
    }

    public class SqlServerAddIndex(string databaseName, string tableName, long magicBenefit, string impact, string indexDetails)
    {
        public string DatabaseName { get; } = databaseName;
        public string TableName { get; } = tableName;
        public long MagicBenefit { get; } = magicBenefit;
        public string Impact { get; } = impact;
        public string IndexDetails { get; } = indexDetails;
    }

    public class SqlServerIndexCheck(IEnumerable<SqlServerRemoveIndex> removeIndices, IEnumerable<SqlServerAddIndex> addIndices)
    {
        public IEnumerable<SqlServerRemoveIndex> RemoveIndices { get; } = removeIndices;
        public IEnumerable<SqlServerAddIndex> AddIndices { get; } = addIndices;
    }

    public static class SqlServerIndexIssues
    {
        public const string IndexNotUsedMessage = "This index has 0 reads meaning it only ocuppies space and makes updates more complex, keep in mind that" +
            "your database might have been restarted recently and the statistics are incorrect, if that's not the case then it can be deleted";
        public const string IndexNotUsedMessageShort = "0 reads";

        public const string IndexIsSmallerSubset = "This index is a smaller subset of another index in the same table and can be easily deleted";
        public const string IndexIsSmallerSubsetShort = "Smaller subset";

        public static SqlServerIndexCheck GetSqlServerChecks(IEnumerable<MonitoringJobIndexUsageLog> indexUsage, IEnumerable<MonitoringJobIndexMissingLog> missingIndexes)
        {
            var indexesWithIssue = new List<SqlServerRemoveIndex>();

            var notUsedIndexes = indexUsage
                .Where(x => x.IsClusteredIndex == false)
                .Where(x => x.ReadsUsage == 0)
                .Select(x => new SqlServerRemoveIndex(x.DatabaseName, x.TableName, x.IndexName, IndexNotUsedMessage, IndexNotUsedMessageShort))
                .GroupBy(x => new {x.DatabaseName, x.TableName, x.Index})
                .Select(x => x.First())
                .ToList();

            indexesWithIssue.AddRange(notUsedIndexes);

            var smallerSubsetIndexes = indexUsage
                .Where(x => x.ReadsUsage > 0)
                .Select(x =>
                {
                    var splitIndexColumnsAndIncludeIndexColumns = x.IndexDetails.Replace("DESC", string.Empty).Replace("ASC", string.Empty).Split(':');

                    x.IndexColumns = splitIndexColumnsAndIncludeIndexColumns[0].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    x.IndexIncludeColumns = splitIndexColumnsAndIncludeIndexColumns[1]?.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();

                    return x;
                })
                .GroupBy(x => new { x.DatabaseName, x.TableName });

            foreach (var indexesOnTableInDatabase in smallerSubsetIndexes)
            {
                foreach (var (indexBeingCheckedIfDuplicate, i) in indexesOnTableInDatabase.OrderByDescending(x => x.IndexColumns.Count + x.IndexIncludeColumns.Count).WithIndex())
                {
                    var indexThatCovers = string.Empty;
                    var areAllColumnsOfCurrentIndexContainedInAnotherIndex = indexesOnTableInDatabase.WithIndex().Any(indexUsedForCheckingDuplicate =>
                    {
                        if (!ReferenceEquals(indexUsedForCheckingDuplicate.item, indexBeingCheckedIfDuplicate) 
                            && !indexBeingCheckedIfDuplicate.IsClusteredIndex && indexUsedForCheckingDuplicate.item.IndexName != indexBeingCheckedIfDuplicate.IndexName)
                        {
                            var doesThisIndexContainAllIndexColumnsOfCurrentCheckIndex = false;
                            var doesThisIndexContainAllIndexIncludeColumnsOfCurrentCheckIndex = false;

                            if (indexBeingCheckedIfDuplicate.IndexColumns.Count <= indexUsedForCheckingDuplicate.item.IndexColumns.Count)
                            {
                                doesThisIndexContainAllIndexColumnsOfCurrentCheckIndex = indexUsedForCheckingDuplicate.item.IndexColumns.Take(indexBeingCheckedIfDuplicate.IndexColumns.Count).SequenceEqual(indexBeingCheckedIfDuplicate.IndexColumns);
                            }

                            if (indexBeingCheckedIfDuplicate.IndexIncludeColumns.Count <= indexUsedForCheckingDuplicate.item.IndexIncludeColumns.Count)
                            {
                                doesThisIndexContainAllIndexIncludeColumnsOfCurrentCheckIndex = indexUsedForCheckingDuplicate.item.IndexIncludeColumns.Take(indexBeingCheckedIfDuplicate.IndexIncludeColumns.Count).SequenceEqual(indexBeingCheckedIfDuplicate.IndexIncludeColumns);
                            }

                            if (doesThisIndexContainAllIndexColumnsOfCurrentCheckIndex && doesThisIndexContainAllIndexIncludeColumnsOfCurrentCheckIndex)
                            {
                                indexThatCovers = indexUsedForCheckingDuplicate.item.IndexName;
                            }

                            return doesThisIndexContainAllIndexColumnsOfCurrentCheckIndex && doesThisIndexContainAllIndexIncludeColumnsOfCurrentCheckIndex;
                        }

                        return false;
                    });

                    if (areAllColumnsOfCurrentIndexContainedInAnotherIndex)
                    {
                        indexesWithIssue.Add(new SqlServerRemoveIndex(
                            indexBeingCheckedIfDuplicate.DatabaseName,
                            indexBeingCheckedIfDuplicate.TableName,
                            indexBeingCheckedIfDuplicate.IndexName,
                            $"{IndexIsSmallerSubset} {indexThatCovers} Covers your index",
                            $"{IndexIsSmallerSubsetShort} {indexThatCovers} Covers your index"));
                    }
                }
            }

            var missingIndexesLog = missingIndexes
                .Select(x => new SqlServerAddIndex(x.DatabaseName, x.TableName, x.MagicBenefitNumber, x.Impact, x.IndexDetails))
                .GroupBy(x => new { x.DatabaseName, x.TableName, x.IndexDetails })
                .Select(x => x.First());

            return new SqlServerIndexCheck(indexesWithIssue, missingIndexesLog);
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
    }
}

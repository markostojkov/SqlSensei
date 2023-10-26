using System.Collections.Generic;
using System.Linq;

namespace SqlSensei.Core
{
    public static class IndexLogic
    {
        private const string IndexNotUsedMessage = "This index has 0 reads meaning it only ocuppies space and makes updates more complex, keep in mind that" +
            "your database might have been restarted recently and the statistics are incorrect, if that's not the case then it can be deleted";

        private const string IndexIsSmallerSubset = "This index is a smaller subset of another index in the same table and can be easily deleted";

        public static List<IMonitoringJobIndexLogUsage> GetIndexesWithIssues(this IEnumerable<IMonitoringJobIndexLogUsage> indexLogUsages)
        {
            var indexesWithIssue = new List<IMonitoringJobIndexLogUsage>();

            var notUsedIndexes = indexLogUsages.Where(x => x.ReadsUsage == 0).Select(x =>
            {
                x.UserMessage = IndexNotUsedMessage;

                return x;
            }).ToList();

            indexesWithIssue.AddRange(notUsedIndexes);

            var smallerSubsetIndexes = indexLogUsages
                .Where(x => x.ReadsUsage > 0)
                .Select(x =>
                {
                    var splitIndexColumsAndIncludeIndexColums = x.IndexDetails.Replace("DESC", string.Empty).Replace("ASC", string.Empty).Split(':');

                    x.IndexColumns = splitIndexColumsAndIncludeIndexColums[0].Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    x.IndexIncludeColumns = splitIndexColumsAndIncludeIndexColums[1]?.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).ToList();

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
                        if (!ReferenceEquals(indexUsedForCheckingDuplicate.item, indexBeingCheckedIfDuplicate) && !indexBeingCheckedIfDuplicate.IsClusteredIndex)
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
                        indexBeingCheckedIfDuplicate.UserMessage = $"{IndexIsSmallerSubset} {indexThatCovers} Covers your index";

                        indexesWithIssue.Add(indexBeingCheckedIfDuplicate);
                    }
                }
            }

            return indexesWithIssue.OrderBy(x => x.TableName).ToList();
        }
    }
}

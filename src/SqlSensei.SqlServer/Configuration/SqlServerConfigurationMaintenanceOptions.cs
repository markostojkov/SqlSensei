using SqlSensei.Core;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSensei.SqlServer
{
    public class SqlServerConfigurationMaintenanceOptions
    {
        private static readonly string _replaceDatabase = "replace-database";
        private readonly string _scriptExecution;
        private readonly string _script = @"
            EXECUTE dbo.IndexOptimize
            @Databases           = '" + _replaceDatabase + @"',
            @FragmentationMedium = '{0}',
            @FragmentationHigh   = '{1}',
            @FragmentationLevel1 =  {2},
            @FragmentationLevel2 =  {3},
            @UpdateStatistics    = '{4}',
            @LogToTable = 'Y'
        ";

        public string ScriptName { get; }

        private SqlServerConfigurationMaintenanceOptions(
            string scriptName,
            SqlServerConfigurationMaintenanceOptionsStatistics statistics,
            SqlServerConfigurationMaintenanceOptionsFragmentation fragmentationLow,
            SqlServerConfigurationMaintenanceOptionsFragmentation fragmentationHigh)
        {
            ScriptName = scriptName;
            _scriptExecution = string.Format(_script, fragmentationLow.IndexFunction, fragmentationHigh.IndexFunction, fragmentationLow.Fragmentation, fragmentationHigh.Fragmentation, statistics.Statistics);
        }

        public static SqlServerConfigurationMaintenanceOptions Default => new(
            "MaintenanceSolution.sql",
            SqlServerConfigurationMaintenanceOptionsStatistics.All,
            new SqlServerConfigurationMaintenanceOptionsFragmentation(5,
                SqlServerConfigurationMaintenanceOptionsIndex.Reorganize,
                SqlServerConfigurationMaintenanceOptionsIndex.RebuildOnline,
                SqlServerConfigurationMaintenanceOptionsIndex.RebuildOffline),
            new SqlServerConfigurationMaintenanceOptionsFragmentation(30,
                SqlServerConfigurationMaintenanceOptionsIndex.RebuildOnline,
                SqlServerConfigurationMaintenanceOptionsIndex.RebuildOffline,
                SqlServerConfigurationMaintenanceOptionsIndex.Reorganize));

        public static SqlServerConfigurationMaintenanceOptions Create(
            SqlServerConfigurationMaintenanceOptionsStatistics statistics,
            SqlServerConfigurationMaintenanceOptionsFragmentation fragmentation1,
            SqlServerConfigurationMaintenanceOptionsFragmentation fragmentation2)
        {
            return new("MaintenanceSolution.sql", statistics, fragmentation1, fragmentation2);
        }

        public string GetScript(IEnumerable<SqlSenseiConfigurationDatabase> databases)
        {
            return _scriptExecution.Replace(_replaceDatabase, string.Join(",", databases.Select(x => x.Database)));
        }
    }

    public class SqlServerConfigurationMaintenanceOptionsStatistics
    {
        private SqlServerConfigurationMaintenanceOptionsStatistics(string statistics)
        {
            Statistics = statistics;
        }

        /// <summary>
        /// Update index and column statistics.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsStatistics All => new("ALL");

        /// <summary>
        /// Update index statistics.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsStatistics Index => new("INDEX");

        /// <summary>
        /// Update column statistics.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsStatistics Column => new("COLUMNS");

        /// <summary>
        /// Do not perform statistics maintenance. This is the default.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsStatistics None => new("NULL");

        public string Statistics { get; }
    }

    public class SqlServerConfigurationMaintenanceOptionsIndex
    {
        private SqlServerConfigurationMaintenanceOptionsIndex(string indexFunction)
        {
            IndexFunction = indexFunction;
        }

        /// <summary>
        /// Rebuild index online.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsIndex RebuildOnline => new("INDEX_REBUILD_ONLINE");

        /// <summary>
        /// Rebuild index offline.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsIndex RebuildOffline => new("INDEX_REBUILD_OFFLINE");

        /// <summary>
        /// Reorganize index.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsIndex Reorganize => new("INDEX_REORGANIZE");

        /// <summary>
        /// Do not perform index maintenance. This is the default.
        /// </summary>
        public static SqlServerConfigurationMaintenanceOptionsIndex None => new("NULL");

        public string IndexFunction { get; }
    }

    public class SqlServerConfigurationMaintenanceOptionsFragmentation
    {
        public int Fragmentation { get; }
        public string IndexFunction { get; }

        public SqlServerConfigurationMaintenanceOptionsFragmentation(int fragmentation, params SqlServerConfigurationMaintenanceOptionsIndex[] indexFunction)
        {
            if (fragmentation <= 0 || fragmentation >= 100)
            {
                throw new ArgumentException("Fragmentation must be between 1 and 99");
            }

            if (indexFunction == null || indexFunction.Length == 0 || indexFunction.Length > 3)
            {
                throw new ArgumentException("Index must be have atleast 1 option and can't be longer than 3");
            }

            Fragmentation = fragmentation;
            IndexFunction = string.Join(",", indexFunction.Select(x => x.IndexFunction));
        }
    }
}
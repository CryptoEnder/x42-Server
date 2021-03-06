﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace X42.Utilities
{
    public interface IServerStats
    {
        /// <summary>Registers action that will be used to append server stats when they are being collected.</summary>
        /// <param name="appendStatsAction">Action that will be invoked during stats collection.</param>
        /// <param name="statsType">Type of stats.</param>
        /// <param name="priority">Stats priority that will be used to determine invocation priority of stats collection.</param>
        void RegisterStats(Action<StringBuilder> appendStatsAction, StatsType statsType, int priority = 0);

        /// <summary>Collects inline stats and then feature stats.</summary>
        string GetStats();

        /// <summary>Collects benchmark stats.</summary>
        string GetBenchmark();
    }

    public class ServerStats : IServerStats
    {
        private readonly IDateTimeProvider dateTimeProvider;

        /// <summary>Protects access to <see cref="stats" />.</summary>
        private readonly object locker;

        private List<StatsItem> stats;

        public ServerStats(IDateTimeProvider dateTimeProvider)
        {
            locker = new object();
            this.dateTimeProvider = dateTimeProvider;

            stats = new List<StatsItem>();
        }

        /// <inheritdoc />
        public void RegisterStats(Action<StringBuilder> appendStatsAction, StatsType statsType, int priority = 0)
        {
            lock (locker)
            {
                stats.Add(new StatsItem
                {
                    AppendStatsAction = appendStatsAction,
                    StatsType = statsType,
                    Priority = priority
                });

                stats = stats.OrderByDescending(x => x.Priority).ToList();
            }
        }

        /// <inheritdoc />
        public string GetStats()
        {
            StringBuilder statsBuilder = new StringBuilder();

            lock (locker)
            {
                string date = dateTimeProvider.GetUtcNow().ToString(CultureInfo.InvariantCulture);
                statsBuilder.AppendLine($"====== x42 Master Node Stats ====== {date}");

                foreach (StatsItem actionPriority in stats.Where(x => x.StatsType == StatsType.Inline))
                    actionPriority.AppendStatsAction(statsBuilder);

                foreach (StatsItem actionPriority in stats.Where(x => x.StatsType == StatsType.Component))
                    actionPriority.AppendStatsAction(statsBuilder);
            }

            return statsBuilder.ToString();
        }

        /// <inheritdoc />
        public string GetBenchmark()
        {
            StringBuilder statsBuilder = new StringBuilder();

            lock (locker)
            {
                foreach (StatsItem actionPriority in stats.Where(x => x.StatsType == StatsType.Benchmark))
                    actionPriority.AppendStatsAction(statsBuilder);
            }

            return statsBuilder.ToString();
        }

        private struct StatsItem
        {
            public StatsType StatsType;

            public Action<StringBuilder> AppendStatsAction;

            public int Priority;
        }
    }

    public enum StatsType
    {
        /// <summary>
        ///     Inline stats are usually single line stats that should
        ///     display most important information about the server.
        /// </summary>
        Inline,

        /// <summary>
        ///     Component-related stats are usually blocks of component specific stats.
        /// </summary>
        Component,

        /// <summary>
        ///     Benchmarking stats that display performance related information.
        /// </summary>
        Benchmark
    }
}
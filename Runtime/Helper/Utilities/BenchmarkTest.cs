

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Base.Logging;
using UniRx;
using UnityEngine;

namespace Base.Helper
{
    /// <summary>
    /// This class is used for test cost of the code
    /// </summary>
    public static class PBenchMark
    {
        private static readonly Lazy<IDictionary<string, PBenchmarkData>> LazyBenchmarkData = new(() => new Dictionary<string, PBenchmarkData>());
        public static void StartTest(string testTitle, bool precise = false)
        {
            if (!LazyBenchmarkData.Value.TryGetValue(testTitle, out PBenchmarkData benchmark))
            {
                benchmark = new PBenchmarkData(testTitle, precise);

                if (!LazyBenchmarkData.Value.TryAdd(benchmark.BenchMarkTitle, benchmark))
                {
                    PDebug.ErrorFormat("Cannot add benchmark name {0}", testTitle);
                    benchmark.Timer.Stop();
                    return;
                }
            }

            benchmark.Timer.Start();
        }

        public static void PauseTest(string title)
        {
            if (LazyBenchmarkData.Value.TryGetValue(title, out PBenchmarkData benchmark))
            {
                benchmark.Timer.Stop();
            }
        }

        public static void EndTest(string title)
        {
            if (LazyBenchmarkData.Value.TryGetValue(title, out PBenchmarkData benchmark))
            {
                benchmark.EndTest();
                LazyBenchmarkData.Value.Remove(title);
            }
        }
    }

    public struct PBenchmarkData
    {
        private string    m_benchMarkTitle;
        private bool      m_precise;
        private Stopwatch m_timer;
        
        public  string    BenchMarkTitle => m_benchMarkTitle;
        public  bool      Precise        => m_precise;
        public  Stopwatch Timer          => m_timer;

        private StringBuilder m_stringBuilder;

        public PBenchmarkData(string benchMarkTitle, bool precise)
        {
            m_benchMarkTitle = benchMarkTitle;
            m_precise        = precise;
            m_timer          = Stopwatch.StartNew();
            m_stringBuilder  = new StringBuilder(String.Empty);
        }

        public void EndTest()
        {
            var ms         = m_timer.ElapsedMilliseconds;
            var elapsedVal = m_precise ? ms : ms / 1000f;
            var valMark   = m_precise ? "ms" : "s";

            m_stringBuilder.Length = 0;
            m_stringBuilder.Append("Time Test <color=brown>")
                           .Append(m_benchMarkTitle)
                           .Append("</color>: ")
                           .Append(elapsedVal)
                           .Append(valMark);
            
            m_timer.Stop();

            PDebug.Info(m_stringBuilder.ToString());

            m_stringBuilder = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfObserver.Model
{
    internal class SampleStatRow
    {
        internal string ProcessName;
        internal string SampleDateTime { get; set; }
        internal double AverageTime  { get; set; }
        internal double StandartDeviation { get; set; }
        internal double MinValue { get; set; }
        internal double MaxValue { get; set; }
        internal List<SubProcessRatio> SubProcessRatio { get; set; }
    }
    internal class SubProcessRatio
    {
        internal string SubProcessName { get; set; }
        internal double MainProcessusRatio { get; set; }

    }
}

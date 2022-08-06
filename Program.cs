using PerfObserver.TEST;
using System.Reflection;

namespace PerfObserver
{
    /// <summary>
    /// entry point to execute tests
    /// </summary>
    internal static class Program
    {
        #region ACTION TEST
        private static readonly Action SIMPLY_LOG = () => Test.PerfLogger_LogProcess_1();
        private static readonly Action LOG_PROCESS = () => Test.PerfLogger_LogProcess_2();
        private static readonly Action LOG_STAT = () => Test.PerfLogger_LogSampleProcess();
        private static readonly Action CREATE_SAMPLE_XLSX = () => Test.Process_CreateSample_XlsxUtils();
        private static readonly Action CHARTS = () => Test.ChartsUtils_CreateChartFromProcess();
        private static readonly Action MANAGER = () => Test.ProcessManagerTests();
        #endregion
        private static void ExecuteTest(Action Test) => Test.Invoke();
        private static void ExecuteTests() =>

            typeof(Program).GetMembers(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.MemberType == MemberTypes.Field)
            .Select(m => (Action)typeof(Program).GetField(m.Name, BindingFlags.Static | BindingFlags.NonPublic).GetValue(typeof(Program)))
            .ToList()
            .ForEach(a=>a.Invoke());
        
            
        static void Main()
        {
            Console.WriteLine();
            ExecuteTests();
        }
    }
}
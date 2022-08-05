using PerfObserver.TEST;
using System.Reflection;

namespace PerfObserver
{
    /// <summary>
    /// Point d'entrée Temporaire du programme afin de tester ces fonctionalité en cours de developpement
    /// </summary>
    internal static class Program
    {
        private static readonly Action SIMPLY_LOG = () => Test.BasicPerfLogger_SimplyLog();
        private static readonly Action LOG_PROCESS = () => Test.BasicPerfLogger_LogProcess();
        private static readonly Action LOG_STAT = () => Test.BasicPerfLogger_LogProcessSampleStatistics();
        private static readonly Action CREATE_SAMPLE_XLSX = () => Test.Process_CreateSampleForProcessAndSubProcess_XlsxUtils();
        private static readonly Action CHARTS = () => Test.ChartsUtils_CreateChartFromProcess();
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
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
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
        static readonly Action SIMPLY_LOG = () => Test.PerfLogger_LogProcess_1();
        static readonly Action LOG_PROCESS = () => Test.PerfLogger_LogProcess_2();
        static readonly Action LOG_STAT = () => Test.PerfLogger_LogSampleProcess();
        static readonly Action CREATE_SAMPLE_XLSX = () => Test.Process_CreateSample_XlsxUtils();
        static readonly Action CHARTS = () => Test.ChartsUtils_CreateChartFromProcess();
        static readonly Action MANAGER = () => Test.ProcessManagerTests();
#pragma warning restore S1144 // Unused private types or members should be removed
#pragma warning restore IDE0052 // Supprimer les membres privés non lus
        #endregion

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
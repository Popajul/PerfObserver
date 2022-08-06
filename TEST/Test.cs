using PerfObserver.Charts;
using PerfObserver.XLSX;
using TEST.TestMethods;

namespace PerfObserver.TEST
{
    internal static class Test
    {
        private static readonly Type TARGET_TYPE_1 = typeof(Arithmetic);
        private static readonly string METHOD_NAME_1 = "IsEven";
        private static readonly string METHOD_NAME_1_2 = "LogIsEven";
        private static readonly object[] CTOR_PARAMETERS_1 = new object[] { 39 };
        private static readonly object[] NULL_CTOR_PARAMETERS = null;
        private static readonly Type[] PARAMETERS_TYPES_1 = new Type[] { typeof(string) };
        private static readonly object[] METHOD_PARAMETERS_1 = new object[] { "5" };
        private static readonly object[] METHOD_PARAMETERS_1_2 = new object[] { 5 };
        private static readonly string INVALID_METHOD_NAME = "invalid";

        private static readonly Type TARGET_TYPE_2 = typeof(FakeMethods);
        private static readonly string METHOD_NAME_2 = "FakeMethod0";
        private static readonly string METHOD_NAME_2_1 = "FakeMethod10";
        private static readonly string METHOD_NAME_2_2 = "FakeMethod11";
        private static readonly string METHOD_NAME_2_3 = "FakeMethod20";
        private static readonly int SAMPLE_SIZE = 3;

        internal static void PerfLogger_LogProcess_1()
        {
            // Test with private non static method
            PerfLogger.LogProcess(TARGET_TYPE_1, METHOD_NAME_1, CTOR_PARAMETERS_1);

            // Test with Static public Method and method's Parameters
            PerfLogger.LogProcess(TARGET_TYPE_1, METHOD_NAME_1, NULL_CTOR_PARAMETERS, PARAMETERS_TYPES_1, METHOD_PARAMETERS_1);

            // Test With invalid Method Name
            try
            {
                PerfLogger.LogProcess(TARGET_TYPE_1, INVALID_METHOD_NAME, NULL_CTOR_PARAMETERS, PARAMETERS_TYPES_1, METHOD_PARAMETERS_1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // test with invalid Parameters

            try
            {
                PerfLogger.LogProcess(TARGET_TYPE_1, METHOD_NAME_1, NULL_CTOR_PARAMETERS, PARAMETERS_TYPES_1, METHOD_PARAMETERS_1_2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Test with void return Method
            PerfLogger.LogProcess(TARGET_TYPE_1, METHOD_NAME_1_2, CTOR_PARAMETERS_1);
        }

        internal static void PerfLogger_LogProcess_2()
        {
            ConfigureTestProcess(out Process _process0);

            PerfLogger.LogProcess(_process0);
        }

        internal static void PerfLogger_LogSampleProcess()
        {
            ConfigureTestProcess(out Process _process0);
            PerfLogger.LogProcessSample(_process0, 5);
        }


        internal static void Process_CreateSample_XlsxUtils()
        {

            ConfigureTestProcess(out Process _process0);

            for (int i = 0; i < 2; i++)
                _process0.CreateSampleForProcessAndSubProcess(SAMPLE_SIZE);


            XlsxUtils.CreateProcessXLSXFile(_process0);

            _ = XlsxUtils.GetSampleStatRowsFromProcess(_process0);
        }

        internal static void ChartsUtils_CreateChartFromProcess()
        {

            ConfigureTestProcess(out Process _process0);


            for (int i = 0; i < 3; i++)
            {
                _process0.CreateSampleForProcessAndSubProcess(SAMPLE_SIZE);
                Thread.Sleep(1000);
            }
 
            XlsxUtils.CreateProcessXLSXFile(_process0);
            ChartsUtils.CreateChartsFromProcess(_process0);
        }

        internal static void ProcessManagerTests()
        {
            ProcessManager manager = new ();

            Process _process0 = manager.CreateProcess(TARGET_TYPE_2, METHOD_NAME_2);
            _ = manager.CreateSubProcess(_process0, TARGET_TYPE_2, METHOD_NAME_2_1);

            Process _process_1_2 = manager.CreateSubProcess(_process0, TARGET_TYPE_2, METHOD_NAME_2_2);
            _= manager.CreateSubProcess(_process_1_2, TARGET_TYPE_2, METHOD_NAME_2_3);

            manager.CreateSample(_process0, SAMPLE_SIZE, true);
            
            try
            {
                manager.CreateBarCharts(_process0);
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("Use \"manager.SaveSampleDataToFile\" before creating charts");
            }

            manager.SaveSampleDataToFile(_process0);
            manager.CreateCharts(_process0);

        }

        private static void ConfigureTestProcess(out Process _process0)
        {
            ProcessManager manager = new();
            _process0 = manager.CreateProcess(TARGET_TYPE_2, METHOD_NAME_2);

            _ = manager.CreateSubProcess(_process0, TARGET_TYPE_2, METHOD_NAME_2_1);

            var _process_1_1 = manager.CreateSubProcess(_process0, TARGET_TYPE_2, METHOD_NAME_2_2);

            _ = manager.CreateSubProcess(_process_1_1, TARGET_TYPE_2, METHOD_NAME_2_3);
        }
    }
}

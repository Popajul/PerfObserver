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

        internal static void BasicPerfLogger_SimplyLog()
        {
            // Test with private non static method
            BasicPerfLogger.SimplyLogPerf(TARGET_TYPE_1, METHOD_NAME_1, CTOR_PARAMETERS_1);

            // Test with Static public Method and method's Parameters
            BasicPerfLogger.SimplyLogPerf(TARGET_TYPE_1, METHOD_NAME_1, NULL_CTOR_PARAMETERS, PARAMETERS_TYPES_1, METHOD_PARAMETERS_1);

            // Test With invalid Method Name
            try
            {
                BasicPerfLogger.SimplyLogPerf(TARGET_TYPE_1, INVALID_METHOD_NAME, NULL_CTOR_PARAMETERS, PARAMETERS_TYPES_1, METHOD_PARAMETERS_1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // test with invalid Parameters

            try
            {
                BasicPerfLogger.SimplyLogPerf(TARGET_TYPE_1, METHOD_NAME_1, NULL_CTOR_PARAMETERS, PARAMETERS_TYPES_1, METHOD_PARAMETERS_1_2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Test with void return Method
            BasicPerfLogger.SimplyLogPerf(TARGET_TYPE_1, METHOD_NAME_1_2, CTOR_PARAMETERS_1);
        }

        internal static void BasicPerfLogger_LogProcess()
        {
            ProcessFactory _factory;
            Process _process0;
            Process _process_1_1;

            _factory = new(TARGET_TYPE_2, METHOD_NAME_2);
            _process0 = _factory.CreateProcess();

            _factory = new(TARGET_TYPE_2, METHOD_NAME_2_1);
            _ = _factory.CreateProcess(_process0);

            _factory = new(TARGET_TYPE_2, METHOD_NAME_2_2);
            _process_1_1 = _factory.CreateProcess(_process0);

            _factory = new(TARGET_TYPE_2, METHOD_NAME_2_3);
            _ = _factory.CreateProcess(_process_1_1);

            BasicPerfLogger.LogProcessPerf(_process0);
        }

        internal static void BasicPerfLogger_LogProcessSampleStatistics()
        {
            ProcessFactory _factory;
            Process _process0;
            Process _process_1_1;
            ConfigureTestProcess(out _factory, out _process0, out _process_1_1);
            BasicPerfLogger.LogProcessSampleStatistics(_process0, 5);
        }

        private static void ConfigureTestProcess(out ProcessFactory _factory, out Process _process0, out Process _process_1_1)
        {
            _factory = new(TARGET_TYPE_2, METHOD_NAME_2);
            _process0 = _factory.CreateProcess();

            _factory = new(TARGET_TYPE_2, METHOD_NAME_2_1);
            _ = _factory.CreateProcess(_process0);

            _factory = new(TARGET_TYPE_2, METHOD_NAME_2_2);
            _process_1_1 = _factory.CreateProcess(_process0);

            _factory = new(TARGET_TYPE_2, METHOD_NAME_2_3);
            _ = _factory.CreateProcess(_process_1_1);
        }

        internal static void Process_CreateSampleForProcessAndSubProcess_XlsxUtils()
        {
            ProcessFactory _factory;
            Process _process0;
            Process _process_1_1;

            ConfigureTestProcess(out _factory, out _process0, out _process_1_1);
            


            for (int i = 0; i < 2; i++)
            {
                _process0.CreateSampleForProcessAndSubProcess(SAMPLE_SIZE);
            }

            XlsxUtils.CreateProcessXLSXFile(_process0);

            _ = XlsxUtils.GetSampleStatRowsFromProcess(_process0);
        }

        internal static void ChartsUtils_CreateChartFromProcess()
        {
            ProcessFactory _factory;
            Process _process0;
            Process _process_1_1;

            ConfigureTestProcess(out _factory, out _process0, out _process_1_1);


            for (int i = 0; i < 3; i++)
            {
                _process0.CreateSampleForProcessAndSubProcess(SAMPLE_SIZE);
                Thread.Sleep(1000);
            }
 
            XlsxUtils.CreateProcessXLSXFile(_process0);
            ChartsUtils.CreateChartsFromProcess(_process0);
        }
    }
}

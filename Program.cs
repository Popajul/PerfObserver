﻿using System.Diagnostics;
using TestMethodsLibrary;

namespace PerfObserver
{
    /// <summary>
    /// Point d'entrée Temporaire du programme afin de tester ces fonctionalité en cours de developpement
    /// </summary>
    internal static class Program
    {
        static void Main()
        {
            #region Test SimplyLogPerf
            // Test with private non static method
            Type targetType = typeof(Arithmetic) ?? throw new Exception("");
            string methodName = "IsEven";
            object[]? ctorParameters = new object[] { 39 };
            BasicPerfLogger.SimplyLogPerf(targetType, methodName, ctorParameters);

            // Test with Static public Method and method's Parameters
            ctorParameters = null;
            var parametersTypes = new Type[] { typeof(string) };
            var methodParameters = new object[] { "5" };
            BasicPerfLogger.SimplyLogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);

            // Test With invalid Method Name
            methodName = "invalid";
            try
            {
                BasicPerfLogger.SimplyLogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);
            }
            catch (Exception e )
            {
                Console.WriteLine(e.Message);
            }


            // test with invalid Parameters
            methodName = "IsEven";
            methodParameters = new object[] {5};
            try
            {
                BasicPerfLogger.SimplyLogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Test with void return Method
            targetType = typeof(Arithmetic) ?? throw new Exception("");
            methodName = "LogIsEven";
            ctorParameters = new object[] { 39 };
            BasicPerfLogger.SimplyLogPerf(targetType, methodName, ctorParameters);
            #endregion

            #region Test LogProcessPerf
            // Test Processus LogProcessPerf with depth 3 process
            targetType = typeof(FakeMethods);
            string methodName_0 = "FakeMethod_Depth_0";
            string methodName_1_0 = "FakeMethod_Depth_1_0";
            string methodName_1_1 = "FakeMethod_Depth_1_1";
            string methodName_2_0 = "FakeMethod_Depth_2_0";
            ProcessFactory factory = new(targetType, methodName_0);
            var process_0 = factory.CreateProcess();
            factory = new(targetType, methodName_1_0);
            _ = factory.CreateProcess(process_0);

            factory = new(targetType, methodName_1_1);
            var process_1_1 = factory.CreateProcess(process_0);

            factory = new(targetType, methodName_2_0);
            _ = factory.CreateProcess(process_1_1);
            BasicPerfLogger.LogProcessPerf(process_0);

            #endregion
            #region Test Statistics
            // test sans parallelisme sur l'itération des sous processus
            var sw = new Stopwatch();
            sw.Start();
            BasicPerfLogger.LogProcessSampleStatistics(process_0);
            sw.Stop();
            Console.WriteLine($"Debug - TOTAL non // : {sw.ElapsedMilliseconds} ms");

            // test avec parallelisme sur l'itération des sous processus

            sw = new Stopwatch();
            sw.Start();
            BasicPerfLogger.LogProcessSampleStatisticsParallel(process_0);
            sw.Stop();
            Console.WriteLine($"Debug - TOTAL // : {sw.ElapsedMilliseconds} ms");


            // environ 2,5 secondes d'avantage sur le parallelisme
            // resultat coherent on s'attend dans ce cas précis à gagner tout au plus le temps le plus court des 2 processus de second niveau A savoir 
            // 500ms * 5 = 2.5s
            // le parallelisme joue parfaitement son rôle içi
            #endregion
        }
    }
}
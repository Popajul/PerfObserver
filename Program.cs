
using TestMethods;

namespace PerfObserver
{
    /// <summary>
    /// Point d'entrée Temporaire du programme afin de tester ces fonctionalité en cours de developpement
    /// </summary>
    internal static class Program
    {
        static void Main()
        {
            // Test with private non static method
            Type targetType = typeof(Arithmetic) ?? throw new Exception("");
            string methodName = "IsEven";
            object[]? ctorParameters = new object[] { 39 };
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters);

            // Test with Static public Method and method's Parameters
            ctorParameters = null;
            var parametersTypes = new Type[] { typeof(string) };
            var methodParameters = new object[] { "5" };
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);

            // Test With invalid Method Name
            methodName = "invalid";
            try
            {
                BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);
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
                BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Test with void return Method
            targetType = typeof(Arithmetic) ?? throw new Exception("");
            methodName = "LogIsEven";
            ctorParameters = new object[] { 39 };
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters);


        }
    }
}
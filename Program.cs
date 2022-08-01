using System.Globalization;
using System.Reflection;
using TestMethods;

namespace PerfObserver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Test with public noin static method
            Type targetType = typeof(Arithmetic) ?? throw new Exception("");
            string methodName = "IsEven";
            object[] ctorParameters = new object[] { 39 };
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters);

            // Test with Static private Method and method's Parameters
            ctorParameters = null;
            var parametersTypes = new Type[] { typeof(string) };
            var methodParameters = new object[] { "5" };
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);

            // Test With invalid Method Name
            /*methodName = "invalid";
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);*/

            // test with invalid Parameters
            /*methodParameters = new object[] {5};
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters, parametersTypes, methodParameters);*/

            // Test with void return Method

            targetType = typeof(Arithmetic) ?? throw new Exception("");
            methodName = "LogIsEven";
            ctorParameters = new object[] { 39 };
            BasicPerfLogger.SimplylogPerf(targetType, methodName, ctorParameters);


        }
    }
}
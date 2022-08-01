using System.Diagnostics;

namespace PerfObserver
{
    internal static class BasicPerfLogger
    {
        public static void SimplylogPerf(Type targetType, string methodName, object[]? ctorParameters = null, Type[]? parametersTypes = null, object[]? methodParameters = null)
        {
            var methodInfo = MethodInfoAndInstanceRecover.GetMethodInfo(targetType, methodName, parametersTypes);
            var instance = MethodInfoAndInstanceRecover.GetInstanceForInvokingMethod(targetType, methodInfo, ctorParameters);

            object? result;
            var sw = new Stopwatch();
            sw.Start();
            try
            {
               result = methodInfo.Invoke(instance, methodParameters);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.StackTrace);
                throw new ArgumentException("ERROR_INVALID_METHODS_PARAMETERS");
            }
            sw.Stop();
            Console.WriteLine($"PERF --- Called Method Name : {methodInfo.Name} || elapsedTime : {sw.ElapsedMilliseconds} || return : {result ?? "void"}");

        }
    }
}

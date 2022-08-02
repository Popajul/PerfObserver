using System.Diagnostics;

namespace PerfObserver
{
    public static class BasicPerfLogger
    {
        public static void SimplyLogPerf(Type targetType, string methodName, object[]? ctorParameters = null, Type[]? parametersTypes = null, object[]? methodParameters = null)
        {
            var methodInfo = MethodInfoAndInstanceRecoverer.GetMethodInfo(targetType, methodName, parametersTypes);
            var instance = MethodInfoAndInstanceRecoverer.GetHostingInstance(targetType, methodInfo, ctorParameters);

            object? result;
            var sw = new Stopwatch();
            sw.Start();
            try
            {
               result = methodInfo.Invoke(instance, methodParameters);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("ERROR_INVALID_METHODS_PARAMETERS");
            }
            sw.Stop();
            Console.WriteLine($"PERF --- Called Method Name : {methodInfo.Name} || elapsedTime : {sw.ElapsedMilliseconds} ms || return : {result ?? "void"}");

        }
        public static void LogProcessPerf(Process process, int depth = -1)
        {
            depth++;
            var elapsed_time = process.Observe();
            Console.WriteLine($"PERF --- Called Method Name : {process._methodInfo.Name} || elapsedTime : {elapsed_time} ms  || depth : {depth}  || parent : {process.Parent?._methodInfo.Name ?? "none"}");
            foreach(var proc in process.SubProcesses)
                LogProcessPerf(proc, depth);


        }
    }
}

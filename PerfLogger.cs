using PerfObserver.Reflection;
using System;
using System.Diagnostics;

namespace PerfObserver
{
    public static class PerfLogger
    {
        public static void LogProcess(Type targetType, string methodName, object[] ctorParameters = null, Type[] parametersTypes = null, object[] methodParameters = null)
        {
            var methodInfo = ReflectionUtils.GetMethodInfo(targetType, methodName, parametersTypes);
            var instance = ReflectionUtils.GetHostingInstance(targetType, methodInfo, ctorParameters);

            object result;
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
            Console.WriteLine($"PERF - Method Name : {methodInfo.Name} || elapsedTime : {sw.ElapsedMilliseconds} ms || return : {result ?? "void"}");

        }

        public static void LogProcess(Process process)
        {
            LogProcess(process, -1);
        }

        public static void CompareProcess(Process process1, Process process2, int numberOfExecutions)
        {
            
            Console.WriteLine($"PERF: {process1.Name} || {process1.ObserveMany(numberOfExecutions)}   || param: {string.Join(',', process1._parameters)}    || x: {numberOfExecutions} times");
            Console.WriteLine($"PERF: {process2.Name} || {process2.ObserveMany(numberOfExecutions)}   || param: {string.Join(',', process2._parameters)}    || x: {numberOfExecutions} times");
        }
        public static void LogProcessSample(Process process, int sampleSize)
        {
            LogProcessSample(process, sampleSize, -1);
        }
        private static void LogProcess(Process process, int depth)
        {
            depth++;
            var elapsed_time = process.Observe();
            Console.WriteLine($"PERF: {process.Name} || {elapsed_time} ms  || param: {string.Join(',', process._parameters)}    || parent : {process.Parent?.Name ?? ""}");
            foreach (var proc in process.SubProcesses)
                LogProcess(proc, depth);
        }
        private static void LogProcessSample(Process process, int sampleSize, int depth)
        {
            depth++;
            var sw = new Stopwatch();
            sw.Start();
            var sample = process.CreateSample(sampleSize);
            var statistics = sample.Statistics;
            Console.WriteLine($"Stat -_ Method Name : {process.Name} || AverageTime : {statistics!.AverageTime} ms  || StandartDeviation : {statistics.StandartDeviation} ||depth : {depth}  || parent : {process.Parent?.Name ?? "none"} || MainProcessusRatio (%): {statistics.MainProcessusRatio?.ToString() ?? "none"}");
            sw.Stop();
            Console.WriteLine($"Debug - // : {sw.ElapsedMilliseconds} ms");

            var actions = new List<Action>();
            foreach (var proc in process.SubProcesses)
            {
                actions.Add(() => LogProcessSample(proc, sampleSize, depth));
            }
            if (actions.Any())
                Parallel.Invoke(actions.ToArray());
        }
    }

}

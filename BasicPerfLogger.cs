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
            Console.WriteLine($"PERF - Method Name : {methodInfo.Name} || elapsedTime : {sw.ElapsedMilliseconds} ms || return : {result ?? "void"}");

        }
        public static void LogProcessPerf(Process process, int depth = -1)
        {
            depth++;
            var elapsed_time = process.Observe();
            Console.WriteLine($"PERF - Method Name : {process._methodInfo.Name} || elapsedTime : {elapsed_time} ms  || depth : {depth}  || parent : {process.Parent?._methodInfo.Name ?? "none"}");
            foreach (var proc in process.SubProcesses)
                LogProcessPerf(proc, depth);


        }

        public static void LogProcessSampleStatistics(Process process, int depth = -1)
        {
            const int SAMPLE_SIZE = 5;
            depth++;
            var sw = new Stopwatch();
            sw.Start();
            var sample = process.CreateSample(SAMPLE_SIZE);
            var statistics = sample.Statistics;
            Console.WriteLine($"Stat -_ Method Name : {process._methodInfo.Name} || AverageTime : {statistics!.AverageTimes} ms  || StandartDeviation : {statistics.StandartDeviation} ||depth : {depth}  || parent : {process.Parent?._methodInfo.Name ?? "none"} || MainProcessusRatio (%): {statistics.MainProcessusRatio?.ToString() ?? "none"}");
            sw.Stop();
            Console.WriteLine($"Debug - non // : {sw.ElapsedMilliseconds} ms");
            foreach (var proc in process.SubProcesses)
                LogProcessSampleStatistics(proc, depth);

        }

        public static void LogProcessSampleStatisticsParallel(Process process, int depth = -1)
        {
            const int SAMPLE_SIZE = 5;
            depth++;
            var sw = new Stopwatch();
            sw.Start();
            var sample = process.CreateSample(SAMPLE_SIZE);
            var statistics = sample.Statistics;
            Console.WriteLine($"Stat -_ Method Name : {process._methodInfo.Name} || AverageTime : {statistics!.AverageTimes} ms  || StandartDeviation : {statistics.StandartDeviation} ||depth : {depth}  || parent : {process.Parent?._methodInfo.Name ?? "none"} || MainProcessusRatio (%): {statistics.MainProcessusRatio?.ToString() ?? "none"}");
            sw.Stop();
            Console.WriteLine($"Debug - // : {sw.ElapsedMilliseconds} ms");

            var actions = new List<Action>();
            foreach(var proc in process.SubProcesses)
            {
                actions.Add(()=>LogProcessSampleStatisticsParallel(proc, depth));
            }
            if(actions.Any())
                Parallel.Invoke(actions.ToArray());

        }
    }

}

using PerfObserver.Model;
using System.Diagnostics;
using System.Reflection;

namespace PerfObserver
{
    public class Process
    {
        private readonly object _instance;
        internal readonly MethodInfo _methodInfo;
        private readonly object[]? _parameters;

        private  readonly List<Process> _subProcesses;
        private readonly Stopwatch _sw;

        internal readonly Process? Parent;
        internal List<Process> SubProcesses
        {
            get { return _subProcesses; }
            
        }

        internal List<Sample> Samples;
        internal Process(object instance, MethodInfo methodInfo, object[]? parameters = null, Process? parent = null)
        {
            _instance = instance;
            _methodInfo = methodInfo;
            _parameters = parameters;
            _subProcesses = new();
            Parent = parent;
            Samples = new();
            _sw = new();
        }

        internal void AddSubProcess(Process process)
        {
            _subProcesses.Add(process);
        }

        internal long Observe()
        {
            
            try
            {
                _sw.Restart();
                _ = _methodInfo.Invoke(_instance, _parameters);
                _sw.Stop();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.StackTrace);
                throw new ArgumentException("ERROR_INVALID_METHODS_PARAMETERS");
            }
            
            return _sw.ElapsedMilliseconds;
        }

        internal Sample CreateSample(int sampleSize)
        {
            Sample sample = new(this, sampleSize)
            {
                SampleIndex = Samples.Count
            };

            for (int i = 0; i < sampleSize; i++)
                sample.StopWatchValues.Add(this.Observe());

            sample.Statistics = new(sample);

            this.Samples.Add(sample);
            return sample;
        }
    }
}

using PerfObserver.Model;
using System.Diagnostics;
using System.Reflection;

namespace PerfObserver
{
    public class Process
    {
        #region properties
        private readonly object _instance;
        private readonly MethodInfo _methodInfo;
        internal readonly object[] _parameters;
        private  readonly List<Process> _subProcesses;
        private readonly Stopwatch _sw;

        internal readonly string Name;
        internal readonly Process Parent;
        internal List<Process> SubProcesses
        {
            get { return _subProcesses; }
            
        }

        internal Project Project;

        internal List<Sample> Samples;
        #endregion properties

        #region ctors
        internal Process(object instance, MethodInfo methodInfo, Project project, object[] parameters = null, Process parent = null)
        {
            _instance = instance;
            _methodInfo = methodInfo;
            _parameters = parameters;
            _subProcesses = new();
            Name = methodInfo.Name;
            Parent = parent;
            Samples = new();
            _sw = new();
            Project = project;
        }
        internal Process(object instance, MethodInfo methodInfo, object[] parameters = null, Process parent = null)
        {
            _instance = instance;
            _methodInfo = methodInfo;
            _parameters = parameters;
            _subProcesses = new();
            Name = methodInfo.Name;
            Parent = parent;
            Samples = new();
            _sw = new();
        }
        #endregion
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

        internal TimeSpan ObserveMany(long count)
        {

            try
            {
                _sw.Restart();
                for(int i = 0; i < count; i++)
                    _ = _methodInfo.Invoke(_instance, _parameters);
                _sw.Stop();
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.StackTrace);
                throw new ArgumentException("ERROR_INVALID_METHODS_PARAMETERS");
            }

            return _sw.Elapsed;
        }
        internal Sample CreateSample(int sampleSize)
        {
            Console.WriteLine($"CreateSample : {this._methodInfo.Name}");
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

        internal void CreateSampleForProcessAndSubProcess(int sampleSize)
        {
            Console.WriteLine($"CreateSampleForProcess : {this._methodInfo.Name}");
            CreateSample(sampleSize);
            var actions = new List<Action>();
            foreach (Process proc in _subProcesses)
            {
                actions.Add(() => proc.CreateSampleForProcessAndSubProcess(sampleSize)); 
            }
            if (actions.Any())
                Parallel.Invoke(actions.ToArray());
        }
    }
}

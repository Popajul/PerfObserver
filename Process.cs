using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        internal Process(object instance, MethodInfo methodInfo, object[]? parameters = null, Process? parent = null)
        {
            _instance = instance;
            _methodInfo = methodInfo;
            _parameters = parameters;
            _subProcesses = new();
            Parent = parent;
            _sw = new();
        }

        internal void AddSubProcess(Process process)
        {
            _subProcesses.Add(process);
        }

        internal long Observe()
        {
            _sw.Restart();
            try
            {
                _ = _methodInfo.Invoke(_instance, _parameters);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.StackTrace);
                throw new ArgumentException("ERROR_INVALID_METHODS_PARAMETERS");
            }
            _sw.Stop();
            return _sw.ElapsedMilliseconds;
        }
    }
}

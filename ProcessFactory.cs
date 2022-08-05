using System.Reflection;

namespace PerfObserver
{
    public class ProcessFactory
    {
        private readonly BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private readonly Type _targetType;
        private readonly string _methodName;
        private readonly Type[] _parametersTypes;
        private readonly object[] _ctorParameters;
        private readonly object[] _methodParameters;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="methode"></param>
        /// <param name="parametersTypes"></param>
        /// <param name="ctorParameters"></param>
        /// <param name="methodParameters"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProcessFactory(Type targetType, string methode, Type[] parametersTypes = null, object[]  ctorParameters = null, object[] methodParameters = null)
        {
            _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            _methodName = methode ?? throw new ArgumentNullException(nameof(methode));
            _parametersTypes = parametersTypes;
            _ctorParameters = ctorParameters;
            _methodParameters = methodParameters;
        }

        /// <summary>
        /// Create a Process, provide parent when creating SubProcess
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Process CreateProcess(Process parent = null)
        {
            
            MethodInfo methodInfo = GetMethodInfo(_targetType, _methodName, _parametersTypes);
            object instance = GetHostingInstance(_targetType, methodInfo, _ctorParameters);
            Process process = new(instance, methodInfo, _methodParameters, parent);
            if(parent !=null)
                parent.AddSubProcess(process);
            return process;
            
        }
        /// <summary>
        /// Get MethodInfo
        /// </summary>
        /// <param name="targetType"> Type of method's parent class</param>
        /// <param name="methodName"> Name of method </param>
        /// <param name="parametersTypes"> Types to indentify method's signature </param>
        /// <returns>MethodInfo for this m</returns>
        /// <exception cref="Exception"> </exception>
        private MethodInfo GetMethodInfo(Type targetType, string methodName, Type[] parametersTypes = null)
        {
            parametersTypes ??= Array.Empty<Type>();

            // Recover Method Info
            return targetType.GetMethod(methodName, bindingFlags, parametersTypes) ?? throw new Exception("ERROR_RECOVERING_METHOD_INFO");
        }
        /// <summary>
        /// Get Instance hosting Method to invoke 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="methodInfo"></param>
        /// <param name="ctorParameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private object GetHostingInstance(Type targetType, MethodInfo methodInfo, object[] ctorParameters = null)
        {
            object instance = targetType;
            if (!methodInfo.IsStatic)
            {
                instance = targetType.Assembly.CreateInstance(targetType.FullName!, true, bindingFlags, null, ctorParameters, null, null) ?? throw new Exception("ERROR_RECOVERING_INSTANCE");
            }
            return instance;
        }
    }
}

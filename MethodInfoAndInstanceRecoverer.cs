using System.Reflection;

namespace PerfObserver
{
    internal static class MethodInfoAndInstanceRecover
    {
        private static readonly BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        /// <summary>
        /// GetMethodInfo
        /// </summary>
        /// <param name="targetType"> Type of method's parent class</param>
        /// <param name="methodName"> Name of method </param>
        /// <param name="parametersTypes"> Types to indentify method's signature </param>
        /// <returns>MethodInfo for this m</returns>
        /// <exception cref="Exception"> </exception>
        internal static MethodInfo GetMethodInfo(Type targetType, string methodName,Type[]? parametersTypes = null)
        {
            parametersTypes ??= Array.Empty<Type>();

            // Recover Method Info
            return targetType.GetMethod(methodName, bindingFlags, parametersTypes) ?? throw new Exception("ERROR_RECOVERING_METHOD_INFO");
        }
        public static object GetInstanceForInvokingMethod(Type targetType, MethodInfo methodInfo, object[]? ctorParameters = null)
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

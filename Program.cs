using System.Globalization;
using System.Reflection;
using TestMethods;

namespace PerfObserver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // input sur le target Type : ctorParameters object[] vide par defaut + targetType
            // input sur la méthode à tester : parameterTypes -> object[] vide par default, methode Name
            // recupérer une methode dans un projet


            // la déclaration du type cible -> ajout d'une reference de projet 
            
            Type targetType = typeof(Arithmetic) ?? throw new Exception("");

            // les bindingFlags permettre d'inclure les critères dans la recherche de la méthode  pour le type fourni
            // On veut veur récupérer les méthodes static ou pas , public ou pas 
            // Selon les cas à traiter 
            var bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            // ParameterTypes permet de distinguer plusieurs signature d'une même méthode
            var parameterTypes = new Type[] { };
            MethodInfo? methodInfo = targetType.GetMethod("IsEven", bindingFlags, parameterTypes);

            // Si la méthode est static l'instance est le targetType sinon il faut la créér en utilisant 
            object instance = targetType;
            if (!methodInfo.IsStatic)
            {
                object[] ctorParameters = new object[] {4};
                // création de l'instance par l'assembly : ignoreCase
                instance =  targetType.Assembly.CreateInstance(targetType.FullName, true, bindingFlags, null, ctorParameters, null, null);
            }

            var result = methodInfo.Invoke(instance, new object[] { "4" });
        }
    }
}
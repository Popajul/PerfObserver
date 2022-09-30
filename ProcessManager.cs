using PerfObserver.Charts;
using PerfObserver.XLSX;

namespace PerfObserver
{
    public static class ProcessManager
    {
        /// <summary>
        /// Création d'un processus Principal 
        /// </summary>
        /// <param name="targetType"> typeof(ClasseName)</param>
        /// <param name="methode">nom de la méthode ciblé</param>
        /// <param name="parametersTypes"> tableau des types des paramètres pour identification de la méthode par sa signature</param>
        /// <param name="ctorParameters"> Pramamètres du constructeur de la classe hôte</param>
        /// <param name="methodParameters"> Valeurs des paramètres utilisées pour l'éxécution de la méthode</param>
        /// <returns>Le processus créé</returns>
        public static Process CreateProcess(Type targetType, string methode, Type[] parametersTypes = null, object[] ctorParameters = null, object[] methodParameters = null)
        {
            ProcessFactory factory = new (targetType, methode, parametersTypes, ctorParameters, methodParameters);
            return factory.CreateProcess();
        }

        /// <summary>
        /// Creation d'un sous processus associé au processus parent
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="targetType"></param>
        /// <param name="methode"></param>
        /// <param name="parametersTypes"></param>
        /// <param name="ctorParameters"></param>
        /// <param name="methodParameters"></param>
        /// <returns>Le sous processus créé </returns>
        public static Process CreateSubProcess(Process parent, Type targetType, string methode, Type[] parametersTypes = null, object[] ctorParameters = null, object[] methodParameters = null)
        {
            ProcessFactory factory = new(targetType, methode, parametersTypes, ctorParameters, methodParameters);
            return factory.CreateProcess(parent);
        }
        /// <summary>
        /// Création d'un échantillon
        /// </summary>
        /// <param name="process">Processus pour lequel l'échantillon est créé</param>
        /// <param name="sampleSize">taille de l'échantillon</param>
        /// <param name="createSubProcessSample">flag indiquant si le processus doit être échantilloné à tous ses niveaux de profondeur</param>
        /// <exception cref="InvalidDataException"></exception>
        public static void CreateSample(Process process , int sampleSize, bool createSubProcessSample = true)
        {
            
            if (sampleSize <= 0)
                throw new InvalidDataException($"{nameof(sampleSize)} = {sampleSize} or must have a positive value");
        

            if (createSubProcessSample)
            {
                process.CreateSampleForProcessAndSubProcess(sampleSize);
                return;
            }
             
            process.CreateSample(sampleSize); 
        }


        /// <summary>
        /// Sauvegarde les données reccueilies d'un processus et de ses sous processus
        /// </summary>
        /// <param name="process"></param>
        public static void SaveSampleDataToFile(Process process) => XlsxUtils.CreateProcessXLSXFile(process);

        /// <summary>
        /// Création de tous les graphiques 
        /// </summary>
        /// <param name="process"></param>
        public static void CreateCharts(Process process) => ChartsUtils.CreateChartsFromProcess(process);
        /// <summary>
        /// Création des diagrammes circulaire circulaires
        /// </summary>
        /// <param name="process"></param>
        public static void CreatePieCharts(Process process) => ChartsUtils.CreatePieChartsFromProcess(process);
        /// <summary>
        /// Création des diagrammes en barre
        /// </summary>
        /// <param name="process"></param>
        public static void CreateBarCharts(Process process) => ChartsUtils.CreateBarChartsFromProcess(process);
        /// <summary>
        /// Création des graphiques de type "line"
        /// </summary>
        /// <param name="process"></param>
        public static void CreateLineCharts(Process process) => ChartsUtils.CreateLineChartsFromProcess(process);

    }
}

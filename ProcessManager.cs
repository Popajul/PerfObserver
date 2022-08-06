using PerfObserver.Charts;
using PerfObserver.XLSX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfObserver
{
    public class ProcessManager
    {
        /// <summary>
        /// Create process 
        /// </summary>
        /// <param name="targetType"> typeof(ClasseName)</param>
        /// <param name="methode">Process Method's name</param>
        /// <param name="parametersTypes">  Method's parameters type Array</param>
        /// <param name="ctorParameters"> Parameters to build hosting type</param>
        /// <param name="methodParameters"> Parameters value to invoke process method</param>
        /// <returns>a new process</returns>
        public Process CreateProcess(Type targetType, string methode, Type[] parametersTypes = null, object[] ctorParameters = null, object[] methodParameters = null)
        {
            ProcessFactory factory = new (targetType, methode, parametersTypes, ctorParameters, methodParameters);
            return factory.CreateProcess();
        }

        /// <summary>
        /// Create SubProcess
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="targetType"></param>
        /// <param name="methode"></param>
        /// <param name="parametersTypes"></param>
        /// <param name="ctorParameters"></param>
        /// <param name="methodParameters"></param>
        /// <returns>new child process from parents </returns>
        public Process CreateSubProcess(Process parent, Type targetType, string methode, Type[] parametersTypes = null, object[] ctorParameters = null, object[] methodParameters = null)
        {
            ProcessFactory factory = new(targetType, methode, parametersTypes, ctorParameters, methodParameters);
            return factory.CreateProcess(parent);
        }
        public void CreateSample(Process process , int sampleSize, bool createSubProcessSample = true)
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
        /// Save Sample Data to xlsxFile
        /// </summary>
        /// <param name="process"></param>
        public void SaveSampleDataToFile(Process process) => XlsxUtils.CreateProcessXLSXFile(process);

        public void CreateCharts(Process process) => ChartsUtils.CreateChartsFromProcess(process);
        public void CreatePieCharts(Process process) => ChartsUtils.CreatePieChartsFromProcess(process);
        public void CreateBarCharts(Process process) => ChartsUtils.CreateBarChartsFromProcess(process);
        public void CreateLineCharts(Process process) => ChartsUtils.CreateLineChartsFromProcess(process);

    }
}

namespace PerfObserver.Model
{
    internal class Sample
    {
        /// <summary>
        /// Processus sur lequel est réalisé l'échantillon
        /// </summary>
        internal Process Process;
        /// <summary>
        /// Temps d'éxécutions recueillis sur le processus
        /// </summary>
        internal List<double> StopWatchValues;
        /// <summary>
        /// taille de l'échantillon
        /// </summary>
        internal int SampleSize;
        /// <summary>
        /// Statistic déstiné à être mise à jour par le module statistique
        /// </summary>
        internal Statistics Statistics;

        /// <summary>
        /// Index de l'échantillon dans la collection du processus
        /// </summary>
        internal int SampleIndex;
        /// <summary>
        /// Date de l'échantillonage
        /// </summary>
        internal string SampleDateTime;
        internal Sample (Process process, int sampleSize)
        {
            Process = process;
            StopWatchValues = new();
            SampleSize = sampleSize;
            SampleDateTime = DateTime.Now.ToString("G");
        }

 
    }
}

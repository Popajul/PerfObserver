namespace PerfObserver.Model
{
    internal class Sample
    {
        internal Process Process;
        internal List<long> StopWatchValues;
        internal int SampleSize;
        internal Statistics? Statistics;
        internal int SampleIndex;
        internal Sample (Process process, int sampleSize)
        {
            Process = process;
            StopWatchValues = new();
            SampleSize = sampleSize;
        }

 
    }
}

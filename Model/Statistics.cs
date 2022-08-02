namespace PerfObserver.Model
{
    internal class Statistics
    {
        internal Sample Sample;
        internal long AverageTimes;
        internal double StandartDeviation;
        internal double? MainProcessusRatio;

        internal Statistics(Sample sample)
        {
            Sample = sample;

            var sampleSize = sample.SampleSize;
            var stopWatchValues = sample.StopWatchValues;

            AverageTimes = sample.StopWatchValues.Sum() / sampleSize;
            StandartDeviation = Math.Round(Math.Sqrt(stopWatchValues.Select(s=>Math.Pow(s - AverageTimes,2)).Sum() / (double)sampleSize) , 2);
            
            Process? sampleProcessParent = sample.Process.Parent;
            var parentSample = sampleProcessParent?.Samples.ElementAt(sample.SampleIndex);
            if (parentSample != null)
                MainProcessusRatio = Math.Round(100 * (double)AverageTimes / (double)parentSample.Statistics!.AverageTimes , 2 );
        }
    }
}

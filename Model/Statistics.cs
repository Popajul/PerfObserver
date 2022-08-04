namespace PerfObserver.Model
{
    internal class Statistics
    {
        internal Sample Sample;
        internal long AverageTime;
        internal double StandartDeviation;
        internal double? MainProcessusRatio;
        internal long MinValue;
        internal long MaxValue;
        internal Statistics(Sample sample)
        {
            Sample = sample;

            var sampleSize = sample.SampleSize;
            var stopWatchValues = sample.StopWatchValues;

            AverageTime = sample.StopWatchValues.Sum() / sampleSize;
            StandartDeviation = Math.Round(Math.Sqrt(stopWatchValues.Select(s=>Math.Pow(s - AverageTime,2)).Sum() / (double)sampleSize) , 2);
            
            MinValue = sample.StopWatchValues.Min();
            MaxValue = sample.StopWatchValues.Max();
            Process? sampleProcessParent = sample.Process.Parent;
            var parentSample = sampleProcessParent?.Samples.ElementAt(sample.SampleIndex);
            if (parentSample != null)
                MainProcessusRatio = Math.Round(100 * (double)AverageTime / (double)parentSample.Statistics!.AverageTime , 2 );
        }
    }
}

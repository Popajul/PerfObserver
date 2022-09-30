namespace PerfObserver.Model
{
    internal class Statistics
    {
        internal Sample Sample;
        internal double AverageTime;
        internal double StandartDeviation;
        internal double? MainProcessusRatio;
        internal double MinValue;
        internal double MaxValue;
        internal Statistics(Sample sample)
        {
            Sample = sample;

            var sampleSize = sample.SampleSize;
            var stopWatchValues = sample.StopWatchValues;
            var filteredSWValues = stopWatchValues.OrderBy(x => x).Skip(sampleSize/10).SkipLast(sampleSize/10);
            AverageTime = filteredSWValues.Sum() / filteredSWValues.Count();
            StandartDeviation = Math.Round(Math.Sqrt(filteredSWValues.Select(s=>Math.Pow(s - AverageTime,2)).Sum() / (double)sampleSize) , 2);
            
            MinValue = sample.StopWatchValues.Min();
            MaxValue = sample.StopWatchValues.Max();
            Process sampleProcessParent = sample.Process.Parent;
            var parentSample = sampleProcessParent?.Samples.ElementAt(sample.SampleIndex);
            if (parentSample != null)
                MainProcessusRatio = Math.Round(100 * AverageTime / parentSample.Statistics!.AverageTime , 6 );
        }
    }
}

using PerfObserver.Model;
using PerfObserver.XLSX;
using QuickChart;
namespace PerfObserver.Charts
{
    internal static class ChartsUtils
    {
        internal static readonly string DIRECTORY_BASE = $"{Directory.GetCurrentDirectory()}/Workbook";
        internal static readonly int WIDTH = 1000;
        internal static readonly int HEIGHT = 600;
        internal static readonly string BACKGROUND_COLOR = "white";

        private static readonly string CHART_CONFIG_GENERAL_TEMPLATE =
            @"{
                type: 'TYPE',
                data: {
                    labels: [LABEL_TAB_VALUES],
                    datasets: [DATASETS]
                },
                options: {
                    title: {
                        display: true,
                        text: ['MAIN_TITLE', 'SUB_TITLE'],
                        font : {
                            fontSize : 50
                        }
                    }
                }
            }";
        private static readonly string CHART_DATA_SETS_TEMPLATE = @"{
                    label: 'DATASET_LABEL',
                    data: [DATASET_TAB_VALUES],
                    fill:FILL_BOOL_VALUE,
                    borderColor:'BORDER_COLOR'
                    }";

        private static readonly string PIE_CHART_DATA_SETS_TEMPLATE = @"{
                    data: [DATASET_TAB_VALUES],
                    fill:FILL_BOOL_VALUE,
                    borderColor:'BORDER_COLOR'
                    }";
        private static string GetGeneralConfigFromTemplate(string chartType, string[] labelValues, List<string> datasetLabels, List<object[]> dataSetsValues, bool[] fills, string[] borderColors, string title, string subTitle)
        {
            var config = CHART_CONFIG_GENERAL_TEMPLATE;

            config = config.Replace("TYPE", chartType);

            var labelTabValues = string.Join(',', labelValues.Select(s => $"'{s}'"));
            config = config.Replace("LABEL_TAB_VALUES", labelTabValues);

            var datasets = new List<string>();


            string dataset;
            for (int i = 0; i < datasetLabels.Count; i++)
            {
                dataset = CHART_DATA_SETS_TEMPLATE;
                dataset = dataset.Replace("DATASET_LABEL", datasetLabels.ElementAt(i));

                var dataSetTabValues = string.Join(',', dataSetsValues.ElementAt(i));
                dataset = dataset.Replace("DATASET_TAB_VALUES", dataSetTabValues);

                dataset = dataset.Replace("FILL_BOOL_VALUE", fills.ElementAt(i).ToString().ToLower());
                dataset = dataset.Replace("BORDER_COLOR", borderColors.ElementAt(i));
                datasets.Add(dataset);
            }

            config = config.Replace("DATASETS", string.Join(",\r\n", datasets));
            config = config.Replace("MAIN_TITLE", title);
            config = config.Replace("SUB_TITLE", subTitle);

            return config;
        }

        private static string GetPieConfigFromTemplate(string chartType, string[] labelValues, object[] dataSetsValues, bool fill, string borderColor, string title, string subTitle)
        {
            var config = CHART_CONFIG_GENERAL_TEMPLATE;

            config = config.Replace("TYPE", chartType);

            var labelTabValues = string.Join(',', labelValues.Select(s => $"'{s}'"));
            config = config.Replace("LABEL_TAB_VALUES", labelTabValues);

            string dataset = PIE_CHART_DATA_SETS_TEMPLATE;


            var dataSetTabValues = string.Join(',', dataSetsValues.Select(v => v.ToString().Replace(',', '.')));
            dataset = dataset.Replace("DATASET_TAB_VALUES", dataSetTabValues);

            dataset = dataset.Replace("FILL_BOOL_VALUE", fill.ToString().ToLower());
            dataset = dataset.Replace("BORDER_COLOR", borderColor);

            config = config.Replace("DATASETS", string.Join(",\r\n", dataset));
            config = config.Replace("MAIN_TITLE", title);
            config = config.Replace("SUB_TITLE", subTitle);

            return config;
        }
        private static Chart GetChartFromConfig(string config)
        {
            Console.WriteLine("Asking new chart to External Api");
            return new()
            {
                Width = WIDTH,
                Height = HEIGHT,
                BackgroundColor = BACKGROUND_COLOR,

                Config = config
            };
        }

        internal static void CreatePieChartsFromProcess(Process process)
        {
            var processDirectory = DIRECTORY_BASE;
            processDirectory += process.Project == null ? $"/{process._methodInfo.Name}" : $"/Projects/{process.Project!.Name}/{process._methodInfo.Name}";


            var chartsProcessDirectory = $"{processDirectory}/Charts";
            Directory.CreateDirectory(chartsProcessDirectory);
            chartsProcessDirectory += "/Samples";
            Directory.CreateDirectory(chartsProcessDirectory);

            var sampleStatRows = XlsxUtils.GetSampleStatRowsFromProcess(process);

            Chart chart;
            string charType = "doughnut";
            string[] labelValues;
            object[] datasetValues;
            bool fill = true;
            string borderColor = "transparent";
            string title;
            string subtitle;

            string config;
            foreach (var row in sampleStatRows.Where(s => s.SubProcessRatio.Any()))
            {
                var formatDateTime = row.SampleDateTime.Replace("/", "").Replace(' ', '_').Replace(":", "");
                var sampleDirectory = $"{chartsProcessDirectory}/{formatDateTime}";
                Directory.CreateDirectory(sampleDirectory);
                var currentChartPath = $"{sampleDirectory}/{row.ProcessName}_{formatDateTime}.png";
                if (!File.Exists(currentChartPath))
                {
                    labelValues = row.SubProcessRatio.Select(s => s.SubProcessName).Append("Other").ToArray();

                    double dataSetOtherValue = Math.Round(100d - row.SubProcessRatio.Select(s => s.MainProcessusRatio).Sum(), 2);
                    datasetValues = row.SubProcessRatio.Select(s => (object)s.MainProcessusRatio).Append((object)dataSetOtherValue).ToArray();
                    title = row.ProcessName;
                    subtitle = row.SampleDateTime;


                    config = GetPieConfigFromTemplate(charType, labelValues, datasetValues, fill, borderColor, title, subtitle);

                    chart = GetChartFromConfig(config);

                    chart.ToFile(currentChartPath);
                }
            }
        }
        internal static void CreateBarChartsFromProcess(Process process)
        {
            var processDirectory = DIRECTORY_BASE;
            processDirectory += process.Project == null ? $"/{process._methodInfo.Name}" : $"/Projects/{process.Project!.Name}/{process._methodInfo.Name}";


            var chartsProcessDirectory = $"{processDirectory}/Charts";
            Directory.CreateDirectory(chartsProcessDirectory);
            chartsProcessDirectory += "/BarCharts";
            Directory.CreateDirectory(chartsProcessDirectory);

            var sampleStatRows = XlsxUtils.GetSampleStatRowsFromProcess(process);
            var DataFilteredSampleStatRows = sampleStatRows.Select(s => new { s.ProcessName, s.SampleDateTime, s.AverageTime, subProcessNames = s.SubProcessRatio.Select(r => r.SubProcessName) });
            var customRows = DataFilteredSampleStatRows
                .Select(s =>
                new
                {
                    ProcessInfo = new { s.ProcessName, s.SampleDateTime, s.AverageTime },
                    SubProcess = DataFilteredSampleStatRows
                        .Where(d => s.subProcessNames.Contains(d.ProcessName))
                        .Select(d => new { d.ProcessName, d.AverageTime, d.SampleDateTime })
                }).Where(o => o.SubProcess.Any());



            var customRowsGroupByProcessName = customRows.GroupBy(c => c.ProcessInfo.ProcessName);
            
            var setOfDataSets = new List<DataSet[]>();
            foreach (var group in customRowsGroupByProcessName)
            {
                var dataSets = new List<DataSet>
                {
                    new DataSet { Label = group.First().ProcessInfo.ProcessName, Values = group.Select(g => (object)g.ProcessInfo.AverageTime).ToArray() }
                };
                foreach (var sub in group.First().SubProcess.GroupBy(s => s.ProcessName))
                {
                    dataSets.Add(new DataSet() { Label = sub.Key, Values = sub.Select(g => (object)g.AverageTime).ToArray() });
                }
                setOfDataSets.Add(dataSets.ToArray());
            }


            // création des Graphiques
            var currentChartsDirectory = $"{chartsProcessDirectory}/{process._methodInfo.Name}";
            Directory.CreateDirectory(currentChartsDirectory);

            Chart chart;
            string charType = "bar";

            string[] labelValues = customRowsGroupByProcessName.First().Select(c => c.ProcessInfo.SampleDateTime).ToArray();

            string title;
            string subtitle = "";
            string config;


            foreach (var dataSets in setOfDataSets)
            {

                bool[] fills = dataSets.Select(d => true).ToArray();
              
                string[] borderColors = dataSets.Select(d => "transparent").ToArray();
                title = dataSets.First().Label;
                config = GetGeneralConfigFromTemplate(charType, labelValues,
                    dataSets.Select(d => d.Label).ToList(), dataSets.Select(d => d.Values).ToList(), fills, borderColors, title, subtitle);
                chart = GetChartFromConfig(config);
                chart.ToFile($"{currentChartsDirectory}/{title}.png");

            }
        }

        internal static void CreateLineChartsFromProcess(Process process)
        {
            var processDirectory = DIRECTORY_BASE;
            processDirectory += process.Project == null ? $"/{process._methodInfo.Name}" : $"/Projects/{process.Project!.Name}/{process._methodInfo.Name}";


            var chartsProcessDirectory = $"{processDirectory}/Charts";
            Directory.CreateDirectory(chartsProcessDirectory);
            chartsProcessDirectory += "/LineCharts";
            Directory.CreateDirectory(chartsProcessDirectory);

            var sampleStatRows = XlsxUtils.GetSampleStatRowsFromProcess(process);
            var DataFilteredSampleStatRows = sampleStatRows.Select(s => new { s.ProcessName, s.SampleDateTime, s.AverageTime, subProcessNames = s.SubProcessRatio.Select(r => r.SubProcessName) });
            var customRows = DataFilteredSampleStatRows
                .Select(s =>
                new
                {
                    ProcessInfo = new { s.ProcessName, s.SampleDateTime, s.AverageTime },
                    SubProcess = DataFilteredSampleStatRows
                        .Where(d => s.subProcessNames.Contains(d.ProcessName))
                        .Select(d => new { d.ProcessName, d.AverageTime, d.SampleDateTime })
                }).Where(o => o.SubProcess.Any());



            var customRowsGroupByProcessName = customRows.GroupBy(c => c.ProcessInfo.ProcessName);
            // maintenant un group = un graphique
            // remplir une collection de datasets
            // puis itérer sur la collection pour faire les graphiques
            var setOfDataSets = new List<DataSet[]>();
            foreach (var group in customRowsGroupByProcessName)
            {
                var dataSets = new List<DataSet>
                {
                    new DataSet { Label = group.First().ProcessInfo.ProcessName, Values = group.Select(g => (object)g.ProcessInfo.AverageTime).ToArray() }
                };
                foreach (var sub in group.First().SubProcess.GroupBy(s => s.ProcessName))
                {
                    dataSets.Add(new DataSet() { Label = sub.Key, Values = sub.Select(g => (object)g.AverageTime).ToArray() });
                }
                setOfDataSets.Add(dataSets.ToArray());
            }


            // création des Graphiques
            var currentChartsDirectory = $"{chartsProcessDirectory}/{process._methodInfo.Name}";
            Directory.CreateDirectory(currentChartsDirectory);

            Chart chart;
            string charType = "line";
            string[] labelValues = customRowsGroupByProcessName.First().Select(c => c.ProcessInfo.SampleDateTime).ToArray();

            string title;
            string subtitle = "";
            string config;


            foreach (var dataSets in setOfDataSets)
            {

                bool[] fills = dataSets.Select(d => false).ToArray();
                IList<string> colors = new List<string>() { "black", "red", "blue", "green", "purple", "pink", "yellow" };
                // Associer une couleur à chaque label de data set
                Dictionary<string, string> colorsByLabel = new Dictionary<string, string>();
                var colorIndex = 0;
                dataSets.Select(d => d.Label).Distinct().ToList().ForEach(l =>
                {
                    colorsByLabel.Add(l, colors.ElementAt(colorIndex));
                    colorIndex++;
                    colorIndex = colorIndex % colors.Count();
                });
                string[] borderColors = dataSets.Select(d => colorsByLabel.GetValueOrDefault(d.Label)).ToArray();
                title = dataSets.First().Label;
                config = GetGeneralConfigFromTemplate(charType, labelValues,
                    dataSets.Select(d => d.Label).ToList(), dataSets.Select(d => d.Values).ToList(), fills, borderColors, title, subtitle);
                chart = GetChartFromConfig(config);
                chart.ToFile($"{currentChartsDirectory}/{title}.png");

            }
        }

        internal static void CreateChartsFromProcess(Process process)
        {
            CreatePieChartsFromProcess(process);
            CreateBarChartsFromProcess(process);
            CreateLineChartsFromProcess(process);
        }
    }


    internal class DataSet
    {
        internal string Label;
        internal Object[] Values;

    }

}

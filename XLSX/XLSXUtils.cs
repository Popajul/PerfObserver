using ExcelDataReader;
using PerfObserver.Model;
using PicoXLSX;
using System.Data;
using System.Text;

namespace PerfObserver.XLSX
{
    internal static class XlsxUtils
    {
        // directory to complete with /Projects/ProjectName/ProcessusName or just /ProcessusName
        internal static readonly string DIRECTORY_BASE = $"{Directory.GetCurrentDirectory()}/Workbook";

        /// <summary>
        /// Create a worbook from an existing template xlsx with a new workbookName
        /// null values for workbookDirectory and workbookName will create an intended to overwrite sourceFile workbook
        /// </summary>
        /// <param name="fileName"> fullPath for source file </param>
        /// <param name="workbookName"></param>
        /// <returns>Workbook</returns>
        private static Workbook CreateWorkbookFromXLSXFile(string sourceDirectory, string sourcefileName, string workbookDirectory, string workbookName)
        {
            // Checking
            // // check source directoryExist
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException($"ERROR_DIRECTORY \"{sourceDirectory}\" NOT_FOUND");
            // // check source file exist
            if (!File.Exists($"{sourceDirectory}/{sourcefileName}"))
                throw new FileNotFoundException($"ERROR_FILE \"{sourceDirectory}/{sourcefileName}\" NOT_FOUND");

            // // set source file full name
            var sourceFileFullName = $"{sourceDirectory}/{sourcefileName}";

            // // workbook directory
            workbookDirectory ??= sourceDirectory;
            // // set workbook full name for overwritting use case
            workbookName ??= sourcefileName;

            // // set workbook full name
            var workbookFullName = $"{workbookDirectory}/{workbookName}";


            // Register special encoding 
            var encodingProvider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(encodingProvider);

            // open fileStream with read access and get reader
            FileStream fs = File.OpenRead(sourceFileFullName);
            IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);

            // get xlsx file as dataSet
            var conf = new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = false
                }
            };
            DataSet dataSet = excelDataReader.AsDataSet(conf);

            // create new workbook
            Workbook workbook = new(workbookFullName, "tempsheetName");

            // get array from dataset
            var tablesCount = dataSet.Tables.Count;

            var tables = new DataTable[tablesCount];
            
                dataSet.Tables.CopyTo(tables, 0);
            
            int index = -1;
            foreach (var table in tables)
            {
                index++;
                // add worksheet
                Worksheet currentWorksheet;
                if (index != 0)
                {
                    workbook.AddWorksheet(table.TableName);
                    currentWorksheet = workbook.Worksheets.Last();
                }
                else
                {
                    currentWorksheet = workbook.CurrentWorksheet;
                    currentWorksheet.SetSheetName(table.TableName);
                }
                currentWorksheet.CurrentCellDirection = Worksheet.CellDirection.ColumnToColumn;
                // mapping data from current dataTable in currentWorksheet

                // // get array from dataTable
                DataRow[] rows = new DataRow[table.Rows.Count];
                table.Rows.CopyTo(rows, 0);


                // // mapping data
                foreach (var datarow in rows.Select(r => r.ItemArray))
                {
                    foreach (var data in datarow)
                    {
                        currentWorksheet.AddNextCell(data);
                    }
                    currentWorksheet.GoToNextRow();
                }
            }

            // close reader
            excelDataReader.Close();

            // return workbook

            return workbook;

        }

        internal static void CreateProcessXLSXFile(Process process)
        {
            Console.WriteLine($"Creating XLSX file for process : {process._methodInfo.Name}");
            string WorkBookFolderName = "Workbook";
            string ProjectFolderName = "Projects";

            // Create directories
            string workbookDirectory = $"{Directory.GetCurrentDirectory()}/{WorkBookFolderName}";

            Directory.CreateDirectory(workbookDirectory);

            var directory = workbookDirectory;
            if (process.Project != null)
            {
                directory += $"/{ProjectFolderName}";
                Directory.CreateDirectory(directory);
                directory += $"/{process.Project.Name}";
                Directory.CreateDirectory(directory);
            }

            directory += $"/{process._methodInfo.Name}";
            Directory.CreateDirectory(directory);

            // get XLSX FullPath
            var wbFullName = $"{directory}/{process._methodInfo.Name}.xlsx";


            // Get or create workbook and add data
            Workbook workbook;
            if (File.Exists(wbFullName))
            {
                workbook = CreateWorkbookFromXLSXFile(directory, $"{process._methodInfo.Name}.xlsx", null, null);
                AddProcessAndSubProcessWorksheets(process, workbook);
                var worksheets = workbook.Worksheets;
                worksheets.ForEach(s =>
                {

                    s.DefaultColumnWidth = 40.25f;
                });
            }
            else
            {
                // creation du workbook
                workbook = new(wbFullName, "feuille0");

                AddProcessAndSubProcessWorksheets(process, workbook);

                var worksheets = workbook.Worksheets;
                worksheets.ForEach(s =>
                {

                    s.DefaultColumnWidth = 40.25f;
                });
                workbook.RemoveWorksheet(0);
            }

            var style = new Style() { CurrentFont = new() { Bold = true } };
            workbook.Worksheets.ForEach(s =>
            {
                s.Cells.Select(c=>c.Value).Where(c => c.CellAddress2.Row == 0 || c.CellAddress2.Column == 0).ToList().ForEach(c =>
                {
                    c.SetStyle(style);
                });
            });
            workbook.Save();
        }


        private static void AddProcessAndSubProcessWorksheets(Process process, Workbook workbook)
        {
            AddProcessWorksheet(process, workbook, -1);
        }
        private static void AddProcessWorksheet(Process process, Workbook workbook, int depth)
        {
            depth++;
            string sheet1Name = $"{process._methodInfo.Name}_STAT_{depth}";
            string sheet2Name = $"{process._methodInfo.Name}_SWVal_{depth}";

            // set header row sheet 1
            List<string> sheet1HeaderNames = new string[] { "SampleDateTime", "AverageTime", "StandartDeviation", "MinValue", "MaxValue" }.ToList();
            sheet1HeaderNames.AddRange(process.SubProcesses.Select(p => $"{p._methodInfo.Name}_MainProcessusRatio"));


            bool isNew = false;
            var firstWorksheet = workbook.Worksheets.FirstOrDefault(w => w.SheetName.Equals(sheet1Name));
            var secondWorksheet = workbook.Worksheets.FirstOrDefault(w => w.SheetName.Equals(sheet2Name));
            if (firstWorksheet == null || secondWorksheet == null)
            {
                workbook.AddWorksheet(sheet1Name);
                firstWorksheet = workbook.Worksheets.Last();
                firstWorksheet.AddCellRange(sheet1HeaderNames.AsReadOnly(), new Cell.Address(0, 0), new Cell.Address(sheet1HeaderNames.Count - 1, 0));

                workbook.AddWorksheet(sheet2Name);
                secondWorksheet = workbook.Worksheets.Last();
                isNew = true;
            }


            foreach (var sample in process.Samples)
            {

                var statistics = sample.Statistics!;
                firstWorksheet.SetCurrentRowNumber(firstWorksheet.GetLastDataRowNumber());
                firstWorksheet.GoToNextRow();
                firstWorksheet.AddNextCell(sample.SampleDateTime);
                firstWorksheet.AddNextCell(statistics.AverageTime);
                firstWorksheet.AddNextCell(statistics.StandartDeviation);
                firstWorksheet.AddNextCell(statistics.MinValue);
                firstWorksheet.AddNextCell(statistics.MaxValue);

                var subProcessusRatios = new List<double>();
                if (process.SubProcesses.Any())
                    subProcessusRatios = process.SubProcesses.Select(s => s.Samples[sample.SampleIndex]).Select(s => s.Statistics!.MainProcessusRatio!.Value).ToList();
                foreach (var ratio in subProcessusRatios)
                {
                    firstWorksheet.AddNextCell(ratio);
                }

                var stopWatchValues = sample.StopWatchValues.Select(v => (object)v).ToList();
                int columnNumber;
                if (isNew)
                {
                    columnNumber = sample.SampleIndex;
                }
                else
                {
                    var numberOfRow = secondWorksheet.GetLastRowNumber() + 1;
                    columnNumber = secondWorksheet.Cells.Count / numberOfRow;
                }
               
                secondWorksheet.AddCell(sample.SampleDateTime, columnNumber, 0);
                secondWorksheet.AddCellRange(stopWatchValues.AsReadOnly(), new Cell.Address(columnNumber, 1), new Cell.Address(columnNumber, stopWatchValues.Count));

            }


            foreach (var proc in process.SubProcesses)
            {
                AddProcessWorksheet(proc, workbook, depth);
            }
        }

        
        private static Workbook GetWorkBookProcessFromXlsx(Process process)
        {
            var processDirectory = DIRECTORY_BASE;
            processDirectory += process.Project == null ? $"/{process._methodInfo.Name}" : $"/Projects/{process.Project!.Name}/{process._methodInfo.Name}";
            var xlsxFileName = $"{process._methodInfo.Name}.xlsx";

            // if file does not exists a FileNotFoudException is thrown
            return XlsxUtils.CreateWorkbookFromXLSXFile(processDirectory, xlsxFileName, null, null);
        }
        private static List<SampleStatRow> GetSampleStatRowsFromWorkook(Workbook workbook)
        {
            List<SampleStatRow> list = new();
            var statSheets = workbook.Worksheets.Where(w => w.SheetName.Contains("STAT")).ToList();
            foreach (var sheet in statSheets)
            {
                var processName = sheet.SheetName.Split("_STAT").First();
                var cells = sheet.Cells.Select(c => c.Value);
                var firstRowData = cells.Where(c => c.CellAddress2.Row == 0).Select(c => (string)c.Value);
                var subProcessNames = firstRowData.Where(v => v.Contains("Ratio"));
                var numberOfSubProcess = subProcessNames.Count();

                var cellsData = cells.Where(c => c.CellAddress2.Row != 0).Select(c => new { c.RowNumber, c.Value });
                var dataRowCount = cellsData.Count() / firstRowData.Count();
                // iterate on row data
                IEnumerable<object> currentRawData;

                for (int i = 1; i <= dataRowCount; i++)
                {
                    currentRawData = cellsData.Where(c => c.RowNumber == i).Select(c => c.Value);
                    var dataStat = currentRawData.SkipLast(numberOfSubProcess);
                    string sampleDateTime = (string)dataStat.ElementAt(0);
                    int averageTime = Convert.ToInt32(dataStat.ElementAt(1));
                    double standartDeviation = (double)dataStat.ElementAt(2);
                    int minValue = Convert.ToInt32(dataStat.ElementAt(3));
                    int maxValue = Convert.ToInt32(dataStat.ElementAt(4));

                    var sublist = new List<SubProcessRatio>();
                    var processRatioData = currentRawData.Except(dataStat);

                    for (int j = 0; j < numberOfSubProcess; j++)
                    {
                        sublist.Add(new()
                        {
                            SubProcessName = subProcessNames.ElementAt(j).Replace("_MainProcessusRatio",""),
                            MainProcessusRatio = (double)processRatioData.ElementAt(j)
                        });
                    }

                    list.Add(new()
                    {
                        ProcessName = processName,
                        AverageTime = averageTime,
                        MaxValue = maxValue,
                        MinValue = minValue,
                        SubProcessRatio = sublist,
                        SampleDateTime = sampleDateTime,
                        StandartDeviation = standartDeviation

                    });


                }
            }
            return list;
        }

        internal static List<SampleStatRow> GetSampleStatRowsFromProcess(Process process)
        {
            return GetSampleStatRowsFromWorkook(GetWorkBookProcessFromXlsx(process));
        }

    }
}

using ExcelDataReader;
using PicoXLSX;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicoXLSX.Style;

namespace PerfObserver.XLSX
{
    internal static class XlsxUtils
    {

        /// <summary>
        /// Create a worbook from an existing template xlsx with a new workbookName
        /// null values for workbookDirectory and workbookName will create an intended to overwrite sourceFile workbook
        /// </summary>
        /// <param name="fileName"> fullPath for source file </param>
        /// <param name="workbookName"></param>
        /// <returns>Workbook</returns>
        internal static Workbook CreateWorkbookFromXLSXFile(string sourceDirectory, string sourcefileName, string? workbookDirectory, string? workbookName)
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
            var tables = new DataTable[dataSet.Tables.Count];
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

        internal static void CreateProjectXLSXFile()
        {
            string projectDirectory = $"{Directory.GetCurrentDirectory()}/Projects";
            const string fileName = "Projects.xlsx";

            Directory.CreateDirectory(projectDirectory);

            Workbook workbook = new($"{projectDirectory}/{fileName}", "projects");

            var currentWorksheet = workbook.CurrentWorksheet;
            if (!Directory.Exists($"{projectDirectory}/{fileName}"))
            {
                return;
            }

            currentWorksheet.AddNextCell("ProjectGuid");
            currentWorksheet.AddNextCell("ProjectName");
            workbook.Save();
        }


    }
}

using PerfObserver.XLSX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicoXLSX.Cell;

namespace PerfObserver.Model
{
    internal class Project
    {
        internal Guid Id { get; set; }
        internal string Name { get; set; }

        internal Project(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        internal static List<string> GetAllProjectNames()
        {
            var workbook = XlsxUtils.CreateWorkbookFromXLSXFile($"{Directory.GetCurrentDirectory()}/Projects", "Projects.xlsx", null, null);
            var projectNames = workbook.CurrentWorksheet.Cells.Values.Select(v => v).Where(c => c.ColumnNumber == 1).Select(c => (string)c.Value).Skip(1).ToList();
            return projectNames;
        }
        internal static bool Exists(string ProjectName)
        {
            return GetAllProjectNames().Any(n=> n.Equals(ProjectName));
        }
    }
}

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
    }
}

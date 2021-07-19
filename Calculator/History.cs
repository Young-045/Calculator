using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class History
    {
        private string process;
        private string result;

        public string Process { get => process; set => process = value; }
        public string Result { get => result; set => result = value; }

        public History()
        {
            Process = "";
            Result = "";
        }
        public History(string process,string result)
        {
            Process = process;
            Result = result;
        }
    }
}

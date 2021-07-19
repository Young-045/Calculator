using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class CalculateData
    {
        private string result;
        private string process;
        private bool complete;
        private bool isSingle;
        private bool isDivisorLegal;
        private bool onlyResult;
        public string Result { get => result; set => result = value; }
        public string Process { get => process; set => process = value; }
        public bool Complete { get => complete; set => complete = value; }
        public bool IsSingle { get => isSingle; set => isSingle = value; }
        public bool IsDivisorLegal { get => isDivisorLegal; set => isDivisorLegal = value; }
        public bool OnlyResult { get => onlyResult; set => onlyResult = value; }

        public CalculateData()
        {
            CalculateDataReset();
        }

        public void CalculateDataReset()
        {
            Result = "0";
            Process = "";
            Complete = false;
            IsSingle = false;
            IsDivisorLegal = true;
            OnlyResult = false;
        }
    }
}

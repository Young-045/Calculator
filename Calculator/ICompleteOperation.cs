using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    interface ICompleteOperation
    {
        void CompleteCallBack(CalculateData calculateData,string historyProcess);
       
    }
}

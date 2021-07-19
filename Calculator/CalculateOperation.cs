using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator
{
    class CalculateOperation
    {
        private int gitTest = 0;
        private string operation = "", singleProcess = "";
        private bool secNumberBegin = false, doubleContinuity = false, singleContinuity = false, invalidOperation = false;
        private float resultNum = 0, operationNum = 0;
        private Stack<string> calculatorStack = new Stack<string>();
        
        private CalculateData calculateData;

        public string Operation { get => operation; set => operation = value; }
        public bool SecNumberBegin { get => secNumberBegin; set => secNumberBegin = value; }
        public bool ContinuityOperation { get => doubleContinuity; set => doubleContinuity = value; }
        public float ResultNum { get => resultNum; set => resultNum = value; }
        public Stack<string> CalculatorStack { get => calculatorStack; set => calculatorStack = value; }
        public float OperationNum { get => operationNum; set => operationNum = value; }
      
        
        public bool InvalidOperation { get => invalidOperation; set => invalidOperation = value; }
        

        private readonly ICompleteOperation _CompleteOperation;
        public CalculateOperation(ICompleteOperation completeOperation)
        {
            _CompleteOperation = completeOperation;
            calculateData = new CalculateData();
        }

        public void ResultOperation(float num, string process)
        {
            if (CalculatorStack.Count() != 0)
            {
                if (!ContinuityOperation)
                {
                    Operation = CalculatorStack.Pop();
                    ResultNum = ReadNum(CalculatorStack.Pop());
                    OperationNum = num;
                    ContinuityOperation = true;
                }
                else
                {
                    resultNum = num;
                }
                ResultCalculate(process);
            }
            else
            {
                if (singleContinuity)
                {
                    process += "=";
                }
                else
                {
                    process = num + "=";
                }
                ResultNum = num;
                InvalidOperation = true;
                
                calculateData.Process = process;
                calculateData.Result = num + "";
                calculateData.Complete = true;
                _CompleteOperation.CompleteCallBack(calculateData,"");
                calculateData.CalculateDataReset();
            }
        }

        private void ResultCalculate(string process)
        {
            if (singleContinuity)
            {
                process += "=";
                singleContinuity = false;
            }
            else
            {
                if ((!Operation.Equals("÷")) || (Operation.Equals("÷") && OperationNum != 0))
                    process = ResultNum + Operation + OperationNum + "=";
            }
            switch (Operation)
            {
                case "+": ResultNum += OperationNum; break;
                case "-": ResultNum -= OperationNum; break;
                case "×": ResultNum *= OperationNum; break;
                case "÷":
                    if (OperationNum != 0)
                    {
                        ResultNum /= OperationNum;
                    }
                    else
                    {
                        Reset();
                        calculateData.Process = process;
                        calculateData.Result = "除数不能为零";
                        calculateData.IsDivisorLegal = false;
                        _CompleteOperation.CompleteCallBack(calculateData,"");
                        calculateData.CalculateDataReset();
                        return;
                    }
                    break;
            }
            CalculatorStack.Push(ResultNum + "");
            CalculatorStack.Push(Operation);
            SecNumberBegin = true;
           
            calculateData.Process = process;
            calculateData.Result = ResultNum + "";
            calculateData.Complete = true;
            _CompleteOperation.CompleteCallBack(calculateData, "");
            calculateData.CalculateDataReset();
        }

        public void DoubleOperation(string symbol, float num, string singleProcess)
        {
            if (ContinuityOperation)
            {
                CalculatorStack.Clear();
            }
            singleContinuity = false;
            ContinuityOperation = false;
            if (CalculatorStack.Count() == 0)
            {
                SecNumberBegin = true;
                CalculatorStack.Push(num + "");
                CalculatorStack.Push(symbol);
                if (singleContinuity)
                {
                    calculateData.Process = singleProcess + symbol;
                }
                else
                {
                    calculateData.Process = num + symbol;
                }
                //_CompleteOperation.CompleteCallBack(calculateData,"");
                calculateData.CalculateDataReset();
            }
            else
            {
                Operation = CalculatorStack.Pop();
                if (SecNumberBegin) //输入5+后修改运算符
                {
                    CalculatorStack.Push(symbol);
                    calculateData.Process = num + symbol;
                    _CompleteOperation.CompleteCallBack(calculateData, "");
                    calculateData.CalculateDataReset();
                }
                else
                {
                    ResultNum = ReadNum(CalculatorStack.Pop());
                    OperationNum = num;
                    string process = ResultNum + Operation;
                    switch (Operation)
                    {
                        case "+": ResultNum += OperationNum; break;
                        case "-": ResultNum -= OperationNum; break;
                        case "×": ResultNum *= OperationNum; break;
                        case "÷":
                            if (OperationNum != 0)
                            {
                                ResultNum /= OperationNum;
                            }
                            else
                            {
                                Reset();
                                calculateData.Process = process;
                                calculateData.Result = "除数不能为零";
                                calculateData.IsDivisorLegal = false;
                                _CompleteOperation.CompleteCallBack(calculateData,"");
                                calculateData.CalculateDataReset();
                                return;
                            }
                            break;
                    }
                    SecNumberBegin = true;
                    CalculatorStack.Push(ResultNum + "");
                    CalculatorStack.Push(symbol);
                   
                    calculateData.Process = ResultNum + symbol;
                    calculateData.Result = ResultNum + "";
                    calculateData.Complete = true;
                    _CompleteOperation.CompleteCallBack(calculateData, process + OperationNum + "=");
                    calculateData.CalculateDataReset();
                }
            }
        }

        public void SingleOperation(string symbol, float num, string process)
        {
            if (!singleContinuity)
            {
                singleProcess = num + "";
                singleContinuity = true;
            }
            switch (symbol)
            {
                case "denominator": if (num != 0) { singleProcess = "1/(" + singleProcess + ")"; num = 1 / num; } break;
                case "square": singleProcess = "sqr(" + singleProcess + ")"; num = (float)Math.Pow(num, 2); break;
                case "radical": singleProcess = "√(" + singleProcess + ")"; num = (float)Math.Sqrt(num); break;
            }
            if (CalculatorStack.Count == 0)
            {
                calculateData.Process = singleProcess;
            }
            else
            {
                string operation = calculatorStack.First();
                string firstnum = calculatorStack.Last();
                calculateData.Process = firstnum + operation + singleProcess;
            }
            calculateData.Result = num + "";
            calculateData.IsSingle = true;
            _CompleteOperation.CompleteCallBack(calculateData,"");
            calculateData.CalculateDataReset();
        }

        public void NegationOperation(float num, string process)
        {
            num *= -1;
            calculateData.Process = process;
            calculateData.Result = num + "";
            calculateData.OnlyResult = true;
            _CompleteOperation.CompleteCallBack(calculateData,"");
        }
       
        public void PersentOperation(float num)
        {
            if (CalculatorStack.Count == 0)
            {
                calculateData.Process = "0";
                calculateData.OnlyResult = true;
                _CompleteOperation.CompleteCallBack(calculateData,"");
            }
            else
            {
                num = (float)(num * 0.01);
                string operation = calculatorStack.First();
                string firstnum = calculatorStack.Last();
                calculateData.Process = firstnum + operation + num;
                calculateData.Result = num + "";
                calculateData.OnlyResult = true;
                _CompleteOperation.CompleteCallBack(calculateData,"");
            }
            calculateData.CalculateDataReset();
        }

        public void UpdateData(string process)
        {
            string[] symbol = { "+", "-", "×", "÷" };
            foreach(string sign in symbol)
            {
                if(process.IndexOf(sign)>-1)
                {
                    operation = sign;
                    break;
                }
            }
            string[] test = process.Split('+','-', '×', '÷','=');
            string operNum = test[1];
            operationNum = float.Parse(operNum);
        }

        public void Reset()
        {
            ResultNum = 0;
            OperationNum = 0;
            Operation = "";
            CalculatorStack.Clear();
            SecNumberBegin = false;
            doubleContinuity = false;
            singleContinuity = false;
            singleProcess = "";
            invalidOperation = false;
            calculateData.CalculateDataReset();
        }

        private float ReadNum(string text)
        {
            float goal;
            bool parse = float.TryParse(text, out goal);
            if (parse)
            {
                return goal;
            }
            return 0;
        }

        



    }
}

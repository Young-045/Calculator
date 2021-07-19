using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
   
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, ICompleteOperation, INotifyPropertyChanged
    {
        private double originalLeft = 0, originalTop = 0;
        private double oriWidth = 0, oriHeight = 0;
        private bool doResult = false, doSingle = false, isDivisorLegal=false, isHistoryDelete=false;
        private string resultContent="0";
        private string processContent="";
        private ObservableCollection<History> historyList = new ObservableCollection<History>();
        
        
        private CalculateOperation calculateOperation;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ObservableCollection<History> HistoryList 
        {
            get => historyList;
            set => historyList = value;         
        }
        public string ResultContent
        {
            get => resultContent;
            set 
            {
                resultContent = value;
                PropertyChanged(this,new PropertyChangedEventArgs("ResultContent"));
            } 
        }

        public string ProcessContent
        {
            get => processContent;
            set
            {
                processContent = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ProcessContent"));
            }
        }

        public bool IsHistoryDelete 
        { 
            get => isHistoryDelete;
            set { isHistoryDelete = value; PropertyChanged(this, new PropertyChangedEventArgs("IsHistoryDelete")); } 
        }

        public MainWindow()
        {
            InitializeComponent();
            calculateOperation = new CalculateOperation(this);
            DataContext = this;
            ResultContent = "0";
            ProcessContent = "";
            History history = new History("暂无历史记录！", "");
            HistoryList.Add(history);
        }

        private void Num_Click(object sender, RoutedEventArgs e)
        {
            string data = ((Button)sender).Content.ToString();
            doSingle = false;
            DoNum(data);
        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            doSingle = false;
            DoPoint();
        }
        
        private void C_Click(object sender, RoutedEventArgs e)
        {
            ResultContent = "0";
            ProcessContent = "";
            calculateOperation.Reset();
            doResult = false;
            doSingle = false;
            isDivisorLegal = false;
        }

        private void CE_Click(object sender, RoutedEventArgs e)
        {
            ResultContent = "0";
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DoBack();
        }
       
        private void SingleNumOperation_Click(object sender, RoutedEventArgs e)
        {
            string symbol = ((Button)sender).Name;
            float num = ReadNum();
            calculateOperation.SingleOperation(symbol, num, Process.Text);           
        }

        private void Persent_Click(object sender, RoutedEventArgs e)
        {
            float num = ReadNum();
            calculateOperation.PersentOperation(num);
        }

        private void Negation_Click(object sender, RoutedEventArgs e)
        {
            float num = ReadNum();
            string process = ProcessContent;
            calculateOperation.NegationOperation(num,process);
        }
        
        private void DoubleNumOperation_Click(object sender, RoutedEventArgs e)
        {
            string symbol = ((Button)sender).Content.ToString();
            float num = ReadNum();    
            if(calculateOperation.CalculatorStack.Count==0)
            {
                ProcessContent = num + symbol;
            }
            calculateOperation.DoubleOperation(symbol, num, ProcessContent);            
        }

        private void Result_Click(object sender, RoutedEventArgs e)
        {
            float num = ReadNum();
            calculateOperation.ResultOperation(num, Process.Text);           
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            HistoryButton.IsChecked = true;
            MemoryButton.IsChecked = false;
            History.Visibility = Visibility.Visible;
            Memory.Visibility = Visibility.Collapsed;
        }

        private void Memory_Click(object sender, RoutedEventArgs e)
        {
            MemoryButton.IsChecked = true;
            HistoryButton.IsChecked = false;
            History.Visibility = Visibility.Collapsed;
            Memory.Visibility = Visibility.Visible;
        }

        private void Root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyScrollViewer.Height = Root.ActualHeight - 50;
            if ((bool)BaseHistory.IsChecked)
                BaseScrollviewer.Height = BaseHisPanel.ActualHeight - 35;
            else
                BaseScrollviewer.Height = Grid.ActualHeight - 35;
            if (!(bool)TopButton.IsChecked)
            {
                if (Root.ActualWidth > 550)
                {
                    BaseHisPanel.Visibility = Visibility.Collapsed;
                    BaseHistory.IsChecked = false;
                    BaseHistory.Visibility = Visibility.Collapsed;
                    Grid.Visibility = Visibility.Visible;
                    SecPanel.Visibility = Visibility.Visible;                 
                }
                else
                {
                    SecPanel.Visibility = Visibility.Collapsed;
                    BaseHistory.Visibility = Visibility.Visible;
                }
            }
        }

        private void Top_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)TopButton.IsChecked)
            {
                originalLeft = this.Left;
                originalTop = this.Top;
                oriWidth = this.Width;
                oriHeight = this.Height;
                this.Left = 1100;
                this.Top = 100;
                this.Topmost = true;
                this.MinWidth = 300;
                this.MinHeight = 450;
                this.Width = 300;
                this.Height = 450;
                Process.Visibility = Visibility.Collapsed;
                Menu.Visibility = Visibility.Collapsed;
                MenuItemHeader.Visibility = Visibility.Collapsed;
                BaseHistory.Visibility = Visibility.Collapsed;
                BaseHistory.IsChecked = false;
                BaseHisPanel.Visibility = Visibility.Collapsed;
                Grid.Visibility = Visibility.Visible;
                SecPanel.Visibility = Visibility.Collapsed;

            }
            else
            {
                this.MinWidth = 400;
                this.MinHeight = 560;
                this.Left = originalLeft;
                this.Top = originalTop;
                this.Topmost = false;
                Process.Visibility = Visibility.Visible;
                Menu.Visibility = Visibility.Visible;
                MenuItemHeader.Visibility = Visibility.Visible;
                BaseHistory.Visibility = Visibility.Visible;
                this.Width = oriWidth;
                this.Height = oriHeight;
            }
        }

        private void HistoryDelete_Click(object sender, RoutedEventArgs e)
        {           
            HistoryList.Clear();
            IsHistoryDelete = false;           
            BaseHistoryClose();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            string input = e.Key.ToString();
            float num = ReadNum();
            string data = input.Substring(input.Length - 1);
            if ("0123456789".IndexOf(data) >= 0)
            {
                DoNum(data);
            }
            else
            {
                CalculateData calculateData = new CalculateData();
                string text = ProcessContent;
                switch (input)
                {
                    case "Add": calculateOperation.DoubleOperation("+", num, text); break;
                    case "Subtract": calculateOperation.DoubleOperation("-", num, text); break;
                    case "Multiply": calculateOperation.DoubleOperation("×", num, text); break;
                    case "Divide": calculateOperation.DoubleOperation("÷", num, text); break;
                    case "Return": calculateOperation.ResultOperation(num, text); break;
                    case "Decimal": DoPoint(); break;
                    case "Back": DoBack(); break;
                    default:break;
                }                
            }
        }

        private void BaseHistory_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)BaseHistory.IsChecked)
            {
                Grid.Visibility = Visibility.Collapsed;
                BaseHisPanel.Visibility = Visibility.Visible;
                Root.MouseLeftButtonDown += Root_MouseLeftButtonDown;
                
            }
            else
            {
                BaseHistoryClose();
            }
        }

        private void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition((IInputElement)sender);
            if (p.Y < this.ActualHeight - BaseHisPanel.ActualHeight)
            {
                BaseHistoryClose();
            }
        }

        public void BaseHistoryClose()
        {
            BaseHisPanel.Visibility = Visibility.Collapsed;
            BaseHistory.IsChecked = false;
            Grid.Visibility = Visibility.Visible;          
            Root.MouseLeftButtonDown -= Root_MouseLeftButtonDown;
        }

        private void DoNum(string data)
        {         
            doSingle = false;
            doResult = false;
            if (calculateOperation.CalculatorStack.Count() == 0)    //第一个操作数
            {
                if (calculateOperation.InvalidOperation) //直接按等号
                {
                    ResultContent = data;
                    calculateOperation.InvalidOperation = false;
                }
                else if (ResultContent.Length < 16)
                {
                    if ((!ResultContent.Equals("0"))&&!isDivisorLegal)
                    {
                        ResultContent += data;
                    }
                    else
                    {
                        ResultContent = data;
                        isDivisorLegal = false;
                    }
                }
            }
            else
            {
                if (calculateOperation.SecNumberBegin)
                {
                    ResultContent = data;
                    calculateOperation.SecNumberBegin = false;
                }
                else if (ResultContent.Length < 16)
                {
                    if (!ResultContent.Equals("0"))
                    {
                        ResultContent += data;
                    }
                    else
                    {
                        ResultContent = data;                       
                    }
                }
            }
        }

        private void DoPoint()
        {
            if (ResultContent.IndexOf(".") == -1)
            {
                if (calculateOperation.InvalidOperation || calculateOperation.SecNumberBegin) //直接按等号
                {
                    ResultContent = "0.";
                    calculateOperation.InvalidOperation = false;
                    calculateOperation.SecNumberBegin = false;
                }
                else
                {
                    ResultContent += ".";
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string menuItem = (string)((MenuItem)sender).Header;
            MenuItemHeader.Text = menuItem;
        }

        private void HistoryContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            History history = (History)((ListBox)sender).SelectedItem;
            if(history!=null)
            {
                string process = history.Process;
                ProcessContent = process;
                ResultContent = history.Result;
                calculateOperation.UpdateData(process);
                BaseHistoryClose();
            }          
        }

        private void DoBack()
        {          
            if (doResult && (!doSingle))    //输入5+1=后点击回退键，删除过程框内容
            {
                ProcessContent = "";
            }
            else if (!(doResult || doSingle))   //doResult和doSingle全为假
            {
                if (ResultContent.Length <= 1)
                {
                    ResultContent = "0";
                }
                else
                {
                    ResultContent = ResultContent.Substring(0, ResultContent.Length - 1);
                }
            }


        }

        private float ReadNum()
        {
            float goal;
            bool parse = float.TryParse(ResultContent, out goal);
            if (parse)
            {
                return goal;
            }
            return 0;
        }

       private void AddItem(string process,string result)
        {
            if(!IsHistoryDelete)
            {
                HistoryList.Clear();
                IsHistoryDelete = true;
            }

            History history = new History(process, result);
            HistoryList.Insert(0, history);
           
        }

       
        public void CompleteCallBack(CalculateData calculateData,string historyProcess)
        {
            ProcessContent = calculateData.Process;
            if (calculateData.Complete)
            {
                ResultContent = calculateData.Result;                
                doResult = true;
                if(historyProcess.Equals(""))
                {
                    historyProcess = calculateData.Process;
                }
                AddItem(historyProcess, calculateData.Result);                
            }
            if (calculateData.IsSingle)
            {
                ResultContent = calculateData.Result;             
                doSingle = true;
            }
            if(!calculateData.IsDivisorLegal)
            {
                ResultContent = calculateData.Result;               
                isDivisorLegal = true;
            }
            if(calculateData.OnlyResult)
            {
                ResultContent = calculateData.Result;              
            }
        }

       
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BACAClient
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            this.Loaded += TestWindow_Loaded;
            
        }

        ObservableCollection<PieDataItem> data = new ObservableCollection<PieDataItem>()
            {
                new PieDataItem() { Title = "s1", Value = 10 },
                //new PieDataItem() { Title = "s2", Value = 30 },
                //new PieDataItem() { Title = "s3", Value = 20 },
                //new PieDataItem() { Title = "s4", Value = 80 }
            };
        private void TestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pie1.DataSource = data;
        }

        private void AddDataButton_Click(object sender, RoutedEventArgs e)
        {
            data.Add(new PieDataItem() { Title = "s5", Value = 12.56 });
            data.Add(new PieDataItem() { Title = "s6", Value = 25 });
        }


        private void RealTimeDataChanges_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        Random rnd = new Random();
        int sliceCounter = 10;
        void timer_Tick(object sender, EventArgs e)
        {
            double action = rnd.NextDouble();
            if (action > 0.85 && data.Count > 0)
            {
                data.RemoveAt(rnd.Next(data.Count));
            }
            else if (action > 0.8)
            {
                data.Add(new PieDataItem() { Title = "slice " + sliceCounter.ToString(), Value = rnd.NextDouble() * 50 });
                sliceCounter++;
            }
            else
            {
                foreach (PieDataItem di in data)
                {
                    di.Value = rnd.NextDouble() * 50;
                }
            }
        }

        private List<string> strListx = new List<string>() { "苹果", "樱桃", "菠萝", "香蕉", "榴莲", "葡萄", "桃子", "猕猴桃" };
        private List<string> strListy = new List<string>() { "13", "75", "60", "38", "97", "22", "39", "80" };

        private List<DateTime> LsTime = new List<DateTime>()
            {
               new DateTime(2012,1,1),
               new DateTime(2012,2,1),
               new DateTime(2012,3,1),
               new DateTime(2012,4,1),
               new DateTime(2012,5,1),
               new DateTime(2012,6,1),
               new DateTime(2012,7,1),
               new DateTime(2012,8,1),
               new DateTime(2012,9,1),
               new DateTime(2012,10,1),
               new DateTime(2012,11,1),
               new DateTime(2012,12,1),
            };
        private List<string> cherry = new List<string>() { "33", "75", "60", "98", "67", "88", "39", "45", "13", "22", "45", "80" };
        private List<string> pineapple = new List<string>() { "13", "34", "38", "12", "45", "76", "36", "80", "97", "22", "76", "39" };
    }


    public class PieDataItem : INotifyPropertyChanged
    {
        public string Title { get; set; }


        private double _value;
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

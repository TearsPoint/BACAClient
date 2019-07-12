using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetSun.MDT
{
    public class PieChartList : ObservableCollection<PieItemData>
    {

        public PieChartList()
        {
            PieItemData a1 = new PieItemData() { OrgName = "内科", OrgId = 13, Count = 20 };
            a1.PieDataSource.Add(new PieItemDataItem() { Title = "已预警未会诊人数", Value = 4 });
            a1.PieDataSource.Add(new PieItemDataItem() { Title = "已会诊人数", Value = 3 });
            a1.PieDataSource.Add(new PieItemDataItem() { Title = "普通患者人数", Value = 20 - 3 - 4 });
            Add(a1);

            PieItemData a2 = new PieItemData() { OrgName = "外科", OrgId = 13, Count = 100 };
            a2.PieDataSource.Add(new PieItemDataItem() { Title = "已预警未会诊人数", Value = 4 });
            a2.PieDataSource.Add(new PieItemDataItem() { Title = "已会诊人数", Value = 3 });
            a2.PieDataSource.Add(new PieItemDataItem() { Title = "普通患者人数", Value = 100 - 3 - 4 });
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
            Add(a2);
        }
    }

    public class PieItemData
    {
        public string OrgName { get; set; }
        public int OrgId { get; set; }
        public int Count { get; set; }
        public string HeaderDesc { get { return string.Format("{0}({1})", OrgName, Count.ToString()); } }

        ObservableCollection<PieItemDataItem> _PieDataSource;
        public ObservableCollection<PieItemDataItem> PieDataSource
        {
            get
            {
                if (_PieDataSource == null) _PieDataSource = new ObservableCollection<PieItemDataItem>();
                return _PieDataSource;
            }
        } 
    }


    public class PieItemDataItem : INotifyPropertyChanged
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

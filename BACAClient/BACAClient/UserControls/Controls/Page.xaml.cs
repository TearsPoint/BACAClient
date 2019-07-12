namespace BACAClient.UserControls.Controls
{
    using Common;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class Page : UserControl
    {
        private int PageSize = PageUtils.GetPageSize(); 
        public event ChangedEventHandler PagerIndexChanged;

        public Page()
        {
            this.InitializeComponent();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.PageCount < 2)
                {
                    base.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.SetPage();
                }
            }
            catch
            {
            }
        }
         
        private void Jump_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string text = this.Jump.Text;
                if (!string.IsNullOrEmpty(text) && (e.Key == Key.Return))
                {
                    int pageIndex = this.PageIndex;
                    this.PageIndex = int.Parse(text);
                    if (this.PageIndex > this.PageCount)
                    {
                        this.PageIndex = this.PageCount;
                    }
                    else if (this.PageIndex <= 0)
                    {
                        this.PageIndex = 1;
                    }
                    if (this.PageIndex != pageIndex)
                    {
                        this.PagerIndexChanged(this.PageIndex);
                    }
                }
            }
            catch
            {
            }
        }

        private void PageDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PagerIndexChanged(this.PageIndex + 1);
            }
            catch
            {
            }
        }

        private void PageEnd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PagerIndexChanged(this.PageCount);
            }
            catch
            {
            }
        }

        private void PageFirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PagerIndexChanged(1);
            }
            catch
            {
            }
        }

        private void PageUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.PagerIndexChanged(this.PageIndex - 1);
            }
            catch
            {
            }
        }

        public void SetPage()
        {
            try
            {
                if (this.PageCount == 1)
                {
                    this.PageInfo.Visibility = Visibility.Hidden;
                    this.PageOperat.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.PageInfo.Text = string.Format("当前页 {0} / {1}", this.PageIndex, this.PageCount);
                    if (this.PageIndex <= 1)
                    {
                        this.First.IsEnabled = false;
                        this.Up.IsEnabled = false;
                    }
                    else if (this.PageIndex >= this.PageCount)
                    {
                        this.Down.IsEnabled = false;
                        this.End.IsEnabled = false;
                    }
                }
            }
            catch
            {
            }
        }
        
        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                e.Handled = new Regex("[^0-9-]+").IsMatch(e.Text);
            }
            catch
            {
            }
        }

        public int PageCount { get; set; }

        public int PageIndex { get; set; }

        public delegate void ChangedEventHandler(int pageindex);
    }
}


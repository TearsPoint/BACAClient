namespace BACAClient.Template.HisDetail
{
    using BACAClient.Model;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;

    public partial class ReInfo : UserControl
    {
        private string LngId = string.Empty;
        private int TypeId = 0;
        public event ChangedEventHandler gotokdp;

        public ReInfo()
        {
            this.InitializeComponent();
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            this.gotokdp(this.TypeId, this.LngId);
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            base.Width = this.width;
            string reTitle = string.Empty;
            if (this.model == null)
            {
                reTitle = string.Format("{0} ({1})", this.knowledge.Key1, this.knowledge.CategoryName);
                this.TypeId = this.knowledge.TypeId;
                this.LngId = this.knowledge.LngId.ToString();
            }
            else
            {
                reTitle = this.model.ReTitle;
                this.index = this.model.RecordIndex;
                this.TypeId = this.model.ReTypeId;
                this.LngId = this.model.ReLngId;
            }
            this.recordindex.Text = this.index.ToString();
            base.ToolTip = reTitle;
            this.ReTitle.Text = reTitle;
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            this.grid.Background = new BrushConverter().ConvertFromInvariantString("#dddddd") as Brush;
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            this.grid.Background = Brushes.White;
        }
        
        public int index { get; set; }

        public Knowledge knowledge { get; set; }

        public BACAClient.Model.Relational model { get; set; }

        public double width { get; set; }

        public delegate void ChangedEventHandler(int TypeId, string LngId);
    }
}


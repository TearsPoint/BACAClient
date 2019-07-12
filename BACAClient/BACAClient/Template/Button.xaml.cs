namespace BACAClient.Template
{
    using BACAClient.Controllers;
    using Base;
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
    using System.Windows.Media.Imaging;

    public partial class Button : UserControl
    {
        private string backgroud = string.Empty;

        public event ChangedEventHandler Click;

        public Button()
        {
            this.InitializeComponent();
            //EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Button));

            this.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
            this.MouseEnter += This_MouseEnter;
            this.MouseLeave += This_Leave;
        }

        int _state = 0;
        object _sender;
        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _state += 1;
            _sender = sender;
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_state == 1 && _sender == sender)
                This_Click(sender, e);
            _state = 0;
        }

        public void Assignment()
        {
            if (this.Hover.Contains(".") && !this.Hover.Contains("pack://application:,,,"))
            {
                this.Hover = string.Format("pack://application:,,,{0}", this.Hover);
            }
        }

        public void SetBackGroud()
        {
            try
            {
                if (this.BackGround.Contains("."))
                {
                    if (!this.BackGround.Contains("pack://application:,,,"))
                    {
                        this.BackGround = string.Format("pack://application:,,,{0}", this.BackGround);
                    }
                    ImageBrush brush1 = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(this.BackGround))
                    };
                    base.Background = brush1;
                }
                else
                {
                    base.Background = new BrushConverter().ConvertFromInvariantString(this.BackGround) as Brush;

                }
            }
            catch (Exception ex)
            {
                OpenWindows.Prompt(ex.CreateErrorMsg());
            }
        }

        private void This_Click(object sender, MouseButtonEventArgs e)
        {
            if (this.BackGround.Contains("."))
            {
                ImageBrush brush1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(this.BackGround))
                };
                base.Background = brush1;
            }
            else
            {
                base.Background = new BrushConverter().ConvertFromInvariantString(this.BackGround) as Brush;
            }
            if (Click != null)
                this.Click(this.TypeName);
        }

        private void This_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.BackGround))
            {
                this.Assignment();
            }
        }

        private void This_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Hover.Contains("."))
            {
                ImageBrush brush1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(this.Hover))
                };
                base.Background = brush1;
            }
            else
            {
                base.Background = new BrushConverter().ConvertFromInvariantString(this.Hover) as Brush;
            }
        }

        private void This_Leave(object sender, MouseEventArgs e)
        {
            SetBackGroud();
        }

        private void This_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.BackGround.Contains("."))
            {
                ImageBrush brush1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(this.BackGround))
                };
                base.Background = brush1;
            }
            else
            {
                base.Background = new BrushConverter().ConvertFromInvariantString(this.BackGround) as Brush;
            }
        }

        public string BackGround
        {
            get
            {
                return this.backgroud;
            }
            set
            {
                this.backgroud = value;
                this.SetBackGroud();
            }
        }

        public string Enter { get; set; }

        public string Hover { get; set; }

        public string TypeName { get; set; }

        public delegate void ChangedEventHandler(string Name);
    }
}


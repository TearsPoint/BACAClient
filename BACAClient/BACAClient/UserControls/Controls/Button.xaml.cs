namespace BACAClient.UserControls.Controls
{
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
        }

        private void _Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.BackGround.Contains("."))
                {
                    ImageBrush brush1 = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri(this.BackGround))
                    };
                    base.Background = brush1;
                }
                else
                {
                    base.Background = new BrushConverter().ConvertFromInvariantString(this.BackGround) as Brush;
                }
                this.Click(this.TypeName);
            }
            catch
            {
            }
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.BackGround) && (this.Hover.Contains(".") && !this.Hover.Contains("pack://application:,,,")))
                {
                    this.Hover = string.Format("pack://application:,,,{0}", this.Hover);
                }
            }
            catch
            {
            }
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.Hover.Contains("."))
                {
                    ImageBrush brush1 = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri(this.Hover))
                    };
                    base.Background = brush1;
                }
                else
                {
                    base.Background = new BrushConverter().ConvertFromInvariantString(this.Hover) as Brush;
                }
            }
            catch
            {
            }
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.BackGround.Contains("."))
                {
                    ImageBrush brush1 = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri(this.BackGround))
                    };
                    base.Background = brush1;
                }
                else
                {
                    base.Background = new BrushConverter().ConvertFromInvariantString(this.BackGround) as Brush;
                }
            }
            catch
            {
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
                    ImageBrush brush1 = new ImageBrush {
                        ImageSource = new BitmapImage(new Uri(this.BackGround))
                    };
                    base.Background = brush1;
                }
                else
                {
                    base.Background = new BrushConverter().ConvertFromInvariantString(this.BackGround) as Brush;
                }
            }
            catch
            {
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


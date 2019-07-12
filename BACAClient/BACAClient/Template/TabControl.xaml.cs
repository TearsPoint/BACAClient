namespace BACAClient.Template
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

    public partial class TabControl : UserControl
    {
        public bool isClick = false;
        private string url = "pack://application:,,,/BACAClient;component/Images/MainWindow/nav";
        public event ChangedEventHandler Click;

        public Brush _backGround { get; set; }
        public TabControl()
        {
            this.InitializeComponent();

            this.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
            this.MouseEnter += _MouseEnter;
            this.MouseLeave += _MouseLeave;
            _backGround = this.Background;
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
                _Click(sender, e);
            _state = 0;
        }


        private void _Click(object sender, MouseButtonEventArgs e)
        {
            OnHover();
            if (this.Click != null)
                this.Click(int.Parse(this.Type), true);
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            this.NavImg.Source = new BitmapImage(new Uri(string.Format("{0}/h{1}.png", this.url, this.Type)));
            this.TxtName.Text = this.Title;
        }

        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            _backGround = this.Background;
            OnHover();
        }

        private void OnHover()
        {
            ImageBrush brush1;
            if (!string.IsNullOrEmpty(this.Hover))
            {
                brush1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(this.Hover))
                };
            }
            else
            {
                brush1 = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(this.url + "/top_hoverbkcolor.png")),
                    Stretch = Stretch.Fill
                };
            }

            base.Background = brush1;
        }

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            base.Background = _backGround; //new BrushConverter().ConvertFromInvariantString("#3399cc") as Brush;
        }

        public string Title { get; set; }

        public string Type { get; set; }

        public delegate void ChangedEventHandler(int TypeId, bool IsClick);


        public double ImgWidth
        {
            get { return NavImg.Width; }
            set
            {
                NavImg.Width = value;
            }
        }

        public double ImgHeight
        {
            get { return NavImg.Height; }
            set { NavImg.Height = value; }
        }

        bool _IsOrientationHorizontal = false;
        public bool IsOrientationHorizontal
        {
            get { return _IsOrientationHorizontal; }
            set
            {
                _IsOrientationHorizontal = value;
                if (value)
                    _sp.Orientation = Orientation.Horizontal;
                else
                    _sp.Orientation = Orientation.Vertical;

            }
        }

        public System.Windows.Thickness TextMargin
        {
            get
            {
                return TxtName.Margin;
            }
            set { TxtName.Margin = value; SetValue(TextMarginProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register(
    "TextMargin", typeof(Thickness), typeof(TabControl),
    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTextMarginChanged)));

        private static void OnTextMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        public string Hover { get; set; }

    }
}


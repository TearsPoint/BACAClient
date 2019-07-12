namespace BACAClient.OtherWindow
{
    using BACAClient.Model;
    using BACAClient.Template;
    using BACAClient.UserControls.Controllers;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    public partial class Feedback : Window
    {
        private BACAClient.Model.FeedbackType feedbacktype = new BACAClient.Model.FeedbackType(); 
        private string type = string.Empty; 

        public Feedback()
        {
            this.InitializeComponent();
        }
        
        private void _Loaded(object sender, RoutedEventArgs e)
        {
            string str = this.ShowWindowInfo();
            this.WindowInfo.Text = str;
            this.WindowInfo.ToolTip = str;
        }

        private void Add_Click(string Name)
        {
            try
            {
                char[] trimChars = new char[] { ',' };
                this.type = this.type.TrimEnd(trimChars);
                MESSAGE message = new MESSAGE();
                if (string.IsNullOrEmpty(this.type))
                {
                    OpenWindows.Prompt(message.FeedbackTypeIsNull);
                }
                else if (string.IsNullOrEmpty(this.Description.Text))
                {
                    OpenWindows.Prompt(message.DescriptionNotNull);
                }
                else
                {
                    this.model.Type = this.type;
                    this.model.Description = this.Description.Text;
                    if (UserActionUtils.WriteFeedback(this.model))
                    {
                        base.Close();
                    }
                }
            }
            catch
            {
            }
        }

        private void Close_Click(string Name)
        {
            base.Close();
        }

        private void Data_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = this.Data.IsChecked;
            bool flag2 = true;
            if ((isChecked.GetValueOrDefault() == flag2) ? isChecked.HasValue : false)
            {
                this.type = this.type + this.feedbacktype.DataError + ",";
            }
            else
            {
                this.type = this.type.Replace(this.feedbacktype.DataError, string.Empty);
            }
        }
        
        private void Program_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = this.Program.IsChecked;
            bool flag2 = true;
            if ((isChecked.GetValueOrDefault() == flag2) ? isChecked.HasValue : false)
            {
                this.type = this.type + this.feedbacktype.ProgramError + ",";
            }
            else
            {
                this.type = this.type.Replace(this.feedbacktype.ProgramError, string.Empty);
            }
        }

        public string ShowWindowInfo()
        {
            BACAClient.Model.WindowInfo info = new BACAClient.Model.WindowInfo();
            string str = string.Format("{0}:{1};", info.PageName, this.model.PageName);
            if (!string.IsNullOrEmpty(this.model.SearchType))
            {
                str = str + string.Format(" {0}:{1};", info.SearchType, this.model.SearchType);
            }
            if (!string.IsNullOrEmpty(this.model.SearchInfo))
            {
                str = str + string.Format(" {0}:{1};", info.SearchInfo, this.model.SearchInfo);
            }
            if (!string.IsNullOrEmpty(this.model.Category))
            {
                str = str + string.Format(" {0}:{1};", info.Category, this.model.Category);
            }
            if (!string.IsNullOrEmpty(this.model.TypeID))
            {
                str = str + string.Format(" {0}:{1};", info.TypeID, this.model.TypeID);
            }
            if (!string.IsNullOrEmpty(this.model.LngId))
            {
                str = str + string.Format(" {0}:{1};", info.LngId, this.model.LngId);
            }
            if (!string.IsNullOrEmpty(this.model.Knowledge))
            {
                str = str + string.Format(" {0}:{1};", info.Knowledge, this.model.Knowledge);
            }
            if (!string.IsNullOrEmpty(this.model.NodeName))
            {
                str = str + string.Format(" {0}:{1};", info.NodeName, this.model.NodeName);
            }
            if (this.model.PageIndex > 0)
            {
                str = str + string.Format(" {0}:{1};", info.PageIndex, this.model.PageIndex);
            }
            return str;
        }

        private void Suggest_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = this.Suggest.IsChecked;
            bool flag2 = true;
            if ((isChecked.GetValueOrDefault() == flag2) ? isChecked.HasValue : false)
            {
                this.type = this.type + this.feedbacktype.ImproveSuggest + ",";
            }
            else
            {
                this.type = this.type.Replace(this.feedbacktype.ImproveSuggest, string.Empty);
            }
        }
        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                base.DragMove();
            }
            catch
            {
            }
        }

        public BACAClient.Model.Feedback model { get; set; }
    }
}


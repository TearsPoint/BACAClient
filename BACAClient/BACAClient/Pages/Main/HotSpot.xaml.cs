namespace BACAClient.Pages.Main
{
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.Repository;
    using BACAClient.Template;
    using Base;
    using fastJSON;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;

    public partial class HotSpot : BasePage
    {
        public event ChangedEventHandler gotokdp;

        public event ChangedEvent MoreInfo;

        public HotSpot()
        {
            this.InitializeComponent();
            _sp.Children.Clear();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void LoadData()
        {
            string category = string.Empty;
            if (this.type == 2)
            {
                category = this.UserDep;
            }
            DynamicJson courseList = RestRC.BACARepository.Request(ApiList.courseList, string.Empty).AsDynamicJson();
            dynamic categoryList = RestRC.BACARepository.Request(ApiList.categoryList, string.Empty).ToDynamicJson().list;

            if (categoryList != null)
            {
                foreach (DynamicJson item in categoryList)
                {
                    IndexInfo catelogElement = new IndexInfo() { model = item };
                    catelogElement.Title.Content = item.GetStr("title");

                    foreach (var course in courseList.Get("list") as IEnumerable<object>)
                    {
                        catelogElement._wpCourseItems.Children.Add(new CourseItem());
                    }

                    _sp.Children.Add(catelogElement);
                }
            }
        }

        public void goTokdp(BACAClient.Model.GoodAndHot model)
        {
            this.gotokdp(model);
        }

        private void More1_Click(object sender, RoutedEventArgs e)
        {
            //this.MoreInfo(this.Title1.Content.ToString());
        }

        private void More2_Click(object sender, RoutedEventArgs e)
        {
            //this.MoreInfo(this.Title2.Content.ToString());
        }

        public string Name1 { get; set; }

        public string Name2 { get; set; }

        public int type { get; set; }

        public string UserDep { get; set; }

        public delegate void ChangedEvent(string Type);

        public delegate void ChangedEventHandler(GoodAndHot model);
    }
}


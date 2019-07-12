namespace BACAClient.Pages.Main
{
    using BACAClient.Common;
    using BACAClient.Header;
    using BACAClient.Model;
    using BACAClient.OtherWindow;
    using BACAClient.Repository;
    using BACAClient.Template;
    using BACAClient.UserControls.Controllers;
    using Base;
    using Base.Ex;
    using Base.IO;
    using fastJSON;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class Index : BasePage
    {
        private CacheParameterName cache = new CacheParameterName();
        private MESSAGE message = new MESSAGE();
        private InterfacesUsers model = new InterfacesUsers();
        private string PageName = "首页";
        private PageSwitch pageswitch = new PageSwitch();
        private int type = 1;

        public Index()
        {
            this.InitializeComponent();
            this.Loaded += _Loaded;
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
            LoadData();
            this.model = CacheHelper.GetInterfacesUsersModel();
            BACAClient.Pages.Main.Announcement announcement = new BACAClient.Pages.Main.Announcement();
            //this.Right.Content = announcement;
            this.Knowledge();
        }

        /// <summary>
        /// 从运行目录上一级的Resource文件夹下取Images
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ImageSource> RollImages()
        {
            yield return AppResources.GetImageSource(@"Images\rollimg\img1.png");
            yield return AppResources.GetImageSource(@"Images\rollimg\img2.png");
            yield return AppResources.GetImageSource(@"Images\rollimg\img3.png");
            yield return AppResources.GetImageSource(@"Images\rollimg\img4.png");
            yield return AppResources.GetImageSource(@"Images\rollimg\img5.png");
            yield return AppResources.GetImageSource(@"Images\rollimg\img6.png");
        }

        public void Refresh()
        {
            #region old
            List<BitmapImage> ls_adv_img = new List<BitmapImage>();
            try
            {
                foreach (BitmapImage a in RollImages())
                {
                    BitmapImage img;
                    try
                    {
                        img = a;
                    }
                    catch (Exception ex)
                    {
                        img = new BitmapImage();
                    }
                    ls_adv_img.Add(img);
                }
            }
            catch (Exception ex)
            {

            }
            this._rollImg.ls_images = ls_adv_img;
            this._rollImg.Begin();


            #endregion

            //try
            //{
            //    foreach (BitmapImage a in RollImages())
            //    {
            //        BitmapImage img;
            //        try
            //        {
            //            _rollImg.AddImage(a);
            //            _rollImg.OnTouchDownEvent += _rollImg_OnTouchDownEvent;
            //        }
            //        catch (Exception ex)
            //        {
            //            img = new BitmapImage();
            //        } 
            //    }
            //}
            //catch (Exception ex)
            //{

            //}

        }

        private void LoadData()
        {
            _sp.Children.Clear();
            DynamicJson courseList = RestRC.BACARepository.Request(ApiList.courseList, string.Empty).AsDynamicJson();
            dynamic categoryList = RestRC.BACARepository.Request(ApiList.categoryList, string.Empty).ToDynamicJson().list;

            if (categoryList != null)
            {
                foreach (DynamicJson category in categoryList)
                {
                    if (category.GetStr("type") != "1") continue;
                    IndexInfo catelogElement = new IndexInfo() { model = category };
                    catelogElement.Title.Content = category.GetStr("title");

                    foreach (DynamicJson course in courseList.GetChild<List<object>>("list"))
                    {
                        if (category.GetStr("id") == course.GetStr("basecoursecategoryid"))
                        {
                            CourseItem ci = new CourseItem();
                            ci._imgCourse.Source = AppResources.GetImageSource(course.GetStr("coverimg"));
                            ci._imgCourse.Tag = course.GetStr("Id");
                            ci._imgTeacher.Source = AppResources.GetImageSource(course.GetChild<DynamicJson>("teacherEntity").GetStr("headimg"));
                            ci._imgTeacher.Tag = course.GetStr("teacherid"); 
                            ci._tblDesc.Text = string.Format("教师:{0}\r\n{1}", course.GetChild<DynamicJson>("teacherEntity").GetStr("name"), course.GetStr("describeshort"));
                            catelogElement._wpCourseItems.Children.Add(ci);
                            ci._imgCourse.PreviewMouseLeftButtonUp += _imgCourse_PreviewMouseLeftButtonUp;
                            ci._imgTeacher.PreviewMouseLeftButtonUp += _imgTeacher_PreviewMouseLeftButtonUp;
                        }
                    }
                    _sp.Children.Add(catelogElement);
                }
            }
        }

        private void _imgTeacher_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CacheParameterName name = new CacheParameterName();
            CacheHelper.SetCache(CacheParameterName.courseId, (sender as Image).Tag.ToStringEx());
            base.ParentWindow.PageSwitch(this.pageswitch.TeacherPage);
        }

        private void _imgCourse_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CacheParameterName name = new CacheParameterName();
            CacheHelper.SetCache(CacheParameterName.teacherId, (sender as Image).Tag.ToStringEx());
            base.ParentWindow.PageSwitch(this.pageswitch.CoursePage);

        }

        private void _rollImg_OnTouchDownEvent(UIElement view, int index)
        {

        }

        public void ChangeChildren()
        {

        }

        private void data_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.RapidRetrieval();
            }
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            BACAClient.Model.Feedback model = new BACAClient.Model.Feedback
            {
                PageName = this.PageName
            };
            OpenWindows.Feedback(model);
        }

        private void gotokdp(GoodAndHot model)
        {
            BACAClient.Model.Knowledge modelByTypeidAndLngid = new Model.Knowledge(); //todo
            if (modelByTypeidAndLngid.TypeId == 3)
            {
                new PDF { Id = modelByTypeidAndLngid.Id, TypeId = modelByTypeidAndLngid.TypeId }.Show();
            }
            else
            {
                CacheParameterName name = new CacheParameterName();
                CacheHelper.SetCache(name.HistoryIndex, string.Empty);
                CacheHelper.SetCache(name.Id, modelByTypeidAndLngid.Id.ToString());
                base.ParentWindow.PageSwitch(this.pageswitch.kdp);
            }
        }

        public void Knowledge()
        {
            //if (!string.IsNullOrEmpty(this.model.DepName) || (this.type != 2))
            //{
            //    string str = "近期热门";
            //    string str2 = "点赞推荐";
            //    if (this.type == 2)
            //    {
            //        str = "热门";
            //        str2 = "点赞";
            //    }
            //    BACAClient.Pages.Main.HotSpot spot = new BACAClient.Pages.Main.HotSpot
            //    {
            //        UserDep = this.model.DepName,
            //        type = this.type,
            //        Name1 = str,
            //        Name2 = str2
            //    };
            //    spot.gotokdp += new BACAClient.Pages.Main.HotSpot.ChangedEventHandler(this.gotokdp);
            //    spot.MoreInfo += new BACAClient.Pages.Main.HotSpot.ChangedEvent(this.moreInfo);
            //    this.Left.Content = spot;
            //}
        }

        private void moreInfo(string type)
        {
            MoreHotSpot spot = new MoreHotSpot
            {
                UserDep = this.model.DepName,
                Type = type
            };
            spot.gotokdp += new MoreHotSpot.ChangedEventHandler(this.gotokdp);
            //this.Left.Content = spot;
            this.PageName = "首页更多";
        }

        public void RapidRetrieval()
        {
            CacheHelper.SetCache(this.cache.IsReturn, SystemEnum.ISRETURN.NO.ToString());
            CacheHelper.SetCache(this.cache.keyword, this.keyword.Text);
            base.ParentWindow.PageSwitch(this.pageswitch.RapidRetrieval);
        }

        private void Search_Click(string Name)
        {
            //this.RapidRetrieval();
        }

        private void TwoNav_Click(string Name)
        {
            //this.nav1.BorderThickness = new Thickness(0.0);
            //this.nav2.BorderThickness = new Thickness(0.0);
            //string str = Name.ToLower();
            //if (!(str == "nav1"))
            //{
            //    if (str == "nav2")
            //    {
            //        if (string.IsNullOrEmpty(this.model.UserId))
            //        {
            //            this.nav1.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
            //            OpenWindows.Prompt(this.message.NotLogin);
            //        }
            //        else
            //        {
            //            this.nav2.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
            //            BACAClient.Pages.Main.Collection collection = new BACAClient.Pages.Main.Collection
            //            {
            //                model = this.model
            //            };
            //            collection.gotokdp += new BACAClient.Pages.Main.Collection.ChangedEvent(this.gotokdp);
            //            this.Right.Content = collection;
            //            this.type = 2;
            //            this.ChangeChildren();
            //        }
            //    }
            //}
            //else
            //{
            //    this.nav1.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
            //    this.Right.Content = new BACAClient.Pages.Main.Announcement();
            //    this.type = 1;
            //    this.ChangeChildren();
            //}
        }
    }
}


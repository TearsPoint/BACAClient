using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACAClient.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiList : RestfulApiList
    {
        /// <summary>
        /// 课程分类
        /// http://api.pigbanker.com/bankProgramme/api/course/categoryList
        /// </summary>
        public const string categoryList = "course/categoryList";

        /// <summary>
        /// 课程列表接口 
        /// http://api.pigbanker.com/bankProgramme/api/course/courseList
        /// teacherId	false	请求URL	string		教师ID号
        /// baseCategoryId	false	请求URL string 基础分类ID
        /// ageCategoryId	false	请求URL string 年龄分类ID
        /// </summary>
        public const string courseList = "course/courseList";

        /// <summary>
        /// 课程详情
        /// http://api.pigbanker.com/bankProgramme/api/course/courseInfo?courseId={courseId}
        /// </summary>
        public const string courseInfo = "course/courseInfo?courseId={courseId}";

        /// <summary>
        /// 教师列表接口
        /// http://api.pigbanker.com/bankProgramme/api/course/teacherList
        /// </summary>
        public const string teacherList = "course/teacherList";

        /// <summary>
        /// 教师详情接口
        /// course/teacherInfo?teacherId={teacherId}
        /// demo  : IDictionary<string, string> urlParas = new Dictionary<string, string>();
        /// </summary>
        public const string teacherInfo = "course/teacherInfo?teacherId={teacherId}";
    }
}

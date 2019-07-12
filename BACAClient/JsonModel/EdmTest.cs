using Base;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace JsonModel
{

    /// <summary>
    /// 数据模型
    /// </summary>
    [Table(Name = "mdt.Test")] 
    [Seq(Name = "mdt.Test_SEQ")]
    public class DtoTest
    {
        //ID number(10,0) NOT NULL,
        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "ID", IsPrimaryKey = true)]
        public int Id { get; set; }
        //ICDCodePre4 nvarchar2(255) NULL ,  --四位诊断代码及治疗方式
        [Column]
        public string ICDCodePre4 { get; set; }
        //CureWay nvarchar2(255) NULL,  --治疗方式
        [Column]
        public string CureWay { get; set; }
        //Dname Nvarchar2(255) Null ,      --诊断名称
        [Column]
        public string Dname { get; set; }
        //PersonTime NUMBER(18,8) NULL ,     --人次
        [Column]
        public double PersonTime { get; set; }
        //TotalAmount number(18,8) NULL ,        --总金额
        [Column]
        public double TotalAmount { get; set; }
        //CapitaCost number(18,8) NULL , --人均费用
        [Column]
        public double CapitaCost { get; set; }
        //DrugsAmount number(18,8) NULL , --药品金额
        [Column]
        public double DrugsAmount { get; set; }
        //DrugsProportion number(18,8) NULL , --药品比例
        [Column]
        public double DrugsProportion { get; set; }
        //MaterialAmount float (53) NULL ,   --材料金额
        [Column]
        public double MaterialAmount { get; set; }
        //MaterialProportion float (53) NULL ,
        [Column]
        public double MaterialProportion { get; set; }
        //CostStandard50Percent float (53) NULL , --费用定额控制标准 Cost quota of Control Standards   50% , 80% , 100%
        [Column]
        public double CostStandard50Percent { get; set; }
        //CostStandard80Percent float (53) NULL ,
        [Column]
        public double CostStandard80Percent { get; set; }
        //Coststandard100percent Float(53) Null ,
        [Column]
        public double Coststandard100percent { get; set; }
        //Isdeleted Number(1,0) Default(0) Not Null,
        [Column]
        public int Isdeleted { get; set; }
        //RowVersion date default(sysdate) not null 
        [Column]
        public DateTime RowVersion { get; set; }
    }

    /// <summary>
    /// 签名条件
    /// </summary>
    [Serializable]
    [DataContract]
    public class DtoQMCondition
    {
        /// <summary>
        /// 
        /// </summary>
        public DtoQMCondition()
        {
            FQM = "";
            KQM = "";
            FQMP = "";
            FQMPDate = Convert.ToDateTime("1900-01-01 00:00");
            FQMDate = Convert.ToDateTime("1900-01-01 00:00");
            KQMDate = Convert.ToDateTime("1900-01-01 00:00");

            MonthWorkPlanKPZ = "";
            MonthWorkPlanFPZ = "";
            QuarterWorkPlanFPZ = "";
            QuarterSummaryFPZ = "";
            QuarterSummaryKPZ = "";
        }

        /// <summary>
        /// 护理部签名
        /// </summary>
        [DataMember]
        public string FQM { get; set; }

        /// <summary>
        /// 科护士长签名
        /// </summary>
        [DataMember]
        public string KQM { get; set; }

        /// <summary>
        /// 工作计划护理部签名
        /// </summary>
        [DataMember]
        public string FQMP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string MonthWorkPlanKPZ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string MonthWorkPlanFPZ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string QuarterWorkPlanFPZ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string QuarterSummaryFPZ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string QuarterSummaryKPZ { get; set; }

        /// <summary>
        /// 工作计划护理部签名日期
        /// </summary>
        [DataMember]
        public DateTime FQMPDate { get; set; }

        /// <summary>
        /// 护理部签名日期
        /// </summary>
        [DataMember]
        public DateTime FQMDate { get; set; }

        /// <summary>
        /// 科护士长签名日期
        /// </summary>
        [DataMember]
        public DateTime KQMDate { get; set; }
    }

}

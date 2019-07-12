namespace BACAClient.Model
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class KnowledgeBase : DbContext
    {
        public KnowledgeBase() : base("name=KnowledgeBase")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        public virtual DbSet<BACAClient.Model.__MigrationHistory> __MigrationHistory { get; set; }

        public virtual DbSet<BACAClient.Model.Announcement> Announcement { get; set; }

        public virtual DbSet<BACAClient.Model.Category> Category { get; set; }

        public virtual DbSet<BACAClient.Model.Collection> Collection { get; set; }

        public virtual DbSet<BACAClient.Model.Department> Department { get; set; }

        public virtual DbSet<BACAClient.Model.Feedback> Feedback { get; set; }

        public virtual DbSet<BACAClient.Model.Good> Good { get; set; }

        public virtual DbSet<BACAClient.Model.Good_top> Good_top { get; set; }

        public virtual DbSet<BACAClient.Model.Good_top_categoryid> Good_top_categoryid { get; set; }

        public virtual DbSet<BACAClient.Model.HotSpot> HotSpot { get; set; }

        public virtual DbSet<BACAClient.Model.HotSpot_top> HotSpot_top { get; set; }

        public virtual DbSet<BACAClient.Model.HotSpot_top_categoryid> HotSpot_top_categoryid { get; set; }

        public virtual DbSet<BACAClient.Model.ICD> ICD { get; set; }

        public virtual DbSet<BACAClient.Model.Knowledge> Knowledge { get; set; }

        public virtual DbSet<BACAClient.Model.Lab> Lab { get; set; }

        public virtual DbSet<BACAClient.Model.Relational> Relational { get; set; }

        public virtual DbSet<BACAClient.Model.Search> Search { get; set; }

        public virtual DbSet<BACAClient.Model.Users> Users { get; set; }
    }
}


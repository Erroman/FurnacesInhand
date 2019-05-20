namespace FurnacesInHand
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FurnacesModelLocal : DbContext
    {
        public FurnacesModelLocal() : 
            base("name=FurnacesModelLocal")
        {
        }

        public virtual DbSet<vdp01> vdp01 { get; set; }
        public virtual DbSet<vdp02> vdp02 { get; set; }
        public virtual DbSet<vdp03> vdp03 { get; set; }
        public virtual DbSet<vdp04> vdp04 { get; set; }
        public virtual DbSet<vdp05> vdp05 { get; set; }
        public virtual DbSet<vdp06> vdp06 { get; set; }
        public virtual DbSet<vdp07> vdp07 { get; set; }
        public virtual DbSet<vdp08> vdp08 { get; set; }
        public virtual DbSet<vdp09> vdp09 { get; set; }
        public virtual DbSet<vdp10> vdp10 { get; set; }
        public virtual DbSet<vdp15> vdp15 { get; set; }
        public virtual DbSet<vdp16> vdp16 { get; set; }
        public virtual DbSet<vdp17> vdp17 { get; set; }
        public virtual DbSet<vdp18> vdp18 { get; set; }
        public virtual DbSet<vdp19> vdp19 { get; set; }
        public virtual DbSet<vdp20> vdp20 { get; set; }
        public virtual DbSet<vdp29> vdp29 { get; set; }
        public virtual DbSet<vdp30> vdp30 { get; set; }
        public virtual DbSet<vdp31> vdp31 { get; set; }
        public virtual DbSet<vdp32> vdp32 { get; set; }
        public virtual DbSet<vdp33> vdp33 { get; set; }
        public virtual DbSet<vdp44> vdp44 { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
    class MyModel : FurnacesModelLocal
    {
        public DbSet<vdp08> myNew;
        public MyModel()
        {
            myNew = vdp08;
        }
    }
}

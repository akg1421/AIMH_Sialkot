namespace AIMH_Sialkot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientInfoes", "TestTotal", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientInfoes", "TestTotal");
        }
    }
}

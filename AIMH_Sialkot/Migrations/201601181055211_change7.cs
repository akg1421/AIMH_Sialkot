namespace AIMH_Sialkot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientInfoes", "LumpSum", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientInfoes", "LumpSum");
        }
    }
}

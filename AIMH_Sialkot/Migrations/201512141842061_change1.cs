namespace AIMH_Sialkot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TestInfoes", "CreatedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TestInfoes", "CreatedOn", c => c.String());
        }
    }
}

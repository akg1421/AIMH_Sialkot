namespace AIMH_Sialkot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestInfoes", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestInfoes", "Price");
        }
    }
}

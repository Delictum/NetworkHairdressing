namespace NetworkHairdressing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "IsFirstShift", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "IsFirstShift");
        }
    }
}

namespace NetworkHairdressing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeUserId : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Receptions", "AspNetUsersId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Receptions", "AspNetUsersId", c => c.Int(nullable: false));
        }
    }
}

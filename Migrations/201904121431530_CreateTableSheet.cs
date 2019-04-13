namespace NetworkHairdressing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTableSheet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimeSheets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        File = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TimeSheets");
        }
    }
}

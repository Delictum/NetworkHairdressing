namespace NetworkHairdressing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateReceptionAndPrice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Cost = c.Double(nullable: false),
                        IsMale = c.Boolean(nullable: false),
                        Duration = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Receptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        PriceId = c.Int(nullable: false),
                        AspNetUsersId = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Prices", t => t.PriceId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.PriceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Receptions", "PriceId", "dbo.Prices");
            DropForeignKey("dbo.Receptions", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.Receptions", new[] { "PriceId" });
            DropIndex("dbo.Receptions", new[] { "EmployeeId" });
            DropTable("dbo.Receptions");
            DropTable("dbo.Prices");
        }
    }
}

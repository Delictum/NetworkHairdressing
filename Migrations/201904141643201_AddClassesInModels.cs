namespace NetworkHairdressing.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddClassesInModels : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Employees", "BarbershopId");
            CreateIndex("dbo.EmployeeWorks", "EmployeeId");
            AddForeignKey("dbo.Employees", "BarbershopId", "dbo.Barbershops", "Id", cascadeDelete: true);
            AddForeignKey("dbo.EmployeeWorks", "EmployeeId", "dbo.Employees", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeWorks", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "BarbershopId", "dbo.Barbershops");
            DropIndex("dbo.EmployeeWorks", new[] { "EmployeeId" });
            DropIndex("dbo.Employees", new[] { "BarbershopId" });
        }
    }
}

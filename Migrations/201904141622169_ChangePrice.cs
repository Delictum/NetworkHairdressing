namespace NetworkHairdressing.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ChangePrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "HaircutType", c => c.Int(nullable: false));
            DropColumn("dbo.Prices", "IsMale");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Prices", "IsMale", c => c.Boolean(nullable: false));
            DropColumn("dbo.Prices", "HaircutType");
        }
    }
}

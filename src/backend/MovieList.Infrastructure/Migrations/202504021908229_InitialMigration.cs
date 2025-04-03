namespace MovieList.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Title = c.String(),
                        Overview = c.String(),
                        ReleaseDate = c.DateTime(nullable: false),
                        Popularity = c.Double(nullable: false),
                        VoteAverage = c.Double(nullable: false),
                        VoteCount = c.Double(nullable: false),
                        PosterPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Movies");
        }
    }
}

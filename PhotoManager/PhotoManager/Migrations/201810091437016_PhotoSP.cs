namespace PhotoManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoSP : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("dbo.SP_AdvancedTypePhotoSearch",
                p => new
                {
                    @Search = p.String()
                },
                body: @"SELECT * FROM dbo.Photos WHERE Keywords LIKE '%'+@Search+'%'"
                );

        }

        public override void Down()
        {
            DropStoredProcedure("dbo.SP_AdvancedTypePhotoSearch");
        }
    }
}


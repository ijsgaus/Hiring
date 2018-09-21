using System;
using System.Data;
using FluentMigrator;

namespace Hiring.Migrations.Migrations
{
    [Migration(20180921223036)]
    // ReSharper disable once InconsistentNaming
    public class Migration20180921223036_Users_Create : Migration
    {
        public override void Up()
        {
            Create
                .Table("users")
                .WithColumn("user_id").AsInt64().PrimaryKey("pk_users")
                .WithColumn("email").AsString(128).NotNullable()
                .WithColumn("pwd_hash").AsString(128).NotNullable();
        }

        public override void Down()
        {
            Delete
                .Table("users");
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using blogs_ASPNetCore.Models;

namespace blogs_ASPNetCore.Migrations
{
    [DbContext(typeof(BloggingContext))]
    [Migration("20170523214159_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("blogs_ASPNetCore.Models.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Url");

                    b.HasKey("BlogId");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("blogs_ASPNetCore.Models.ExternalLink", b =>
                {
                    b.Property<int>("ExternalLinkId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlogId");

                    b.Property<string>("Url");

                    b.HasKey("ExternalLinkId");

                    b.HasIndex("BlogId");

                    b.ToTable("ExternalLink");
                });

            modelBuilder.Entity("blogs_ASPNetCore.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BlogId");

                    b.Property<string>("Content");

                    b.Property<string>("Title");

                    b.HasKey("PostId");

                    b.HasIndex("BlogId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("blogs_ASPNetCore.Models.ExternalLink", b =>
                {
                    b.HasOne("blogs_ASPNetCore.Models.Blog")
                        .WithMany("ExternalLinks")
                        .HasForeignKey("BlogId");
                });

            modelBuilder.Entity("blogs_ASPNetCore.Models.Post", b =>
                {
                    b.HasOne("blogs_ASPNetCore.Models.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}

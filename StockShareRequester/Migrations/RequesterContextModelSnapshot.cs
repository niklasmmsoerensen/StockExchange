﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using StockShareRequester.DbAccess;
using System;

namespace StockShareRequester.Migrations
{
    [DbContext(typeof(RequesterContext))]
    partial class RequesterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StockShareRequester.DbAccess.Entities.BuyOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime");

                    b.Property<decimal>("Price");

                    b.Property<int>("StockId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("BuyOrders");
                });
#pragma warning restore 612, 618
        }
    }
}

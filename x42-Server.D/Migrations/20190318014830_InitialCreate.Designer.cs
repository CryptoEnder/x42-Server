﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using X42.Feature.Database.Context;

namespace x42.Migrations
{
    [DbContext(typeof(X42DbContext))]
    [Migration("20190318014830_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("X42.Feature.Database.Tables.MasterNodeData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Address");

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("Ip");

                    b.Property<DateTime>("LastSeen");

                    b.Property<string>("Port");

                    b.Property<long>("Priority");

                    b.Property<string>("Signature");

                    b.HasKey("Id");

                    b.ToTable("masternode");
                });

            modelBuilder.Entity("X42.Feature.Database.Tables.ServerData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("Ip");

                    b.Property<string>("Port");

                    b.Property<string>("Signature");

                    b.HasKey("Id");

                    b.ToTable("server");
                });
#pragma warning restore 612, 618
        }
    }
}
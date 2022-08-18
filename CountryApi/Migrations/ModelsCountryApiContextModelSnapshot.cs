﻿// <auto-generated />
using System;
using CountryApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CountryApi.Migrations
{
    [DbContext(typeof(ModelsCountryApiContext))]
    partial class ModelsCountryApiContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CountryApi.Models.Database.CountryTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Alpha2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Alpha3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Numeric")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("CountryApi.Models.Database.CurrencyTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("CountryTableCurrencyTable", b =>
                {
                    b.Property<Guid>("CountriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CurrenciesId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CountriesId", "CurrenciesId");

                    b.HasIndex("CurrenciesId");

                    b.ToTable("CountryTableCurrencyTable");
                });

            modelBuilder.Entity("CountryTableCurrencyTable", b =>
                {
                    b.HasOne("CountryApi.Models.Database.CountryTable", null)
                        .WithMany()
                        .HasForeignKey("CountriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CountryApi.Models.Database.CurrencyTable", null)
                        .WithMany()
                        .HasForeignKey("CurrenciesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

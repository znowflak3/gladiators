﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GladiatorDatabase.Migrations
{
    [DbContext(typeof(EditorContext))]
    [Migration("20200130102332_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BattleResult", b =>
                {
                    b.Property<int>("BattleResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LoserId")
                        .HasColumnType("int");

                    b.Property<int>("WinnerId")
                        .HasColumnType("int");

                    b.HasKey("BattleResultId");

                    b.ToTable("BattleResults");
                });

            modelBuilder.Entity("Gladiator", b =>
                {
                    b.Property<int>("GladiatorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Alive")
                        .HasColumnType("bit");

                    b.Property<int>("BattleId")
                        .HasColumnType("int");

                    b.Property<int>("Kills")
                        .HasColumnType("int");

                    b.Property<int>("LanistaId")
                        .HasColumnType("int");

                    b.Property<int?>("LastBattleBattleResultId")
                        .HasColumnType("int");

                    b.Property<int>("Loss")
                        .HasColumnType("int");

                    b.Property<int?>("ShopId")
                        .HasColumnType("int");

                    b.Property<int>("Wins")
                        .HasColumnType("int");

                    b.HasKey("GladiatorId");

                    b.HasIndex("LanistaId");

                    b.HasIndex("LastBattleBattleResultId");

                    b.HasIndex("ShopId");

                    b.ToTable("Gladiators");
                });

            modelBuilder.Entity("GladiatorKills", b =>
                {
                    b.Property<int>("GladiatorKillsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BattleId")
                        .HasColumnType("int");

                    b.Property<int?>("BattleResultId")
                        .HasColumnType("int");

                    b.Property<int>("KilledId")
                        .HasColumnType("int");

                    b.HasKey("GladiatorKillsId");

                    b.HasIndex("BattleResultId");

                    b.ToTable("GladiatorKills");
                });

            modelBuilder.Entity("Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("GladiatorId")
                        .HasColumnType("int");

                    b.Property<int?>("LanistaId")
                        .HasColumnType("int");

                    b.Property<int?>("ShopId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("ItemId");

                    b.HasIndex("GladiatorId");

                    b.HasIndex("LanistaId");

                    b.HasIndex("ShopId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Lanista", b =>
                {
                    b.Property<int>("LanistaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LanistaName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Money")
                        .HasColumnType("int");

                    b.Property<int>("ShopId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LanistaId");

                    b.HasIndex("ShopId");

                    b.HasIndex("UserId");

                    b.ToTable("Lanistas");
                });

            modelBuilder.Entity("Shop", b =>
                {
                    b.Property<int>("ShopId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("ShopId");

                    b.ToTable("Shops");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Gladiator", b =>
                {
                    b.HasOne("Lanista", "Lanista")
                        .WithMany("Gladiators")
                        .HasForeignKey("LanistaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BattleResult", "LastBattle")
                        .WithMany("Gladiators")
                        .HasForeignKey("LastBattleBattleResultId");

                    b.HasOne("Shop", null)
                        .WithMany("Gladiators")
                        .HasForeignKey("ShopId");
                });

            modelBuilder.Entity("GladiatorKills", b =>
                {
                    b.HasOne("BattleResult", "BattleResult")
                        .WithMany("Killed")
                        .HasForeignKey("BattleResultId");
                });

            modelBuilder.Entity("Item", b =>
                {
                    b.HasOne("Gladiator", "Gladiator")
                        .WithMany("Equipment")
                        .HasForeignKey("GladiatorId");

                    b.HasOne("Lanista", "Lanista")
                        .WithMany("Items")
                        .HasForeignKey("LanistaId");

                    b.HasOne("Shop", "Shop")
                        .WithMany("Items")
                        .HasForeignKey("ShopId");
                });

            modelBuilder.Entity("Lanista", b =>
                {
                    b.HasOne("Shop", "Shop")
                        .WithMany()
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", "User")
                        .WithMany("Lanistas")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
﻿// <auto-generated />
using System;
using DeepReader.Storage.TiDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DeepReader.TiDB.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220711152551_added-abi's")]
    partial class addedabis
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DeepReader.Storage.Faster.Abis.AbiCacheItem", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("AbiVersions")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Abis");
                });

            modelBuilder.Entity("DeepReader.Types.DbOp", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong?>("ActionTraceGlobalSequence")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Code")
                        .HasColumnType("bigint unsigned");

                    b.Property<byte[]>("NewData")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<ulong>("NewPayer")
                        .HasColumnType("bigint unsigned");

                    b.Property<byte[]>("OldData")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<ulong>("OldPayer")
                        .HasColumnType("bigint unsigned");

                    b.Property<byte>("Operation")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("PrimaryKey")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong>("Scope")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("TableName")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("ActionTraceGlobalSequence");

                    b.ToTable("DbOp");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.Action", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Account")
                        .HasColumnType("bigint unsigned");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<ulong>("Name")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("Action");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.ActionReceipt", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("AbiSequence")
                        .HasColumnType("int unsigned");

                    b.Property<byte[]>("ActionDigest")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<uint>("CodeSequence")
                        .HasColumnType("int unsigned");

                    b.Property<ulong>("GlobalSequence")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ReceiveSequence")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Receiver")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("ActionReceipt");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.Legacy.ProducerKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("AccountName")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("BlockSigningKey")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong?>("ProducerScheduleId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("ProducerScheduleId");

                    b.ToTable("ProducerKey");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.PermissionLevel", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong?>("ActionId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Actor")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Permission")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("ActionId");

                    b.ToTable("PermissionLevel");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.ProducerSchedule", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("Version")
                        .HasColumnType("int unsigned");

                    b.HasKey("Id");

                    b.ToTable("ProducerSchedule");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.TransactionReceiptHeader", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("CpuUsageUs")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("NetUsageWords")
                        .HasColumnType("int unsigned");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TransactionReceiptHeader");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.TransactionTraceAuthSequence", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Account")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong?>("ActionReceiptId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Sequence")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("ActionReceiptId");

                    b.ToTable("TransactionTraceAuthSequence");
                });

            modelBuilder.Entity("DeepReader.Types.RamOp", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong?>("ActionTraceGlobalSequence")
                        .HasColumnType("bigint unsigned");

                    b.Property<long>("Delta")
                        .HasColumnType("bigint");

                    b.Property<byte>("Operation")
                        .HasColumnType("tinyint unsigned");

                    b.Property<ulong>("Payer")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Usage")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("ActionTraceGlobalSequence");

                    b.ToTable("RamOp");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.ActionTrace", b =>
                {
                    b.Property<ulong>("GlobalSequence")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ActId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Console")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("ContextFree")
                        .HasColumnType("tinyint(1)");

                    b.Property<ulong?>("CreatorActionId")
                        .HasColumnType("bigint unsigned");

                    b.Property<long>("ElapsedUs")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsNotify")
                        .HasColumnType("tinyint(1)");

                    b.Property<ulong>("ReceiptId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Receiver")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("ReturnValue")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte[]>("TransactionTraceId")
                        .HasColumnType("varbinary(3072)");

                    b.HasKey("GlobalSequence");

                    b.HasIndex("ActId");

                    b.HasIndex("CreatorActionId");

                    b.HasIndex("ReceiptId");

                    b.HasIndex("TransactionTraceId");

                    b.ToTable("ActionTraces");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.Block", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("varbinary(3072)");

                    b.Property<byte[]>("ActionMroot")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<ushort>("Confirmed")
                        .HasColumnType("smallint unsigned");

                    b.Property<ulong?>("NewProducersId")
                        .HasColumnType("bigint unsigned");

                    b.Property<uint>("Number")
                        .HasColumnType("int unsigned");

                    b.Property<byte[]>("Previous")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<ulong>("Producer")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("ProducerSignature")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<uint>("ScheduleVersion")
                        .HasColumnType("int unsigned");

                    b.Property<uint>("Timestamp")
                        .HasColumnType("int unsigned");

                    b.Property<byte[]>("TransactionMroot")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.HasKey("Id");

                    b.HasIndex("NewProducersId");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.TransactionTrace", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("varbinary(3072)");

                    b.Property<byte[]>("BlockId")
                        .HasColumnType("varbinary(3072)");

                    b.Property<uint>("BlockNum")
                        .HasColumnType("int unsigned");

                    b.Property<long>("Elapsed")
                        .HasColumnType("bigint");

                    b.Property<ulong>("NetUsage")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ReceiptId")
                        .HasColumnType("bigint unsigned");

                    b.Property<bool>("Scheduled")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.HasIndex("ReceiptId");

                    b.ToTable("TransactionTraces");
                });

            modelBuilder.Entity("DeepReader.Types.TableOp", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong?>("ActionTraceGlobalSequence")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("Code")
                        .HasColumnType("bigint unsigned");

                    b.Property<byte>("Operation")
                        .HasColumnType("tinyint unsigned");

                    b.Property<ulong>("Scope")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("TableName")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("ActionTraceGlobalSequence");

                    b.ToTable("TableOp");
                });

            modelBuilder.Entity("DeepReader.Types.DbOp", b =>
                {
                    b.HasOne("DeepReader.Types.StorageTypes.ActionTrace", null)
                        .WithMany("DbOps")
                        .HasForeignKey("ActionTraceGlobalSequence");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.Legacy.ProducerKey", b =>
                {
                    b.HasOne("DeepReader.Types.Eosio.Chain.ProducerSchedule", null)
                        .WithMany("Producers")
                        .HasForeignKey("ProducerScheduleId");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.PermissionLevel", b =>
                {
                    b.HasOne("DeepReader.Types.Eosio.Chain.Action", null)
                        .WithMany("Authorization")
                        .HasForeignKey("ActionId");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.TransactionTraceAuthSequence", b =>
                {
                    b.HasOne("DeepReader.Types.Eosio.Chain.ActionReceipt", null)
                        .WithMany("AuthSequence")
                        .HasForeignKey("ActionReceiptId");
                });

            modelBuilder.Entity("DeepReader.Types.RamOp", b =>
                {
                    b.HasOne("DeepReader.Types.StorageTypes.ActionTrace", null)
                        .WithMany("RamOps")
                        .HasForeignKey("ActionTraceGlobalSequence");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.ActionTrace", b =>
                {
                    b.HasOne("DeepReader.Types.Eosio.Chain.Action", "Act")
                        .WithMany()
                        .HasForeignKey("ActId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DeepReader.Types.StorageTypes.ActionTrace", "CreatorAction")
                        .WithMany("CreatedActions")
                        .HasForeignKey("CreatorActionId");

                    b.HasOne("DeepReader.Types.Eosio.Chain.ActionReceipt", "Receipt")
                        .WithMany()
                        .HasForeignKey("ReceiptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DeepReader.Types.StorageTypes.TransactionTrace", null)
                        .WithMany("ActionTraces")
                        .HasForeignKey("TransactionTraceId");

                    b.Navigation("Act");

                    b.Navigation("CreatorAction");

                    b.Navigation("Receipt");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.Block", b =>
                {
                    b.HasOne("DeepReader.Types.Eosio.Chain.ProducerSchedule", "NewProducers")
                        .WithMany()
                        .HasForeignKey("NewProducersId");

                    b.Navigation("NewProducers");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.TransactionTrace", b =>
                {
                    b.HasOne("DeepReader.Types.StorageTypes.Block", null)
                        .WithMany("Transactions")
                        .HasForeignKey("BlockId");

                    b.HasOne("DeepReader.Types.Eosio.Chain.TransactionReceiptHeader", "Receipt")
                        .WithMany()
                        .HasForeignKey("ReceiptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receipt");
                });

            modelBuilder.Entity("DeepReader.Types.TableOp", b =>
                {
                    b.HasOne("DeepReader.Types.StorageTypes.ActionTrace", null)
                        .WithMany("TableOps")
                        .HasForeignKey("ActionTraceGlobalSequence");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.Action", b =>
                {
                    b.Navigation("Authorization");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.ActionReceipt", b =>
                {
                    b.Navigation("AuthSequence");
                });

            modelBuilder.Entity("DeepReader.Types.Eosio.Chain.ProducerSchedule", b =>
                {
                    b.Navigation("Producers");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.ActionTrace", b =>
                {
                    b.Navigation("CreatedActions");

                    b.Navigation("DbOps");

                    b.Navigation("RamOps");

                    b.Navigation("TableOps");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.Block", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("DeepReader.Types.StorageTypes.TransactionTrace", b =>
                {
                    b.Navigation("ActionTraces");
                });
#pragma warning restore 612, 618
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeepReader.TiDB.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Abis",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AbiVersions = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abis", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<byte[]>(type: "longblob", nullable: false),
                    Account = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Name = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ActionReceipt",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Receiver = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActionDigest = table.Column<byte[]>(type: "longblob", nullable: false),
                    GlobalSequence = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ReceiveSequence = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CodeSequence = table.Column<uint>(type: "int unsigned", nullable: false),
                    AbiSequence = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionReceipt", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProducerSchedule",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducerSchedule", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionReceiptHeader",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CpuUsageUs = table.Column<uint>(type: "int unsigned", nullable: false),
                    NetUsageWords = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionReceiptHeader", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PermissionLevel",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Actor = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Permission = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActionId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionLevel_Action_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Action",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionTraceAuthSequence",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Account = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Sequence = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActionReceiptId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTraceAuthSequence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionTraceAuthSequence_ActionReceipt_ActionReceiptId",
                        column: x => x.ActionReceiptId,
                        principalTable: "ActionReceipt",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Number = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id = table.Column<byte[]>(type: "longblob", nullable: true),
                    Timestamp = table.Column<uint>(type: "int unsigned", nullable: false),
                    Producer = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Confirmed = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    Previous = table.Column<byte[]>(type: "longblob", nullable: false),
                    TransactionMroot = table.Column<byte[]>(type: "longblob", nullable: false),
                    ActionMroot = table.Column<byte[]>(type: "longblob", nullable: false),
                    ScheduleVersion = table.Column<uint>(type: "int unsigned", nullable: false),
                    NewProducersId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    ProducerSignature = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Blocks_ProducerSchedule_NewProducersId",
                        column: x => x.NewProducersId,
                        principalTable: "ProducerSchedule",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProducerKey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountName = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    BlockSigningKey = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProducerScheduleId = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducerKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProducerKey_ProducerSchedule_ProducerScheduleId",
                        column: x => x.ProducerScheduleId,
                        principalTable: "ProducerSchedule",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionTraces",
                columns: table => new
                {
                    TransactionNum = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id = table.Column<byte[]>(type: "longblob", nullable: true),
                    BlockNum = table.Column<uint>(type: "int unsigned", nullable: false),
                    ReceiptId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Elapsed = table.Column<long>(type: "bigint", nullable: false),
                    NetUsage = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Scheduled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BlockNumber = table.Column<uint>(type: "int unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTraces", x => x.TransactionNum);
                    table.ForeignKey(
                        name: "FK_TransactionTraces_Blocks_BlockNumber",
                        column: x => x.BlockNumber,
                        principalTable: "Blocks",
                        principalColumn: "Number");
                    table.ForeignKey(
                        name: "FK_TransactionTraces_TransactionReceiptHeader_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "TransactionReceiptHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ActionTraces",
                columns: table => new
                {
                    GlobalSequence = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReceiptId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Receiver = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Console = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContextFree = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ElapsedUs = table.Column<long>(type: "bigint", nullable: false),
                    ReturnValue = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsNotify = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatorActionId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    TransactionTraceTransactionNum = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTraces", x => x.GlobalSequence);
                    table.ForeignKey(
                        name: "FK_ActionTraces_Action_ActId",
                        column: x => x.ActId,
                        principalTable: "Action",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionTraces_ActionReceipt_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "ActionReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionTraces_ActionTraces_CreatorActionId",
                        column: x => x.CreatorActionId,
                        principalTable: "ActionTraces",
                        principalColumn: "GlobalSequence");
                    table.ForeignKey(
                        name: "FK_ActionTraces_TransactionTraces_TransactionTraceTransactionNum",
                        column: x => x.TransactionTraceTransactionNum,
                        principalTable: "TransactionTraces",
                        principalColumn: "TransactionNum");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DbOp",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Operation = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Code = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TableName = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Scope = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    PrimaryKey = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OldPayer = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    NewPayer = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    OldData = table.Column<byte[]>(type: "longblob", nullable: false),
                    NewData = table.Column<byte[]>(type: "longblob", nullable: false),
                    ActionTraceGlobalSequence = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbOp_ActionTraces_ActionTraceGlobalSequence",
                        column: x => x.ActionTraceGlobalSequence,
                        principalTable: "ActionTraces",
                        principalColumn: "GlobalSequence");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RamOp",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Operation = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Payer = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Delta = table.Column<long>(type: "bigint", nullable: false),
                    Usage = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActionTraceGlobalSequence = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RamOp_ActionTraces_ActionTraceGlobalSequence",
                        column: x => x.ActionTraceGlobalSequence,
                        principalTable: "ActionTraces",
                        principalColumn: "GlobalSequence");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TableOp",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Operation = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Code = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Scope = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TableName = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ActionTraceGlobalSequence = table.Column<ulong>(type: "bigint unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableOp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableOp_ActionTraces_ActionTraceGlobalSequence",
                        column: x => x.ActionTraceGlobalSequence,
                        principalTable: "ActionTraces",
                        principalColumn: "GlobalSequence");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTraces_ActId",
                table: "ActionTraces",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTraces_CreatorActionId",
                table: "ActionTraces",
                column: "CreatorActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTraces_ReceiptId",
                table: "ActionTraces",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTraces_TransactionTraceTransactionNum",
                table: "ActionTraces",
                column: "TransactionTraceTransactionNum");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_NewProducersId",
                table: "Blocks",
                column: "NewProducersId");

            migrationBuilder.CreateIndex(
                name: "IX_DbOp_ActionTraceGlobalSequence",
                table: "DbOp",
                column: "ActionTraceGlobalSequence");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionLevel_ActionId",
                table: "PermissionLevel",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProducerKey_ProducerScheduleId",
                table: "ProducerKey",
                column: "ProducerScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_RamOp_ActionTraceGlobalSequence",
                table: "RamOp",
                column: "ActionTraceGlobalSequence");

            migrationBuilder.CreateIndex(
                name: "IX_TableOp_ActionTraceGlobalSequence",
                table: "TableOp",
                column: "ActionTraceGlobalSequence");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTraceAuthSequence_ActionReceiptId",
                table: "TransactionTraceAuthSequence",
                column: "ActionReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTraces_BlockNumber",
                table: "TransactionTraces",
                column: "BlockNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTraces_ReceiptId",
                table: "TransactionTraces",
                column: "ReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abis");

            migrationBuilder.DropTable(
                name: "DbOp");

            migrationBuilder.DropTable(
                name: "PermissionLevel");

            migrationBuilder.DropTable(
                name: "ProducerKey");

            migrationBuilder.DropTable(
                name: "RamOp");

            migrationBuilder.DropTable(
                name: "TableOp");

            migrationBuilder.DropTable(
                name: "TransactionTraceAuthSequence");

            migrationBuilder.DropTable(
                name: "ActionTraces");

            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "ActionReceipt");

            migrationBuilder.DropTable(
                name: "TransactionTraces");

            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "TransactionReceiptHeader");

            migrationBuilder.DropTable(
                name: "ProducerSchedule");
        }
    }
}

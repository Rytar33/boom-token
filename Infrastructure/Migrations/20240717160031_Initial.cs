using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "improvement",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    improvement_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    value_increased = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<short>(type: "smallint", nullable: false),
                    cost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_improvement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task_for_reward",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    task_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    link = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    progress_to_completion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    reward = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_for_reward", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    balance = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    rang_tap = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    limit_energy = table.Column<int>(type: "integer", nullable: false, defaultValue: 500),
                    energy = table.Column<int>(type: "integer", nullable: false, defaultValue: 500),
                    energy_recovery_in_second = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    reward_for_click = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    count_click = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    referal_link = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    date_time_registration = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "improvement_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_user = table.Column<Guid>(type: "uuid", nullable: false),
                    id_improvement = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_improvement_access", x => x.id);
                    table.ForeignKey(
                        name: "FK_improvement_access_improvement_id_improvement",
                        column: x => x.id_improvement,
                        principalTable: "improvement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_improvement_access_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "referal_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    count_take_from_click = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    id_user = table.Column<Guid>(type: "uuid", nullable: false),
                    id_user_invited = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referal_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_referal_users_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_referal_users_user_id_user_invited",
                        column: x => x.id_user_invited,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "task_for_reward_access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_user = table.Column<Guid>(type: "uuid", nullable: false),
                    id_task_for_reward = table.Column<Guid>(type: "uuid", nullable: false),
                    current_value = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    date_time_completed = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_for_reward_access", x => x.id);
                    table.ForeignKey(
                        name: "FK_task_for_reward_access_task_for_reward_id_task_for_reward",
                        column: x => x.id_task_for_reward,
                        principalTable: "task_for_reward",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_for_reward_access_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "improvement",
                columns: new[] { "id", "cost", "description", "improvement_type", "level", "name", "value_increased" },
                values: new object[,]
                {
                    { new Guid("1170536b-a815-4629-a791-945773c83d9d"), 150000, "", "ProfitPerTap", (short)4, "Tap profit lvl 4", 2 },
                    { new Guid("243520ac-ad05-4703-b904-e3044ef45d96"), 35000, "", "EnergyLimit", (short)4, "Energy limit lvl 4", 150 },
                    { new Guid("3555bbe3-0dc4-431a-9b78-d696af2f15a9"), 5000, "", "EnergyLimit", (short)1, "Energy limit lvl 1", 50 },
                    { new Guid("47c05f9c-6198-4e77-ae22-a422005519af"), 20000, "", "ProfitPerTap", (short)2, "Tap profit lvl 2", 1 },
                    { new Guid("48e1fd52-7253-4f25-8452-c79985b6630a"), 100000, "", "SpeedEnergyRecovery", (short)4, "Energy recovery lvl 4", 2 },
                    { new Guid("4970dbba-7bbf-429e-81d8-965c4276ede7"), 20000, "", "SpeedEnergyRecovery", (short)2, "Energy recovery lvl 2", 1 },
                    { new Guid("4b85eb67-07a9-449c-ba7a-d353b2df4b78"), 50000, "", "ProfitPerTap", (short)3, "Tap profit lvl 3", 1 },
                    { new Guid("5cd5a851-9d62-44b6-9acf-0d92d5e4b9fa"), 10000, "", "SpeedEnergyRecovery", (short)1, "Energy recovery lvl 1", 1 },
                    { new Guid("7748c297-cd54-4fa7-bafe-0d9c73892d8b"), 20000, "", "EnergyLimit", (short)3, "Energy limit lvl 3", 100 },
                    { new Guid("882b441a-1b74-4748-8b76-fc4c8f227d4d"), 50000, "", "SpeedEnergyRecovery", (short)3, "Energy recovery lvl 3", 2 },
                    { new Guid("c0900e59-f769-4644-b276-05489f9cbd41"), 200000, "", "SpeedEnergyRecovery", (short)5, "Energy recovery lvl 5", 5 },
                    { new Guid("cabe8e60-a3f8-41d7-8716-4d482515a7fb"), 10000, "", "ProfitPerTap", (short)1, "Tap profit lvl 1", 1 },
                    { new Guid("cbb334d3-8e7e-4d3b-bca4-898d5337e271"), 10000, "", "EnergyLimit", (short)2, "Energy limit lvl 2", 50 },
                    { new Guid("e9912eec-213f-4be2-bc85-b9f57a44aa25"), 50000, "", "EnergyLimit", (short)5, "Energy limit lvl 5", 250 },
                    { new Guid("ec71173f-b87b-4280-83ca-65d20d88d5e6"), 250000, "", "ProfitPerTap", (short)5, "Tap profit lvl 5", 2 }
                });

            migrationBuilder.InsertData(
                table: "task_for_reward",
                columns: new[] { "id", "description", "link", "name", "progress_to_completion", "reward", "target_type", "task_type" },
                values: new object[,]
                {
                    { new Guid("0610d647-758b-4866-990c-3c5fd047075a"), "", "https://t.me/rutar33", "Канал Boom token", 1, 500, "OpenLink", "EveryDay" },
                    { new Guid("08e7dd67-0935-434f-9d64-a7d5b78734e5"), "", null, "Накликать 1.5к раз", 1500, 3000, "Tap", "EveryDay" },
                    { new Guid("4f4e507e-5d2e-474c-9378-366322781fcd"), "", "https://t.me/Dartist1", "Канал OLEG", 1, 1000, "OpenLink", "Single" },
                    { new Guid("51e9172b-cc89-47ab-92b1-3faaace653ac"), "", "https://t.me/Dartist1", "Канал ISPUM", 1, 1000, "OpenLink", "Single" },
                    { new Guid("831824be-e7db-4128-be5b-c4ccf84fc9ed"), "", null, "Пригласить друга", 1, 3000, "ReferalInvite", "EveryDay" },
                    { new Guid("ea535c8b-4af7-495f-9dbc-c865d428129f"), "", "https://t.me/rutar33", "Канал LOREM", 1, 1000, "OpenLink", "Single" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_improvement_access_id_improvement",
                table: "improvement_access",
                column: "id_improvement");

            migrationBuilder.CreateIndex(
                name: "IX_improvement_access_id_user",
                table: "improvement_access",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_referal_users_id_user",
                table: "referal_users",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_referal_users_id_user_invited",
                table: "referal_users",
                column: "id_user_invited");

            migrationBuilder.CreateIndex(
                name: "IX_task_for_reward_access_id_task_for_reward",
                table: "task_for_reward_access",
                column: "id_task_for_reward");

            migrationBuilder.CreateIndex(
                name: "IX_task_for_reward_access_id_user",
                table: "task_for_reward_access",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "improvement_access");

            migrationBuilder.DropTable(
                name: "referal_users");

            migrationBuilder.DropTable(
                name: "task_for_reward_access");

            migrationBuilder.DropTable(
                name: "improvement");

            migrationBuilder.DropTable(
                name: "task_for_reward");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PayStation.Migrations
{
    /// <inheritdoc />
    public partial class NOME_MIGRAZIONE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlarmsDB",
                columns: table => new
                {
                    id_alarm = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    alarm_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    status_code = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    alarm_code = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    operator_code = table.Column<string>(type: "TEXT", nullable: true),
                    test_colum = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmsDB", x => x.id_alarm);
                });

            migrationBuilder.CreateTable(
                name: "CashClosureReceiptsDB",
                columns: table => new
                {
                    id_movement = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    closing_progress = table.Column<int>(type: "INTEGER", nullable: true),
                    movement_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    @operator = table.Column<string>(name: "operator", type: "TEXT", nullable: true),
                    outcome = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    non_given_change = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load_5_rc = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load_10_rc = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load_20_rc = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load_5_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load_10_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load_20_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_load_50_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_200 = table.Column<int>(type: "INTEGER", nullable: true),
                    non_loaded_banknotes_total = table.Column<int>(type: "INTEGER", nullable: true),
                    non_loaded_banknotes = table.Column<int>(type: "INTEGER", nullable: true),
                    non_loaded_banknotes1 = table.Column<int>(type: "INTEGER", nullable: true),
                    total_paid_cents = table.Column<int>(type: "INTEGER", nullable: true),
                    total_paid_cents_cash = table.Column<int>(type: "INTEGER", nullable: true),
                    pos_income = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_introduction = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_introduction_5 = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_introduction_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_introduction_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes_introduction_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_200 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_200 = table.Column<int>(type: "INTEGER", nullable: true),
                    change = table.Column<int>(type: "INTEGER", nullable: true),
                    change_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_500 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_1000 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_2000 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_banknotes_total = table.Column<int>(type: "INTEGER", nullable: true),
                    present_banknote_5 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_banknote_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_banknote_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_banknote_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_coins_total = table.Column<int>(type: "INTEGER", nullable: true),
                    present_coin_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_coin_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_coin_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_coin_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    present_coin_200 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashClosureReceiptsDB", x => x.id_movement);
                });

            migrationBuilder.CreateTable(
                name: "CashDetailsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    tube_10 = table.Column<short>(type: "INTEGER", nullable: true),
                    tube_20 = table.Column<short>(type: "INTEGER", nullable: true),
                    tube_50 = table.Column<short>(type: "INTEGER", nullable: true),
                    tube_100 = table.Column<short>(type: "INTEGER", nullable: true),
                    tube_200 = table.Column<short>(type: "INTEGER", nullable: true),
                    dispensed_banknote = table.Column<short>(type: "INTEGER", nullable: true),
                    dispensed_banknote1 = table.Column<short>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashDetailsDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevicesDB",
                columns: table => new
                {
                    device_type = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    enabled = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesDB", x => x.device_type);
                });

            migrationBuilder.CreateTable(
                name: "IngenicoPosMovementsDB",
                columns: table => new
                {
                    id_mov = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_movmentDB = table.Column<int>(type: "INTEGER", nullable: true),
                    paid_cents = table.Column<int>(type: "INTEGER", nullable: true),
                    transaction_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    success = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    overhead = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngenicoPosMovementsDB", x => x.id_mov);
                });

            migrationBuilder.CreateTable(
                name: "MovementsDB",
                columns: table => new
                {
                    id_movement = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    movement_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    outcome = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    paid_cents = table.Column<int>(type: "INTEGER", nullable: true),
                    banknotes = table.Column<int>(type: "INTEGER", nullable: true),
                    coins = table.Column<int>(type: "INTEGER", nullable: true),
                    change = table.Column<int>(type: "INTEGER", nullable: true),
                    change_banknotes = table.Column<int>(type: "INTEGER", nullable: true),
                    change_banknotes1 = table.Column<int>(type: "INTEGER", nullable: true),
                    closing_progress = table.Column<int>(type: "INTEGER", nullable: true),
                    operator_code = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementsDB", x => x.id_movement);
                });

            migrationBuilder.CreateTable(
                name: "ParametersDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Row1 = table.Column<string>(type: "TEXT", nullable: true),
                    Row2 = table.Column<string>(type: "TEXT", nullable: true),
                    Row3 = table.Column<string>(type: "TEXT", nullable: true),
                    Row4 = table.Column<string>(type: "TEXT", nullable: true),
                    Row5 = table.Column<string>(type: "TEXT", nullable: true),
                    Row6 = table.Column<string>(type: "TEXT", nullable: true),
                    Row7 = table.Column<string>(type: "TEXT", nullable: true),
                    Cash_ID = table.Column<int>(type: "INTEGER", nullable: true),
                    Terminal_ID = table.Column<string>(type: "TEXT", nullable: true),
                    BanknoteValueInCassetteBox1 = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxBanknoteGiveBackBox1 = table.Column<int>(type: "INTEGER", nullable: true),
                    BanknoteValueInCassetteBox2 = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxBanknoteGiveBackBox2 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametersDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartialCashClosuresDB",
                columns: table => new
                {
                    closing_progress = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date_update = table.Column<DateTime>(type: "TEXT", nullable: true),
                    non_given_change = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load_5_rc = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load_10_rc = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load_20_rc = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load_5_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load_10_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load_20_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_load_50_stacker = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_load_200 = table.Column<int>(type: "INTEGER", nullable: true),
                    total_paid_cents = table.Column<int>(type: "INTEGER", nullable: true),
                    total_paid_cents_cash = table.Column<int>(type: "INTEGER", nullable: true),
                    pos_income = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_introduction = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_introduction_5 = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_introduction_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_introduction_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    banknote_introduction_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    coin_introduction_200 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    excess_coin_200 = table.Column<int>(type: "INTEGER", nullable: true),
                    change = table.Column<int>(type: "INTEGER", nullable: true),
                    change_10 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_20 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_50 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_100 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_500 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_1000 = table.Column<int>(type: "INTEGER", nullable: true),
                    change_2000 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartialCashClosuresDB", x => x.closing_progress);
                });

            migrationBuilder.CreateTable(
                name: "SerialConnectionParametersDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Device = table.Column<int>(type: "INTEGER", nullable: true),
                    LastPortName = table.Column<string>(type: "TEXT", nullable: true),
                    BaudRate = table.Column<int>(type: "INTEGER", nullable: false),
                    DataBits = table.Column<int>(type: "INTEGER", nullable: false),
                    Parity = table.Column<int>(type: "INTEGER", nullable: false),
                    StopBits = table.Column<int>(type: "INTEGER", nullable: false),
                    Handshake = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialConnectionParametersDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerialConnectionSettingDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Device = table.Column<string>(type: "TEXT", nullable: true),
                    MaxRetryAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    RetryDelayMilliseconds = table.Column<int>(type: "INTEGER", nullable: false),
                    IsTimedMode = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialConnectionSettingDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextPrinterObjectsDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    Highlighting = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeFont = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    Underline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Bold = table.Column<bool>(type: "INTEGER", nullable: false),
                    Overlapping = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpsideDown = table.Column<bool>(type: "INTEGER", nullable: false),
                    Revolving = table.Column<bool>(type: "INTEGER", nullable: false),
                    LeftMargin = table.Column<int>(type: "INTEGER", nullable: false),
                    Alligment = table.Column<int>(type: "INTEGER", nullable: false),
                    DistanceBeforeObj = table.Column<int>(type: "INTEGER", nullable: false),
                    DistanceAfterObj = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextPrinterObjectsDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketLayoutDB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Objects = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketLayoutDB", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AlarmsDB",
                columns: new[] { "id_alarm", "alarm_code", "alarm_date", "description", "operator_code", "status_code", "test_colum" },
                values: new object[] { 1, "001", new DateTime(2024, 4, 12, 16, 11, 38, 399, DateTimeKind.Local).AddTicks(5975), "Alarms", "A", "I", null });

            migrationBuilder.InsertData(
                table: "CashClosureReceiptsDB",
                columns: new[] { "id_movement", "banknotes_introduction", "banknotes_introduction_10", "banknotes_introduction_20", "banknotes_introduction_5", "banknotes_introduction_50", "banknotes_load", "banknotes_load_10_rc", "banknotes_load_10_stacker", "banknotes_load_20_rc", "banknotes_load_20_stacker", "banknotes_load_50_stacker", "banknotes_load_5_rc", "banknotes_load_5_stacker", "change", "change_10", "change_100", "change_1000", "change_20", "change_2000", "change_50", "change_500", "closing_progress", "coin_introduction", "coin_introduction_10", "coin_introduction_100", "coin_introduction_20", "coin_introduction_200", "coin_introduction_50", "coin_load", "coin_load_10", "coin_load_100", "coin_load_20", "coin_load_200", "coin_load_50", "description", "excess_coin_10", "excess_coin_100", "excess_coin_20", "excess_coin_200", "excess_coin_50", "movement_date", "non_given_change", "non_loaded_banknotes", "non_loaded_banknotes1", "non_loaded_banknotes_total", "operator", "outcome", "pos_income", "present_banknote_10", "present_banknote_20", "present_banknote_5", "present_banknote_50", "present_banknotes_total", "present_coin_10", "present_coin_100", "present_coin_20", "present_coin_200", "present_coin_50", "present_coins_total", "total_paid_cents", "total_paid_cents_cash" },
                values: new object[] { 1, 150, 30, 30, 30, 30, 200, 50, 50, 50, 50, 50, 50, 50, 50, 10, 10, 0, 10, 0, 10, 0, 1, 50, 10, 10, 10, 10, 10, 100, 20, 20, 20, 20, 20, "Description1", 5, 5, 5, 5, 5, new DateTime(2024, 4, 17, 16, 11, 38, 399, DateTimeKind.Local).AddTicks(8434), 100, 30, 30, 150, "Operator1", "OUT", 300, 30, 30, 30, 30, 150, 10, 10, 10, 10, 10, 50, 500, 200 });

            migrationBuilder.InsertData(
                table: "CashDetailsDB",
                columns: new[] { "Id", "dispensed_banknote", "dispensed_banknote1", "tube_10", "tube_100", "tube_20", "tube_200", "tube_50" },
                values: new object[] { 1, (short)10, (short)20, (short)50, (short)50, (short)50, (short)50, (short)50 });

            migrationBuilder.InsertData(
                table: "DevicesDB",
                columns: new[] { "device_type", "description", "enabled" },
                values: new object[,]
                {
                    { "1", "Gryphon (lettore monete)", "0" },
                    { "2", "Vega Pro (lettore banconote)", "0" },
                    { "3", "isef2000 (lettore POS)", "0" },
                    { "4", "KP300 (stampante ticket)", "0" },
                    { "5", "RC (rendi banconote sigolo taglio)", "0" },
                    { "6", "TWIN (rendi banconote doppio taglio)", "0" }
                });

            migrationBuilder.InsertData(
                table: "IngenicoPosMovementsDB",
                columns: new[] { "id_mov", "description", "id_movmentDB", "overhead", "paid_cents", "success", "transaction_date" },
                values: new object[] { 1, "Payment successful", 0, "Some overhead data", 1000, "Y", new DateTime(2024, 4, 17, 16, 11, 38, 400, DateTimeKind.Local).AddTicks(4352) });

            migrationBuilder.InsertData(
                table: "MovementsDB",
                columns: new[] { "id_movement", "banknotes", "change", "change_banknotes", "change_banknotes1", "closing_progress", "coins", "description", "movement_date", "operator_code", "outcome", "paid_cents" },
                values: new object[] { 1, 5, 0, 0, 0, 0, 10, "Initial deposit", new DateTime(2024, 4, 12, 16, 11, 38, 400, DateTimeKind.Local).AddTicks(6194), "A", "IN", 1000 });

            migrationBuilder.InsertData(
                table: "ParametersDB",
                columns: new[] { "Id", "BanknoteValueInCassetteBox1", "BanknoteValueInCassetteBox2", "Cash_ID", "Description", "MaxBanknoteGiveBackBox1", "MaxBanknoteGiveBackBox2", "Name", "Row1", "Row2", "Row3", "Row4", "Row5", "Row6", "Row7", "Terminal_ID", "Value" },
                values: new object[] { 1, 10, 20, 1, "Description1", 5, 10, "Parameter1", "Row1", "Row2", "Row3", "Row4", "Row5", "Row6", "Row7", "Terminal1", "Value1" });

            migrationBuilder.InsertData(
                table: "PartialCashClosuresDB",
                columns: new[] { "closing_progress", "banknote_introduction", "banknote_introduction_10", "banknote_introduction_20", "banknote_introduction_5", "banknote_introduction_50", "banknote_load", "banknote_load_10_rc", "banknote_load_10_stacker", "banknote_load_20_rc", "banknote_load_20_stacker", "banknote_load_50_stacker", "banknote_load_5_rc", "banknote_load_5_stacker", "change", "change_10", "change_100", "change_1000", "change_20", "change_2000", "change_50", "change_500", "coin_introduction", "coin_introduction_10", "coin_introduction_100", "coin_introduction_20", "coin_introduction_200", "coin_introduction_50", "coin_load", "coin_load_10", "coin_load_100", "coin_load_20", "coin_load_200", "coin_load_50", "date_update", "excess_coin_10", "excess_coin_100", "excess_coin_20", "excess_coin_200", "excess_coin_50", "non_given_change", "pos_income", "total_paid_cents", "total_paid_cents_cash" },
                values: new object[] { 1, 150, 30, 30, 30, 30, 200, 50, 50, 50, 50, 50, 50, 50, 50, 10, 10, 0, 10, 0, 10, 0, 50, 10, 10, 10, 10, 10, 100, 20, 20, 20, 20, 20, new DateTime(2024, 4, 17, 16, 11, 38, 401, DateTimeKind.Local).AddTicks(234), 5, 5, 5, 5, 5, 100, 300, 500, 200 });

            migrationBuilder.InsertData(
                table: "SerialConnectionParametersDB",
                columns: new[] { "Id", "BaudRate", "DataBits", "Device", "Handshake", "LastPortName", "Parity", "StopBits" },
                values: new object[,]
                {
                    { 1, 9600, 8, 0, 0, "COM2", 2, 1 },
                    { 2, 9600, 8, 3, 0, "COM8", 2, 1 },
                    { 3, 9600, 8, 1, 0, "COM7", 0, 1 },
                    { 4, 115200, 8, 2, 0, "COM8", 0, 1 }
                });

            migrationBuilder.InsertData(
                table: "SerialConnectionSettingDB",
                columns: new[] { "Id", "Device", "IsTimedMode", "MaxRetryAttempts", "RetryDelayMilliseconds" },
                values: new object[,]
                {
                    { 1, "CASH", true, 3, 300 },
                    { 2, "PRINTER", true, 3, 300 },
                    { 3, "COIN", true, 3, 300 },
                    { 4, "POS", true, 3, 300 }
                });

            migrationBuilder.InsertData(
                table: "TextPrinterObjectsDB",
                columns: new[] { "Id", "Alligment", "Bold", "DistanceAfterObj", "DistanceBeforeObj", "Highlighting", "LeftMargin", "Name", "Overlapping", "Revolving", "SizeHeight", "SizeWidth", "Text", "Type", "TypeFont", "Underline", "UpsideDown" },
                values: new object[] { 1, 0, false, 0, 0, false, 0, "Company", false, false, 1, 1, "Company", 1, 0, false, false });

            migrationBuilder.InsertData(
                table: "TicketLayoutDB",
                columns: new[] { "Id", "Name", "Objects" },
                values: new object[] { 1, "Ticket cassa", "[1,2]" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmsDB");

            migrationBuilder.DropTable(
                name: "CashClosureReceiptsDB");

            migrationBuilder.DropTable(
                name: "CashDetailsDB");

            migrationBuilder.DropTable(
                name: "DevicesDB");

            migrationBuilder.DropTable(
                name: "IngenicoPosMovementsDB");

            migrationBuilder.DropTable(
                name: "MovementsDB");

            migrationBuilder.DropTable(
                name: "ParametersDB");

            migrationBuilder.DropTable(
                name: "PartialCashClosuresDB");

            migrationBuilder.DropTable(
                name: "SerialConnectionParametersDB");

            migrationBuilder.DropTable(
                name: "SerialConnectionSettingDB");

            migrationBuilder.DropTable(
                name: "TextPrinterObjectsDB");

            migrationBuilder.DropTable(
                name: "TicketLayoutDB");
        }
    }
}

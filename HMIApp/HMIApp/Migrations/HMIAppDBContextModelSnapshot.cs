﻿// <auto-generated />
using HMIApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HMIApp.Migrations
{
    [DbContext(typeof(HMIAppDBContext))]
    partial class HMIAppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HMIApp.Components.DataBase.Reference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("DyszaWahliwaP4")
                        .HasColumnType("bit");

                    b.Property<float>("DyszaWahliwa_PozPionowa")
                        .HasColumnType("real");

                    b.Property<float>("DyszaWahliwa_PozPozioma")
                        .HasColumnType("real");

                    b.Property<float>("DyszaWahliwa_PozZjazduOslonkiSmarowanie")
                        .HasColumnType("real");

                    b.Property<float>("DyszaWahliwa_PozdyszywOslonce")
                        .HasColumnType("real");

                    b.Property<bool>("MontazOslonkiP2")
                        .HasColumnType("bit");

                    b.Property<string>("NameOfClient")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("OetickerP3")
                        .HasColumnType("bit");

                    b.Property<float>("Oslonka_PozNakladania")
                        .HasColumnType("real");

                    b.Property<float>("Oslonka_PozPowrotu")
                        .HasColumnType("real");

                    b.Property<float>("Oslonka_PozSmarowania")
                        .HasColumnType("real");

                    b.Property<float>("Oslonka_PozStartowa")
                        .HasColumnType("real");

                    b.Property<float>("PozOetickera")
                        .HasColumnType("real");

                    b.Property<float>("PozwyjeciaOsi")
                        .HasColumnType("real");

                    b.Property<bool>("PrzeciskanieP1")
                        .HasColumnType("bit");

                    b.Property<float>("Przeciskanie_DojazdWolny")
                        .HasColumnType("real");

                    b.Property<float>("Przeciskanie_KoniecCzytSily")
                        .HasColumnType("real");

                    b.Property<float>("Przeciskanie_PoczCzytSily")
                        .HasColumnType("real");

                    b.Property<float>("Przeciskanie_PozStartowa")
                        .HasColumnType("real");

                    b.Property<int>("Przeciskanie_SilaMax")
                        .HasColumnType("int");

                    b.Property<int>("Przeciskanie_SilaMin")
                        .HasColumnType("int");

                    b.Property<bool>("RFIDGlowicaGornaP9")
                        .HasColumnType("bit");

                    b.Property<bool>("RFIDGniazdoPrzegubuP12")
                        .HasColumnType("bit");

                    b.Property<bool>("RFIDPlytaSmarujacaP10")
                        .HasColumnType("bit");

                    b.Property<bool>("RFIDSzczekiOslonkiP11")
                        .HasColumnType("bit");

                    b.Property<string>("ReferenceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("SmarPrzegubP6")
                        .HasColumnType("bit");

                    b.Property<bool>("SmarTulipP5")
                        .HasColumnType("bit");

                    b.Property<float>("Smarowanie_DawkaPrzegub")
                        .HasColumnType("real");

                    b.Property<float>("Smarowanie_DawkaTulip")
                        .HasColumnType("real");

                    b.Property<string>("Smarowanie_RodzajSmaruPrzegub")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Smarowanie_RodzajSmaruTulip")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Smarowanie_TolDawkiPrzegub")
                        .HasColumnType("real");

                    b.Property<float>("Smarowanie_TolDawkiTulip")
                        .HasColumnType("real");

                    b.Property<bool>("SpareP13")
                        .HasColumnType("bit");

                    b.Property<bool>("SpareP14")
                        .HasColumnType("bit");

                    b.Property<bool>("SpareP15")
                        .HasColumnType("bit");

                    b.Property<bool>("SpareP16")
                        .HasColumnType("bit");

                    b.Property<int>("TagRFID_GlowicaGorna")
                        .HasColumnType("int");

                    b.Property<int>("TagRFID_PlytaSmar")
                        .HasColumnType("int");

                    b.Property<int>("TagRFID_Przegub")
                        .HasColumnType("int");

                    b.Property<int>("TagRFID_SzczekiOslonki")
                        .HasColumnType("int");

                    b.Property<bool>("TraceUpP7")
                        .HasColumnType("bit");

                    b.Property<bool>("TraceUpZapisP8")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("References");
                });
#pragma warning restore 612, 618
        }
    }
}

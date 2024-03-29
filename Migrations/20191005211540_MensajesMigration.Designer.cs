﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestApiDating.Data;

namespace RestApiDating.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20191005211540_MensajesMigration")]
    partial class MensajesMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("RestApiDating.Models.Foto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Descripcion");

                    b.Property<bool>("EsPrincipal");

                    b.Property<DateTime>("FechaCarga");

                    b.Property<string>("IdPublico");

                    b.Property<string>("Url");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Fotos");
                });

            modelBuilder.Entity("RestApiDating.Models.Like", b =>
                {
                    b.Property<int>("LikerId");

                    b.Property<int>("LikedId");

                    b.HasKey("LikerId", "LikedId");

                    b.HasIndex("LikedId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("RestApiDating.Models.Mensaje", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Contenido");

                    b.Property<int>("EmisorId");

                    b.Property<DateTime>("FechaEnvio");

                    b.Property<DateTime?>("FechaLectura");

                    b.Property<bool>("HaSidoEliminadoPorEmisor");

                    b.Property<bool>("HaSidoEliminadoPorReceptor");

                    b.Property<bool>("HaSidoLeido");

                    b.Property<int>("ReceptorId");

                    b.HasKey("Id");

                    b.HasIndex("EmisorId");

                    b.HasIndex("ReceptorId");

                    b.ToTable("Mensajes");
                });

            modelBuilder.Entity("RestApiDating.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Apellido");

                    b.Property<string>("Buscando");

                    b.Property<string>("Ciudad");

                    b.Property<DateTime>("FechaCreacion");

                    b.Property<DateTime?>("FechaNacimiento");

                    b.Property<string>("Genero");

                    b.Property<string>("Intereses");

                    b.Property<string>("Introduccion");

                    b.Property<string>("Nombre");

                    b.Property<string>("Pais");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<DateTime?>("UltimaConexion");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RestApiDating.Models.Value", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("RestApiDating.Models.Foto", b =>
                {
                    b.HasOne("RestApiDating.Models.User", "User")
                        .WithMany("Fotos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RestApiDating.Models.Like", b =>
                {
                    b.HasOne("RestApiDating.Models.User", "Liked")
                        .WithMany("Likers")
                        .HasForeignKey("LikedId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("RestApiDating.Models.User", "Liker")
                        .WithMany("Likes")
                        .HasForeignKey("LikerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("RestApiDating.Models.Mensaje", b =>
                {
                    b.HasOne("RestApiDating.Models.User", "Emisor")
                        .WithMany("MensajesEnviados")
                        .HasForeignKey("EmisorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("RestApiDating.Models.User", "Receptor")
                        .WithMany("MensajesRecibidos")
                        .HasForeignKey("ReceptorId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}

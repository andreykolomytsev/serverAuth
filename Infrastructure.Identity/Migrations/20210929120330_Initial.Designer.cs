﻿// <auto-generated />
using System;
using Infrastructure.Identity.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Infrastructure.Identity.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210929120330_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelAccessToken", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CreatedByBrowser")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByIp")
                        .HasColumnType("text");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsOutDated")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AccessTokens", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelPermission", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Permissions", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelRefreshToken", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("CreatedByBrowser")
                        .HasColumnType("text");

                    b.Property<string>("CreatedByIp")
                        .HasColumnType("text");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ReplacedByToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("RevokedByIp")
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Roles", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelService", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("IP")
                        .HasColumnType("text");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Port")
                        .HasColumnType("text");

                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Services", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelTenant", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("INN")
                        .HasColumnType("text");

                    b.Property<string>("KPP")
                        .HasColumnType("text");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("OGRN")
                        .HasColumnType("text");

                    b.Property<string>("OKPO")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tenants", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("MiddleName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Users", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.PermissionRole", b =>
                {
                    b.Property<string>("PermissionId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("PermissionId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("PermissionRoles", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.PermissionTenant", b =>
                {
                    b.Property<string>("PermissionId")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("PermissionId", "TenantId");

                    b.HasIndex("TenantId");

                    b.ToTable("PermissionTenants", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.TenantService", b =>
                {
                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("MicroServiceId")
                        .HasColumnType("text");

                    b.HasKey("TenantId", "MicroServiceId");

                    b.HasIndex("MicroServiceId");

                    b.ToTable("TenantServices", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.UserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.UserService", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("MicroServiceId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "MicroServiceId");

                    b.HasIndex("MicroServiceId");

                    b.ToTable("UserServices", "Identity");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelAccessToken", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelUser", "User")
                        .WithMany("AccessTokens")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelRefreshToken", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelUser", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelRole", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelTenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelUser", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelTenant", "Tenant")
                        .WithMany("Users")
                        .HasForeignKey("TenantId");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.PermissionRole", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelPermission", "Permission")
                        .WithMany("Roles")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Identity.Models.ModelRole", "Role")
                        .WithMany("Permissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.PermissionTenant", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelPermission", "Permission")
                        .WithMany("Tenants")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Identity.Models.ModelTenant", "Tenant")
                        .WithMany("Permissions")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.TenantService", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelService", "MicroService")
                        .WithMany("Tenants")
                        .HasForeignKey("MicroServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Identity.Models.ModelTenant", "Tenant")
                        .WithMany("Services")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MicroService");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.UserRole", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelRole", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Identity.Models.ModelUser", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.UserService", b =>
                {
                    b.HasOne("Infrastructure.Identity.Models.ModelService", "MicroService")
                        .WithMany("Users")
                        .HasForeignKey("MicroServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Infrastructure.Identity.Models.ModelUser", "User")
                        .WithMany("UserServices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MicroService");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelPermission", b =>
                {
                    b.Navigation("Roles");

                    b.Navigation("Tenants");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelRole", b =>
                {
                    b.Navigation("Permissions");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelService", b =>
                {
                    b.Navigation("Tenants");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelTenant", b =>
                {
                    b.Navigation("Permissions");

                    b.Navigation("Services");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Infrastructure.Identity.Models.ModelUser", b =>
                {
                    b.Navigation("AccessTokens");

                    b.Navigation("RefreshTokens");

                    b.Navigation("Roles");

                    b.Navigation("UserServices");
                });
#pragma warning restore 612, 618
        }
    }
}
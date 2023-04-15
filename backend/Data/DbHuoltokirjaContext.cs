using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public partial class DbHuoltokirjaContext : DbContext
{
    public DbHuoltokirjaContext()
    {
    }

    public DbHuoltokirjaContext(DbContextOptions<DbHuoltokirjaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auditointi> Auditointis { get; set; }

    public virtual DbSet<Auditointipohja> Auditointipohjas { get; set; }

    public virtual DbSet<Kayttaja> Kayttajas { get; set; }

    public virtual DbSet<Kohde> Kohdes { get; set; }

    public virtual DbSet<Kohderyhma> Kohderyhmas { get; set; }

    public virtual DbSet<KohteenTila> KohteenTilas { get; set; }

    public virtual DbSet<Liite> Liites { get; set; }

    public virtual DbSet<MuutoshistoriaKohde> MuutoshistoriaKohdes { get; set; }

    public virtual DbSet<Tarkastu> Tarkastus { get; set; }

    public virtual DbSet<Vaatimu> Vaatimus { get; set; }

    public virtual DbSet<Vaatimuspohja> Vaatimuspohjas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("poistettu"); // POISTETTU AZURE SQL CONNECTION

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auditointi>(entity =>
        {
            entity.HasKey(e => e.Idauditointi).HasName("PK_auditointi_idauditointi");

            entity.ToTable("auditointi", "huoltokirja");

            entity.HasIndex(e => e.Idkayttaja, "fk_auditointi_kayttaja1_idx");

            entity.HasIndex(e => e.Idkohde, "fk_auditointi_kohde1_idx");

            entity.Property(e => e.Idauditointi).HasColumnName("idauditointi");
            entity.Property(e => e.Idkayttaja).HasColumnName("idkayttaja");
            entity.Property(e => e.Idkohde).HasColumnName("idkohde");
            entity.Property(e => e.Lopputulos).HasColumnName("lopputulos");
            entity.Property(e => e.Luotu)
                .HasPrecision(0)
                .HasColumnName("luotu");
            entity.Property(e => e.Selite)
                .HasMaxLength(45)
                .HasColumnName("selite");

            entity.HasOne(d => d.IdkayttajaNavigation).WithMany(p => p.Auditointis)
                .HasForeignKey(d => d.Idkayttaja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("auditointi$fk_auditointi_kayttaja1");

            entity.HasOne(d => d.IdkohdeNavigation).WithMany(p => p.Auditointis)
                .HasForeignKey(d => d.Idkohde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("auditointi$fk_auditointi_kohde1");
        });

        modelBuilder.Entity<Auditointipohja>(entity =>
        {
            entity.HasKey(e => e.Idauditointipohja).HasName("PK_auditointipohja_idauditointipohja");

            entity.ToTable("auditointipohja", "huoltokirja");

            entity.HasIndex(e => e.Idkayttaja, "fk_auditointipohja_kayttaja1_idx");

            entity.HasIndex(e => e.Idkohderyhma, "fk_auditointipohja_kohderyhma1_idx");

            entity.Property(e => e.Idauditointipohja).HasColumnName("idauditointipohja");
            entity.Property(e => e.Idkayttaja).HasColumnName("idkayttaja");
            entity.Property(e => e.Idkohderyhma).HasColumnName("idkohderyhma");
            entity.Property(e => e.Luontiaika)
                .HasPrecision(0)
                .HasColumnName("luontiaika");
            entity.Property(e => e.Selite)
                .HasMaxLength(45)
                .HasColumnName("selite");

            entity.HasOne(d => d.IdkayttajaNavigation).WithMany(p => p.Auditointipohjas)
                .HasForeignKey(d => d.Idkayttaja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("auditointipohja$fk_auditointipohja_kayttaja1");

            entity.HasOne(d => d.IdkohderyhmaNavigation).WithMany(p => p.Auditointipohjas)
                .HasForeignKey(d => d.Idkohderyhma)
                .HasConstraintName("auditointipohja$fk_auditointipohja_kohderyhma1");
        });

        modelBuilder.Entity<Kayttaja>(entity =>
        {
            entity.HasKey(e => e.Idkayttaja).HasName("PK_kayttaja_idkayttaja");

            entity.ToTable("kayttaja", "huoltokirja");

            entity.HasIndex(e => e.Idkayttaja, "kayttaja$idkayttaja_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Kayttajatunnus, "kayttaja$kayttajatunnus_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Salasana, "kayttaja$salasana_UNIQUE").IsUnique();

            entity.Property(e => e.Idkayttaja).HasColumnName("idkayttaja");
            entity.Property(e => e.Kayttajatunnus)
                .HasMaxLength(45)
                .HasColumnName("kayttajatunnus");
            entity.Property(e => e.Luotu)
                .HasPrecision(0)
                .HasColumnName("luotu");
            entity.Property(e => e.Nimi)
                .HasMaxLength(45)
                .HasColumnName("nimi");
            entity.Property(e => e.Poistettu).HasColumnName("poistettu");
            entity.Property(e => e.Rooli)
                .HasMaxLength(10)
                .HasColumnName("rooli");
            entity.Property(e => e.Salasana)
                .HasMaxLength(200)
                .HasColumnName("salasana");
            entity.Property(e => e.ViimeisinKirjautuminen)
                .HasPrecision(0)
                .HasColumnName("viimeisin_kirjautuminen");
        });

        modelBuilder.Entity<Kohde>(entity =>
        {
            entity.HasKey(e => e.Idkohde).HasName("PK_kohde_idkohde");

            entity.ToTable("kohde", "huoltokirja");

            entity.HasIndex(e => e.Idkayttaja, "fk_kohde_kayttaja_idx");

            entity.HasIndex(e => e.Idkohderyhma, "fk_kohde_kohderyhma1_idx");

            entity.HasIndex(e => e.IdkohteenTila, "fk_kohde_kohteen_tila1_idx");

            entity.HasIndex(e => e.Idkohde, "kohde$idkohde_UNIQUE").IsUnique();

            entity.Property(e => e.Idkohde).HasColumnName("idkohde");
            entity.Property(e => e.Idkayttaja).HasColumnName("idkayttaja");
            entity.Property(e => e.Idkohderyhma).HasColumnName("idkohderyhma");
            entity.Property(e => e.IdkohteenTila).HasColumnName("idkohteen_tila");
            entity.Property(e => e.Kuvaus)
                .HasMaxLength(45)
                .HasColumnName("kuvaus");
            entity.Property(e => e.Luotu)
                .HasPrecision(0)
                .HasColumnName("luotu");
            entity.Property(e => e.Malli)
                .HasMaxLength(45)
                .HasColumnName("malli");
            entity.Property(e => e.Nimi)
                .HasMaxLength(45)
                .HasColumnName("nimi");
            entity.Property(e => e.Sijainti)
                .HasMaxLength(45)
                .HasColumnName("sijainti");
            entity.Property(e => e.Tunnus)
                .HasMaxLength(45)
                .HasColumnName("tunnus");
            entity.Property(e => e.Tyyppi)
                .HasMaxLength(45)
                .HasColumnName("tyyppi");

            entity.HasOne(d => d.IdkayttajaNavigation).WithMany(p => p.Kohdes)
                .HasForeignKey(d => d.Idkayttaja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("kohde$fk_kohde_kayttaja");

            entity.HasOne(d => d.IdkohderyhmaNavigation).WithMany(p => p.Kohdes)
                .HasForeignKey(d => d.Idkohderyhma)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("kohde$fk_kohde_kohderyhma1");

            entity.HasOne(d => d.IdkohteenTilaNavigation).WithMany(p => p.Kohdes)
                .HasForeignKey(d => d.IdkohteenTila)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("kohde$fk_kohde_kohteen_tila1");
        });

        modelBuilder.Entity<Kohderyhma>(entity =>
        {
            entity.HasKey(e => e.Idkohderyhma).HasName("PK_kohderyhma_idkohderyhma");

            entity.ToTable("kohderyhma", "huoltokirja");

            entity.Property(e => e.Idkohderyhma).HasColumnName("idkohderyhma");
            entity.Property(e => e.Nimi)
                .HasMaxLength(45)
                .HasColumnName("nimi");
        });

        modelBuilder.Entity<KohteenTila>(entity =>
        {
            entity.HasKey(e => e.IdkohteenTila).HasName("PK_kohteen_tila_idkohteen_tila");

            entity.ToTable("kohteen_tila", "huoltokirja");

            entity.Property(e => e.IdkohteenTila)
                .ValueGeneratedNever()
                .HasColumnName("idkohteen_tila");
            entity.Property(e => e.Kuvaus)
                .HasMaxLength(45)
                .HasColumnName("kuvaus");
        });

        modelBuilder.Entity<Liite>(entity =>
        {
            entity.HasKey(e => e.Idliite).HasName("PK_liite_idliite");

            entity.ToTable("liite", "huoltokirja");

            entity.HasIndex(e => e.Idtarkastus, "fk_liite_tarkastus1_idx");

            entity.Property(e => e.Idliite).HasColumnName("idliite");
            entity.Property(e => e.Idtarkastus).HasColumnName("idtarkastus");
            entity.Property(e => e.Sijainti)
                .HasMaxLength(150)
                .HasColumnName("sijainti");

            entity.HasOne(d => d.IdtarkastusNavigation).WithMany(p => p.Liites)
                .HasForeignKey(d => d.Idtarkastus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("liite$fk_liite_tarkastus1");
        });

        modelBuilder.Entity<MuutoshistoriaKohde>(entity =>
        {
            entity.HasKey(e => e.IdmuutoshistoriaKohde).HasName("PK_muutoshistoria_kohde_idmuutoshistoria_kohde");

            entity.ToTable("muutoshistoria_kohde", "huoltokirja");

            entity.HasIndex(e => e.KayttajaIdkayttaja, "fk_muutoshistoria_kohde_kayttaja1_idx");

            entity.HasIndex(e => e.KohdeIdkohde, "fk_muutoshistoria_kohde_kohde1_idx");

            entity.HasIndex(e => e.IdkohteenTila, "fk_muutoshistoria_kohde_kohteen_tila1_idx");

            entity.HasIndex(e => e.IdmuutoshistoriaKohde, "muutoshistoria_kohde$idkohde_UNIQUE").IsUnique();

            entity.Property(e => e.IdmuutoshistoriaKohde).HasColumnName("idmuutoshistoria_kohde");
            entity.Property(e => e.IdkohteenTila).HasColumnName("idkohteen_tila");
            entity.Property(e => e.KayttajaIdkayttaja).HasColumnName("kayttaja_idkayttaja");
            entity.Property(e => e.KohdeIdkohde).HasColumnName("kohde_idkohde");
            entity.Property(e => e.Kuvaus)
                .HasMaxLength(45)
                .HasColumnName("kuvaus");
            entity.Property(e => e.Muokattu)
                .HasPrecision(0)
                .HasColumnName("muokattu");
            entity.Property(e => e.Nimi)
                .HasMaxLength(45)
                .HasColumnName("nimi");
            entity.Property(e => e.Sijainti)
                .HasMaxLength(45)
                .HasColumnName("sijainti");
            entity.Property(e => e.Tunnus)
                .HasMaxLength(45)
                .HasColumnName("tunnus");

            entity.HasOne(d => d.IdkohteenTilaNavigation).WithMany(p => p.MuutoshistoriaKohdes)
                .HasForeignKey(d => d.IdkohteenTila)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("muutoshistoria_kohde$fk_muutoshistoria_kohde_kohteen_tila1");

            entity.HasOne(d => d.KayttajaIdkayttajaNavigation).WithMany(p => p.MuutoshistoriaKohdes)
                .HasForeignKey(d => d.KayttajaIdkayttaja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("muutoshistoria_kohde$fk_muutoshistoria_kohde_kayttaja1");

            entity.HasOne(d => d.KohdeIdkohdeNavigation).WithMany(p => p.MuutoshistoriaKohdes)
                .HasForeignKey(d => d.KohdeIdkohde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("muutoshistoria_kohde$fk_muutoshistoria_kohde_kohde1");
        });

        modelBuilder.Entity<Tarkastu>(entity =>
        {
            entity.HasKey(e => e.Idtarkastus).HasName("PK_tarkastus_idtarkastus");

            entity.ToTable("tarkastus", "huoltokirja");

            entity.HasIndex(e => e.Idkayttaja, "fk_tarkastus_kayttaja1_idx");

            entity.HasIndex(e => e.Idkohde, "fk_tarkastus_kohde1_idx");

            entity.Property(e => e.Idtarkastus).HasColumnName("idtarkastus");
            entity.Property(e => e.Aikaleima)
                .HasPrecision(0)
                .HasColumnName("aikaleima");
            entity.Property(e => e.Havainnot)
                .HasMaxLength(300)
                .HasColumnName("havainnot");
            entity.Property(e => e.Idkayttaja).HasColumnName("idkayttaja");
            entity.Property(e => e.Idkohde).HasColumnName("idkohde");
            entity.Property(e => e.Syy)
                .HasMaxLength(45)
                .HasColumnName("syy");
            entity.Property(e => e.TilanMuutos).HasColumnName("tilan_muutos");

            entity.HasOne(d => d.IdkayttajaNavigation).WithMany(p => p.Tarkastus)
                .HasForeignKey(d => d.Idkayttaja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tarkastus$fk_tarkastus_kayttaja1");

            entity.HasOne(d => d.IdkohdeNavigation).WithMany(p => p.Tarkastus)
                .HasForeignKey(d => d.Idkohde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tarkastus$fk_tarkastus_kohde1");
        });

        modelBuilder.Entity<Vaatimu>(entity =>
        {
            entity.HasKey(e => e.Idvaatimus).HasName("PK_vaatimus_idvaatimus");

            entity.ToTable("vaatimus", "huoltokirja");

            entity.HasIndex(e => e.Idauditointi, "fk_vaatimus_auditointi1_idx");

            entity.Property(e => e.Idvaatimus).HasColumnName("idvaatimus");
            entity.Property(e => e.Idauditointi).HasColumnName("idauditointi");
            entity.Property(e => e.Kuvaus)
                .HasMaxLength(45)
                .HasColumnName("kuvaus");
            entity.Property(e => e.Pakollisuus)
                .HasMaxLength(45)
                .HasColumnName("pakollisuus");
            entity.Property(e => e.Taytetty).HasColumnName("taytetty");

            entity.HasOne(d => d.IdauditointiNavigation).WithMany(p => p.Vaatimus)
                .HasForeignKey(d => d.Idauditointi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vaatimus$fk_vaatimus_auditointi1");
        });

        modelBuilder.Entity<Vaatimuspohja>(entity =>
        {
            entity.HasKey(e => e.Idvaatimuspohja).HasName("PK_vaatimuspohja_idvaatimuspohja");

            entity.ToTable("vaatimuspohja", "huoltokirja");

            entity.HasIndex(e => e.Idauditointipohja, "fk_vaatimus_auditointipohja1_idx");

            entity.Property(e => e.Idvaatimuspohja).HasColumnName("idvaatimuspohja");
            entity.Property(e => e.Idauditointipohja).HasColumnName("idauditointipohja");
            entity.Property(e => e.Kuvaus)
                .HasMaxLength(45)
                .HasColumnName("kuvaus");
            entity.Property(e => e.Pakollisuus)
                .HasMaxLength(45)
                .HasColumnName("pakollisuus");

            entity.HasOne(d => d.IdauditointipohjaNavigation).WithMany(p => p.Vaatimuspohjas)
                .HasForeignKey(d => d.Idauditointipohja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vaatimuspohja$fk_vaatimus_auditointipohja1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

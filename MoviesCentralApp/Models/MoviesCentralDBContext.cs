using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MoviesCentralApp.Models;

namespace MoviesCentralApp.Models;

public partial class MoviesCentralDBContext : DbContext
{
    public MoviesCentralDBContext()
    {
    }

    public MoviesCentralDBContext(DbContextOptions<MoviesCentralDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MoviesActor> MoviesActors { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=2EKATHIN15\\SQLEXPRESS;Database=MoviesCentralDb;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.Actorid).HasName("PK__Actors__83335D33E58D6E85");

            entity.Property(e => e.Actorid).ValueGeneratedNever();
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Commentid).HasName("PK__Comments__CDA84BC5D8F8BFCF");

            entity.Property(e => e.Commentid).ValueGeneratedNever();

            entity.HasOne(d => d.Movie).WithMany(p => p.Comments).HasConstraintName("FK__Comments__moviei__2F10007B");

            entity.HasOne(d => d.User).WithMany(p => p.Comments).HasConstraintName("FK__Comments__userid__2E1BDC42");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Movieid).HasName("PK__Movies__42EACB66D2A9532C");

            entity.Property(e => e.Movieid).ValueGeneratedNever();
        });

        modelBuilder.Entity<MoviesActor>(entity =>
        {
            entity.HasKey(e => e.Maid).HasName("PK__MoviesAc__7A21293D2539DE1F");

            entity.Property(e => e.Maid).ValueGeneratedNever();

            entity.HasOne(d => d.Actor).WithMany(p => p.MoviesActors).HasConstraintName("FK__MoviesAct__actor__2B3F6F97");

            entity.HasOne(d => d.Movie).WithMany(p => p.MoviesActors).HasConstraintName("FK__MoviesAct__movie__2A4B4B5E");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Ratingid).HasName("PK__Ratings__2D2E08C1DFC6B698");

            entity.Property(e => e.Ratingid).ValueGeneratedNever();

            entity.HasOne(d => d.Movie).WithMany(p => p.Ratings).HasConstraintName("FK__Ratings__movieid__31EC6D26");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings).HasConstraintName("FK__Ratings__userid__32E0915F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PK__Users__CBA1B2571CFF1022");

            entity.Property(e => e.Userid).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<MoviesCentralApp.Models.MyLogin> MyLogin { get; set; } = default!;
}

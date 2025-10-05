using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MoviesCentralApp.Models;

public partial class Movie
{
    [Key]
    [Column("movieid")]
    public int Movieid { get; set; }

    [Column("title")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Title { get; set; }

    [Column("release_date")]
    public DateOnly? ReleaseDate { get; set; }

    [Column("genre")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Genre { get; set; }

    [Column("director")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Director { get; set; }

    [Column("description")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Description { get; set; }

    [Column("posturl")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Posturl { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("Movie")]
    public virtual ICollection<MoviesActor> MoviesActors { get; set; } = new List<MoviesActor>();

    [InverseProperty("Movie")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MoviesCentralApp.Models;

public partial class Actor
{
    [Key]
    [Column("actorid")]
    public int Actorid { get; set; }

    [Column("fullname")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Fullname { get; set; }

    [Column("photourl")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Photourl { get; set; }

    [InverseProperty("Actor")]
    public virtual ICollection<MoviesActor> MoviesActors { get; set; } = new List<MoviesActor>();
}

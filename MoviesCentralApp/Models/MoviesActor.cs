using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MoviesCentralApp.Models;

public partial class MoviesActor
{
    [Key]
    [Column("maid")]
    public int Maid { get; set; }

    [Column("movieid")]
    public int? Movieid { get; set; }

    [Column("actorid")]
    public int? Actorid { get; set; }

    [ForeignKey("Actorid")]
    [InverseProperty("MoviesActors")]
    public virtual Actor? Actor { get; set; }

    [ForeignKey("Movieid")]
    [InverseProperty("MoviesActors")]
    public virtual Movie? Movie { get; set; }

    
}

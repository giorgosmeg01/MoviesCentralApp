using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MoviesCentralApp.Models;

public partial class Rating
{
    [Key]
    [Column("ratingid")]
    public int Ratingid { get; set; }

    [Column("movieid")]
    public int? Movieid { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("rating")]
    public int? Rating1 { get; set; }

    [ForeignKey("Movieid")]
    [InverseProperty("Ratings")]
    public virtual Movie? Movie { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Ratings")]
    public virtual User? User { get; set; }
}

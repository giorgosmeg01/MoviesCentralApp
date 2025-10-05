using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MoviesCentralApp.Models;

public partial class Comment
{
    [Key]
    [Column("commentid")]
    public int Commentid { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("movieid")]
    public int? Movieid { get; set; }

    [Column("comment")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Comment1 { get; set; }

    [ForeignKey("Movieid")]
    [InverseProperty("Comments")]
    public virtual Movie? Movie { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Comments")]
    public virtual User? User { get; set; }
}

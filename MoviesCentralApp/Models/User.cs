using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MoviesCentralApp.Models;

public partial class User
{
    [Key]
    [Column("userid")]
    public int Userid { get; set; }

    [Column("username")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Username { get; set; }

    [Column("email")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("password")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Password { get; set; }

    [Column("role")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Role { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty("User")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}

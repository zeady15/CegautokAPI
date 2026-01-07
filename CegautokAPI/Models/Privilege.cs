using System;
using System.Collections.Generic;

namespace CegautokAPI.Models;

public partial class Privilege
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Level { get; set; }

    public virtual ICollection<User>? Users { get; set; } = new List<User>();
}

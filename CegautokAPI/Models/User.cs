using System;
using System.Collections.Generic;

namespace CegautokAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LoginName { get; set; } = null!;

    public bool Active { get; set; }

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string? Phone { get; set; }

    public string Image { get; set; } = null!;

    public int Permission { get; set; }

    public virtual ICollection<Kikuldottjarmu>? Kikuldottjarmus { get; set; } = new List<Kikuldottjarmu>();

    public virtual Privilege? PermissionNavigation { get; set; } = null!;
}

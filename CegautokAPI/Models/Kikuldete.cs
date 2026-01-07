using System;
using System.Collections.Generic;

namespace CegautokAPI.Models;

public partial class Kikuldete
{
    public int Id { get; set; }

    public string Celja { get; set; } = null!;

    public string Cim { get; set; } = null!;

    public DateTime Kezdes { get; set; }

    public DateTime Befejezes { get; set; }

    public virtual ICollection<Kikuldottjarmu>? Kikuldottjarmus { get; set; } = new List<Kikuldottjarmu>();
}

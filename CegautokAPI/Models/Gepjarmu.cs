using System;
using System.Collections.Generic;

namespace CegautokAPI.Models;

public partial class Gepjarmu
{
    public int Id { get; set; }

    public string Rendszam { get; set; } = null!;

    public string Marka { get; set; } = null!;

    public string Tipus { get; set; } = null!;

    public int Ulesek { get; set; }

    public virtual ICollection<Kikuldottjarmu>? Kikuldottjarmus { get; set; } = new List<Kikuldottjarmu>();
}

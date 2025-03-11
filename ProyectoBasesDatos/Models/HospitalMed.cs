using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class HospitalMed
{
    public string Id { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string HospitalId { get; set; } = null!;

    public virtual Hospital Hospital { get; set; } = null!;

    public virtual ICollection<Medication> Medications { get; set; } = new List<Medication>();
}

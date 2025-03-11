using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class Specialty
{
    public string Id { get; set; } = null!;

    public string SpeName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}

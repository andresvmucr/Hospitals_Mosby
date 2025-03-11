using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class Hospital
{
    public string Id { get; set; } = null!;

    public string HName { get; set; } = null!;

    public string HAddress { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<HospitalMed> HospitalMeds { get; set; } = new List<HospitalMed>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

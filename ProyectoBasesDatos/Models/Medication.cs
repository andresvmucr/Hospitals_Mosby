using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class Medication
{
    public string Id { get; set; } = null!;

    public string MName { get; set; } = null!;

    public string MDescription { get; set; } = null!;

    public string HospitalMedsId { get; set; } = null!;

    public virtual HospitalMed HospitalMeds { get; set; } = null!;

    public virtual ICollection<TreatmentMed> TreatmentMeds { get; set; } = new List<TreatmentMed>();
}

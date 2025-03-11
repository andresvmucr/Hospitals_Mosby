using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class TreatmentMed
{
    public string Id { get; set; } = null!;

    public string Dosis { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public DateOnly TDate { get; set; }

    public string TreatmentId { get; set; } = null!;

    public string MedicationId { get; set; } = null!;

    public virtual Medication Medication { get; set; } = null!;

    public virtual Treatment Treatment { get; set; } = null!;
}

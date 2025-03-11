using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class Treatment
{
    public string Id { get; set; } = null!;

    public decimal Price { get; set; }

    public string AppointmentId { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual ICollection<TreatmentMed> TreatmentMeds { get; set; } = new List<TreatmentMed>();
}

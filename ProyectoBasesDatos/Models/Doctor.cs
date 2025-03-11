using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class Doctor
{
    public string Id { get; set; } = null!;

    public string? SpecialtyId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Specialty? Specialty { get; set; }

    public virtual ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
}

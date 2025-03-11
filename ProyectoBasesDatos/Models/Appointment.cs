using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class Appointment
{
    public string Id { get; set; } = null!;

    public DateOnly ADay { get; set; }

    public DateTime AHour { get; set; }

    public string AStatus { get; set; } = null!;

    public string PatientId { get; set; } = null!;

    public string DoctorId { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual User Patient { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
}

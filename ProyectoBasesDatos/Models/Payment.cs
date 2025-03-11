using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class Payment
{
    public string Id { get; set; } = null!;

    public DateOnly PDate { get; set; }

    public decimal Price { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PatientId { get; set; } = null!;

    public string AppointmentId { get; set; } = null!;

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual User Patient { get; set; } = null!;
}

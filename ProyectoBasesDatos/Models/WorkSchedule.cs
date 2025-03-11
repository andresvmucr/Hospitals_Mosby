using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class WorkSchedule
{
    public string WDay { get; set; } = null!;

    public DateTime Starthour { get; set; }

    public DateTime Endhour { get; set; }

    public string DoctorId { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ProyectoBasesDatos.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public string UName { get; set; } = null!;

    public string Firstlastname { get; set; } = null!;

    public string Secondlastname { get; set; } = null!;

    public DateOnly Birthdate { get; set; }

    public string Password { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? HospitalId { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Hospital? Hospital { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

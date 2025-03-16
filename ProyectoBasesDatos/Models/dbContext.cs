using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBasesDatos.Models;

public partial class dbContext : DbContext
{
    public dbContext(DbContextOptions<dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<HospitalMed> HospitalMeds { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Treatment> Treatments { get; set; }

    public virtual DbSet<TreatmentMed> TreatmentMeds { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkSchedule> WorkSchedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__appointm__3213E83F51E73BCE");

            entity.ToTable("appointments");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.ADay).HasColumnName("a_day");
            entity.Property(e => e.AHour)
                .HasColumnType("datetime")
                .HasColumnName("a_hour");
            entity.Property(e => e.AStatus)
                .HasMaxLength(10)
                .HasDefaultValue("Pending")
                .HasColumnName("a_status");
            entity.Property(e => e.DoctorId)
                .HasMaxLength(9)
                .HasColumnName("doctor_id");
            entity.Property(e => e.PatientId)
                .HasMaxLength(9)
                .HasColumnName("patient_id");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__appointme__docto__4BAC3F29");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__appointme__patie__4AB81AF0");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__doctors__3213E83F1FCC24BA");

            entity.ToTable("doctors");

            entity.Property(e => e.Id)
                .HasMaxLength(9)
                .HasColumnName("id");
            entity.Property(e => e.IdDoctor)
                .HasMaxLength(9)
                .HasColumnName("id_doctor");
            entity.Property(e => e.SpecialtyId)
                .HasMaxLength(30)
                .HasColumnName("specialty_id");

            entity.HasOne(d => d.IdDoctorNavigation).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.IdDoctor)
                .HasConstraintName("FK__doctors__id_doct__5CD6CB2B");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .HasConstraintName("FK__doctors__special__4316F928");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__hospital__3213E83FF6BFEF8C");

            entity.ToTable("hospitals");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.HAddress)
                .HasMaxLength(200)
                .HasColumnName("h_address");
            entity.Property(e => e.HName)
                .HasMaxLength(100)
                .HasColumnName("h_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(8)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<HospitalMed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__hospital__3213E83FEA83127C");

            entity.ToTable("hospital_meds");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.HospitalId)
                .HasMaxLength(30)
                .HasColumnName("hospital_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.Hospital).WithMany(p => p.HospitalMeds)
                .HasForeignKey(d => d.HospitalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__hospital___hospi__398D8EEE");
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__medicati__3213E83FE95774F0");

            entity.ToTable("medications");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.HospitalMedsId)
                .HasMaxLength(30)
                .HasColumnName("hospital_meds_id");
            entity.Property(e => e.MDescription)
                .HasMaxLength(300)
                .HasColumnName("m_description");
            entity.Property(e => e.MName)
                .HasMaxLength(100)
                .HasColumnName("m_name");

            entity.HasOne(d => d.HospitalMeds).WithMany(p => p.Medications)
                .HasForeignKey(d => d.HospitalMedsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__medicatio__hospi__4E88ABD4");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83F91AC1B56");

            entity.ToTable("payments");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.AppointmentId)
                .HasMaxLength(30)
                .HasColumnName("appointment_id");
            entity.Property(e => e.PDate).HasColumnName("p_date");
            entity.Property(e => e.PatientId)
                .HasMaxLength(9)
                .HasColumnName("patient_id");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(15)
                .HasColumnName("payment_method");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payments__appoin__59063A47");

            entity.HasOne(d => d.Patient).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__payments__patien__5812160E");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__specialt__3213E83FEDB497EB");

            entity.ToTable("specialties");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.SpeName)
                .HasMaxLength(100)
                .HasColumnName("spe_name");
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__treatmen__3213E83F00C9CCDB");

            entity.ToTable("treatments");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.AppointmentId)
                .HasMaxLength(30)
                .HasColumnName("appointment_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Treatments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__treatment__appoi__5165187F");
        });

        modelBuilder.Entity<TreatmentMed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__treatmen__3213E83FD78F6824");

            entity.ToTable("treatment_meds");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .HasColumnName("id");
            entity.Property(e => e.Dosis)
                .HasMaxLength(200)
                .HasColumnName("dosis");
            entity.Property(e => e.Frequency)
                .HasMaxLength(200)
                .HasColumnName("frequency");
            entity.Property(e => e.MedicationId)
                .HasMaxLength(30)
                .HasColumnName("medication_id");
            entity.Property(e => e.TDate).HasColumnName("t_date");
            entity.Property(e => e.TreatmentId)
                .HasMaxLength(30)
                .HasColumnName("treatment_id");

            entity.HasOne(d => d.Medication).WithMany(p => p.TreatmentMeds)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__treatment__medic__5535A963");

            entity.HasOne(d => d.Treatment).WithMany(p => p.TreatmentMeds)
                .HasForeignKey(d => d.TreatmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__treatment__treat__5441852A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FA3C8E8F8");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasMaxLength(9)
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Firstlastname)
                .HasMaxLength(100)
                .HasColumnName("firstlastname");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.HospitalId)
                .HasMaxLength(30)
                .HasColumnName("hospital_id");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(8)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .HasColumnName("role");
            entity.Property(e => e.Secondlastname)
                .HasMaxLength(100)
                .HasColumnName("secondlastname");
            entity.Property(e => e.UName)
                .HasMaxLength(100)
                .HasColumnName("u_name");

            entity.HasOne(d => d.Hospital).WithMany(p => p.Users)
                .HasForeignKey(d => d.HospitalId)
                .HasConstraintName("FK__users__hospital___3D5E1FD2");
        });

        modelBuilder.Entity<WorkSchedule>(entity =>
        {
            entity.HasKey(e => new { e.WDay, e.DoctorId, e.Starthour }).HasName("PK__work_sch__7E880220B7E29E5D");

            entity.ToTable("work_schedules");

            entity.Property(e => e.WDay)
                .HasMaxLength(30)
                .HasColumnName("w_day");
            entity.Property(e => e.DoctorId)
                .HasMaxLength(9)
                .HasColumnName("doctor_id");
            entity.Property(e => e.Starthour)
                .HasColumnType("datetime")
                .HasColumnName("starthour");
            entity.Property(e => e.Endhour)
                .HasColumnType("datetime")
                .HasColumnName("endhour");

            entity.HasOne(d => d.Doctor).WithMany(p => p.WorkSchedules)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__work_sche__docto__46E78A0C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

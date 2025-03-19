using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoBasesDatos.Models;
using System.Data;

namespace ProyectoBasesDatos.Controllers
{
    public class AdminsController : Controller
    {
        private readonly dbContext _context;

        public AdminsController(dbContext context)
        {
            _context = context;
        }
        public IActionResult AdvancedQueriesHome()
        {
            return View();
        }

        public IActionResult PatientMeds()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetPatients(string doctorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(doctorId) || startDate == null || endDate == null)
                {
                    return Json(new { success = false, message = "Todos los campos son requeridos." });
                }

                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("GetPatientsByDoctor", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@doctorId", doctorId);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var patients = new List<dynamic>();
                            while (await reader.ReadAsync())
                            {
                                var patient = new
                                {
                                    PatientId = reader["PatientId"],
                                    PatientAddress = reader["PatientAddress"],
                                    PatientGender = reader["PatientGender"],
                                    PatientBirthdate = reader["PatientBirthdate"],
                                    PatientName = reader["PatientName"],
                                    PatientFirstLastName = reader["PatientFirstLastName"],
                                    PatientSecondLastName = reader["PatientSecondLastName"],
                                    AppointmentDay = reader["AppointmentDay"],
                                    AppointmentHour = reader["AppointmentHour"]
                                };

                                patients.Add(patient);
                                
                            }

                            // Imprimir la lista de pacientes
                            foreach (var patient in patients)
                            {
                                Console.WriteLine($"PatientId: {patient.PatientId}");
                                Console.WriteLine($"PatientAddress: {patient.PatientAddress}");
                                Console.WriteLine($"PatientGender: {patient.PatientGender}");
                                Console.WriteLine($"PatientBirthdate: {patient.PatientBirthdate}");
                                Console.WriteLine($"PatientName: {patient.PatientName}");
                                Console.WriteLine($"PatientFirstLastName: {patient.PatientFirstLastName}");
                                Console.WriteLine($"PatientSecondLastName: {patient.PatientSecondLastName}");
                                Console.WriteLine($"AppointmentDay: {patient.AppointmentDay}");
                                Console.WriteLine($"AppointmentHour: {patient.AppointmentHour}");
                                Console.WriteLine(); // Separador entre pacientes
                            }
                                
                            return Json(new { success = true, patients });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Ocurrió un error al procesar la solicitud." });
            }
        }


        public IActionResult PendingPayment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> pendingPayments(string patientId)
        {
            try
            {
                if (string.IsNullOrEmpty(patientId))
                {
                    return Json(new { success = false, message = "La cédula del paciente es requerida." });
                }

                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("GetPendingPaymentHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PatientId", patientId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var pendingPayments = new List<dynamic>();
                            while (await reader.ReadAsync())
                            {
                                Console.WriteLine("Dentro de while");
                                var payment = new
                                {
                                    pendingPayment = reader["PendingPayment"],
                                    appointmentId = reader["AppointmentId"],
                                    appointmentDate = reader["AppointmentDay"],
                                    appointmentHour = reader["AppointmentHour"],
                                    specialtyName = reader["DoctorSpecialty"]
                                };
                                pendingPayments.Add(payment);
                            }
                            return Json(new { success = true, pendingPayments });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine("Error SQL: " + sqlEx.Message);
                return Json(new { success = false, message = "Ocurrió un error en la base de datos al procesar la solicitud." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Ocurrió un error al procesar la solicitud." });
            }
        }

        public IActionResult StockPrescription()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StockPrescriptionPost()
        {
            var idHospital = HttpContext.Session.GetString("IdHospital");
            Console.WriteLine("ID HOSPITAL " + idHospital);
            try
            {
                if (string.IsNullOrEmpty(idHospital))
                {
                    return Json(new { success = false, message = "El ID del hospital es requerido." });
                }

                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("GetInventoryAndPrescriptions", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@HospitalId", idHospital);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var medStock = new List<dynamic>();
                            var prescriptions = new List<dynamic>();
                            while (await reader.ReadAsync())
                            {
                                var item = new
                                {
                                    idMed = reader["MedicationId"],
                                    medName = reader["MedicationName"],
                                    medDecription = reader["MedicationDescription"],
                                    medPrice = reader["MedicationPrice"],
                                    availableStock = reader["MedicationStock"]
                                };
                                medStock.Add(item);
                            }
                            await reader.NextResultAsync();
                            while (await reader.ReadAsync())
                            {
                                var item = new
                                {
                                    idPrescription = reader["PrescriptionId"],
                                    medName = reader["MedicationName"],
                                    dosis = reader["Dosis"],
                                    frequency = reader["Frequency"],
                                    preDate = reader["PrescriptionDate"],
                                    treatmentPrice = reader["TreatmentPrice"],
                                    patientName = reader["PatientName"],
                                    firstLastName = reader["PatientFirstLastName"],
                                    secondLastName = reader["PatientSecondLastName"]
                                };
                                prescriptions.Add(item);
                            }
                            return Json(new { success = true, medStock, prescriptions });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "Ocurrió un error al procesar la solicitud." });
            }
        }


        public IActionResult TotalPayment()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> TotalPayment(string patientId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(patientId))
                {
                    return Json(new { success = false, message = "La cédula del paciente es requerida." });
                }
                if (startDate > endDate)
                {
                    return Json(new { success = false, message = "La fecha de inicio no puede ser mayor que la fecha de fin." });
                }

                var connectionString = _context.Database.GetDbConnection().ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("CalculateTotalPaymentsByPatient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@patientId", patientId);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                var payments = new
                                {
                                    patientId = reader["PatientId"],
                                    totalPayments = reader["TotalPayments"],
                                    paymentAmount = reader["NumberOfPayments"]
                                };

                                Console.WriteLine("Patient id: " + payments.patientId); ;

                                return Json(new { success = true, payments });
                            }
                            else
                            {
                                return Json(new { success = false, message = "No se encontraron pagos para el paciente en el rango de fechas especificado." });
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                return Json(new { success = false, message = sqlEx.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al procesar la solicitud." });
            }
        }
    }

}


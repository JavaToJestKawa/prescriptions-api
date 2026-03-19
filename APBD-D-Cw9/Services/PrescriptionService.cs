using APBD_D_Cw9.Data;
using APBD_D_Cw9.DTOs;
using APBD_D_Cw9.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_D_Cw9.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly PrescriptionContext _context;

    public PrescriptionService(PrescriptionContext context)
    {
        _context = context;
    }

    public async Task AddPrescriptionAsync(AddPrescriptionRequest request)
    {
        if (request.Medicaments.Count > 10)
            throw new InvalidOperationException("Prescription cannot contain more than 10 medicaments.");

        if (request.DueDate < request.Date)
            throw new InvalidOperationException("DueDate must be later or equal to Date.");

        var doctor = await _context.Doctors.FindAsync(request.IdDoctor)
            ?? throw new InvalidOperationException("Doctor does not exist.");

        var patient = await _context.Patients.FindAsync(request.Patient.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                IdPatient = request.Patient.IdPatient,
                FirstName = request.Patient.FirstName,
                LastName = request.Patient.LastName,
                Birthdate = request.Patient.Birthdate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            IdDoctor = doctor.IdDoctor,
            IdPatient = patient.IdPatient
        };

        foreach (var m in request.Medicaments)
        {
            var medicament = await _context.Medicaments.FindAsync(m.IdMedicament);
            if (medicament == null)
                throw new InvalidOperationException($"Medicament with ID {m.IdMedicament} not found.");

            prescription.PrescriptionMedicaments.Add(new PrescriptionMedicament
            {
                IdMedicament = medicament.IdMedicament,
                Dose = m.Dose,
                Details = m.Description
            });
        }

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
    }

    public async Task<GetPatientDetailsResponse?> GetPatientDetailsAsync(int idPatient)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
                .ThenInclude(p => p.Doctor)
            .Include(p => p.Prescriptions)
                .ThenInclude(p => p.PrescriptionMedicaments)
                    .ThenInclude(pm => pm.Medicament)
            .FirstOrDefaultAsync(p => p.IdPatient == idPatient);

        if (patient == null)
            return null;

        return new GetPatientDetailsResponse
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            Birthdate = patient.Birthdate,
            Prescriptions = patient.Prescriptions
                .OrderBy(p => p.DueDate)
                .Select(p => new GetPatientDetailsResponse.PrescriptionDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new GetPatientDetailsResponse.DoctorDto
                    {
                        IdDoctor = p.Doctor.IdDoctor,
                        FirstName = p.Doctor.FirstName
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new GetPatientDetailsResponse.MedicamentDto
                    {
                        IdMedicament = pm.Medicament.IdMedicament,
                        Name = pm.Medicament.Name,
                        Description = pm.Medicament.Description,
                        Dose = pm.Dose
                    }).ToList()
                }).ToList()
        };
    }
}

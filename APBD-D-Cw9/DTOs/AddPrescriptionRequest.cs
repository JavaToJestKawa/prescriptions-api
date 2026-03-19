namespace APBD_D_Cw9.DTOs;

public class AddPrescriptionRequest
{
    public PatientDto Patient { get; set; } = null!;
    public List<MedicamentDto> Medicaments { get; set; } = new();
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdDoctor { get; set; }

    public class PatientDto
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Birthdate { get; set; }
    }

    public class MedicamentDto
    {
        public int IdMedicament { get; set; }
        public int? Dose { get; set; }
        public string Description { get; set; } = null!;
    }
}

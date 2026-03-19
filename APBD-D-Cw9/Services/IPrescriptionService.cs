using APBD_D_Cw9.DTOs;

namespace APBD_D_Cw9.Services;

public interface IPrescriptionService
{
    Task AddPrescriptionAsync(AddPrescriptionRequest request);
    Task<GetPatientDetailsResponse?> GetPatientDetailsAsync(int idPatient);
}

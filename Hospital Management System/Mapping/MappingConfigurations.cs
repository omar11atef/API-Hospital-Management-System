
namespace Hospital_Management_System.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {

        TypeAdapterConfig<Doctor, ResponseDoctorWithAppointments>.NewConfig()
            .Map(dest => dest.DoctorId, src => src.Id)
            .Map(dest => dest.DoctorName, src => src.Name)
            .Map(dest => dest.DepartmentName, src => src.Department.Name); 

        TypeAdapterConfig<Appointment, DoctorAppointment>.NewConfig()
            .Map(dest => dest.AppointmentId, src => src.Id)
            .Map(dest => dest.PatientName, src => src.Patient.Name)
            .Map(dest => dest.AppointmentDate, src => src.AppointmentDate.ToString("yyyy-MM-dd"));

        // Appointment → AppointmentResponse (used in GetById, Create, Update)
        config.NewConfig<Appointment, AppointmentResponse>()
            .Map(dest => dest.DoctorName, src => src.Doctor.Name)
            .Map(dest => dest.PatientName, src => src.Patient.Name);

        // Appointment → PatientAppointment (nested inside PatientAppointmentsResponse)
        // Doctor is loaded via ThenInclude — Patient is the parent, not needed here
        config.NewConfig<Appointment, PatientAppointment>()
            .Map(dest => dest.AppointmentId, src => src.Id)
            .Map(dest => dest.DoctorName, src => src.Doctor.Name);

        // Patient → PatientAppointmentsResponse
        // Department and Appointments (with Doctor) must be Included in the query
        config.NewConfig<Patient, PatientAppointmentsResponse>()
            .Map(dest => dest.PatientId, src => src.Id)
            .Map(dest => dest.PatientName, src => src.Name)
            .Map(dest => dest.DepartmentName, src => src.Department.Name)
            .Map(dest => dest.Appointments, src => src.Appointments.Adapt<IEnumerable<PatientAppointment>>());

        // Doctor → DoctorAppointmentsResponse (keep existing)
        config.NewConfig<Doctor, DoctorAppointmentsResponse>()
            .Map(dest => dest.DoctorId, src => src.Id)
            .Map(dest => dest.DoctorName, src => src.Name)
            .Map(dest => dest.Specialization, src => src.Specialization)
            .Map(dest => dest.Appointments, src => src.Appointments.Adapt<IEnumerable<AppointmentResponse>>());

        TypeAdapterConfig<Appointment, AppointmentResponse>.NewConfig()
            .Map(dest => dest.AppointmentDate,
                 src => src.AppointmentDate.ToString("yyyy-MM-dd HH:mm"))
            .Map(dest => dest.SlotDisplay,
                 src => $"{src.AppointmentDate.Hour:D2}:00 – {src.AppointmentDate.Hour + 1:D2}:00");


    }
}


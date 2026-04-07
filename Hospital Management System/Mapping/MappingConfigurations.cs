
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
                 src => BuildSlotDisplay(src.AppointmentDate));

        //--- After use ProjectToType to mapping in Room Service :
        config.NewConfig<Room, RoomAppointmentsResponse>()
            .Map(dest => dest.RoomId, src => src.Id)
            .Map(dest => dest.DepartmentName, src => src.Department != null ? src.Department.Name : "Unknown")
            .Map(dest => dest.Appointments, src => src.PatientRooms
                .Where(pr => !pr.IsDeleted && pr.Appointment != null && !pr.Appointment.IsDeleted)
                .Select(pr => pr.Appointment)
                .OrderBy(a => a.AppointmentDate));

        config.NewConfig<Appointment, RoomAppointmentItem>()
            .Map(dest => dest.DoctorName, src => src.Doctor != null ? src.Doctor.Name : "Unknown")
            .Map(dest => dest.PatientName, src => src.Patient != null ? src.Patient.Name : "Unknown")
            .Map(dest => dest.AppointmentDate, src => src.AppointmentDate);

        //User :
        config.NewConfig<(ApplicationUser user, IList<string> useRroles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.useRroles);
            

        // Admin :
       

    }
    private static string BuildSlotDisplay(DateTime date)
    {
        return $"{date:HH\\:mm} – {date.AddHours(1):HH\\:mm}";
    }
}


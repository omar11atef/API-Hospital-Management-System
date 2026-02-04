namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController(IDoctorService doctorServices) : ControllerBase
{
    
    private readonly IDoctorService _doctorServices= doctorServices;

    // GET All Doctors
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllDoctors(CancellationToken cancellationToken = default)
    {
        var doctors = await _doctorServices.GetAllDoctorsAsync(cancellationToken);
        var respone = doctors.Adapt<IEnumerable<Doctor>>();
        return Ok(respone);
    }
    // GET All Doctors Exsits
    [HttpGet("Doctors Exsits")]
    public async Task<IActionResult> GetAllDoctorsExsits(CancellationToken cancellationToken = default)
    {
        var doctors = await _doctorServices.GetAllDoctorsExsitsAsync(cancellationToken);
        var respone = doctors.Adapt<IEnumerable<Doctor>>();
        return Ok(respone);
    }


    // GET Doctor by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDoctorById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        
        var doctor = await _doctorServices.GetDoctorByIdAsync(id, cancellationToken);
        if (doctor == null)
            return NotFound();
        
        return Ok(doctor);
        
    }
    
    // POST Create a new Doctor
    [HttpPost("Create New Doctor")]
    public async Task <IActionResult> CreateDoctor([FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        if (_doctorServices is null)
            return NotFound();

        var createdDoctor = await _doctorServices.CreateDoctorAsync(doctor.Adapt<Doctor>(),cancellationToken); ;
        if (createdDoctor == null)
            return BadRequest();

        return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.Id }, createdDoctor);
       
    }
    
    //Put Update Doctor
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDoctor([FromRoute] int id, [FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        if (_doctorServices == null)
            return NotFound();
        var updatedDoctor = await _doctorServices.UpdateDoctorAsync(id, doctor.Adapt<Doctor>(),cancellationToken);
        if (updatedDoctor == null)
            return NotFound();
        return NoContent();
    }
    
    // DELETE Doctor
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctor([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        if (_doctorServices == null)
            return NotFound();

        var result = await _doctorServices.DeleteDoctorAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    // Make Doctor Is Existed
    [HttpPut("{id}/ToggleExisted")]
    public async Task<IActionResult> IsDoctorExists([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        if (_doctorServices == null)
            return NotFound();

        var result = await _doctorServices.IsDoctorExistsAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

}

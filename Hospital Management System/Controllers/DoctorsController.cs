namespace Hospital_Management_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorsController(IDoctorServices doctorServices) : ControllerBase
{
    
    private readonly IDoctorServices _doctorServices= doctorServices;

    // GET All Doctors
    [HttpGet]
    public IActionResult GetAllDoctors(CancellationToken cancellationToken = default)
    {
        var doctors = _doctorServices?.GetAllDoctors(cancellationToken);
        return Ok(doctors);
    }

    // GET Doctor by ID
    [HttpGet("{id:int}")]
    public IActionResult GetDoctorById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        
        var doctor = _doctorServices.GetDoctorById(id, cancellationToken);
        if (doctor == null)
            return NotFound();

        return Ok(doctor);
        
    }

    // POST Create a new Doctor
    [HttpPost("Create New Doctor")]
    public IActionResult CreateDoctor([FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        
        var createdDoctor = _doctorServices?.CreateDoctor(doctor.Adapt<Doctor>(),cancellationToken); ;
        if (createdDoctor == null)
            return BadRequest();

        return CreatedAtAction(nameof(GetDoctorById), new { id = createdDoctor.Id }, createdDoctor);
       
    }

    //Put Update Doctor
    [HttpPut("{id:int}")]
    public IActionResult UpdateDoctor([FromRoute] int id, [FromBody] RequestDoctor doctor, CancellationToken cancellationToken = default)
    {
        if (_doctorServices == null)
            return NotFound();
        var updatedDoctor = _doctorServices.UpdateDoctor(id, doctor.Adapt<Doctor>(),cancellationToken);
        if (updatedDoctor == null)
            return NotFound();
        return NoContent();
    }

    // DELETE Doctor
    [HttpDelete("{id}")]
    public IActionResult DeleteDoctor([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        if (_doctorServices == null)
            return NotFound();

        var result = _doctorServices.DeleteDoctor(id, cancellationToken);
        if (!result) return NotFound();
        
        return NoContent();
    }

    

}

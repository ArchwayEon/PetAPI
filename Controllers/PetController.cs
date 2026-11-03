using Microsoft.AspNetCore.Mvc;
using PetAPI.Models.Entities;
using PetAPI.Services;

namespace PetAPI.Controllers;

// A controller is a class that handles incoming HTTP requests.
// The [ApiController] attribute tells ASP.NET Core that this class is a Web API:
//   - it automatically returns 400 Bad Request if the request data is invalid, and
//   - it reads complex parameters from the request body or form for us.
// The [Route] attribute sets the base URL for every action in this class,
// so "api/pet" is the start of every endpoint below.
[ApiController]
[Route("api/pet")]
public class PetController : ControllerBase
{
    // The controller does not talk to the database directly. It asks an
    // IPetRepository to do the work. This separation is called the
    // "repository pattern": the controller handles HTTP, and the repository
    // handles data storage.
    private readonly IPetRepository _repository;

    // Dependency injection: ASP.NET Core sees that this constructor needs an
    // IPetRepository and automatically provides one (see Program.cs).
    // We never call "new DbPetRepository()" ourselves.
    public PetController(IPetRepository repository)
    {
        _repository = repository;
    }

    // ---------------------------------------------------------------
    // GET /api/pet/all
    // Returns every pet in the database.
    //   Request body: none
    //   Response:     200 OK with a JSON array of pets, e.g. [{"id":1,...}, ...]
    // ---------------------------------------------------------------
    [HttpGet("all")]
    public async Task<ActionResult<ICollection<Pet>>> GetAll()
    {
        // "await" pauses this method while the database works, so the server
        // can handle other requests in the meantime.
        var pets = await _repository.ReadAllAsync();

        // Ok(...) builds a 200 OK response. The pets are converted to JSON automatically.
        return Ok(pets);
    }

    // ---------------------------------------------------------------
    // GET /api/pet/one/{id}
    // Returns a single pet. The {id} part of the URL is read into the "id" parameter.
    //   Example: GET /api/pet/one/2
    //   Request body: none
    //   Response:     200 OK with one pet object, or 404 Not Found if no pet has that id
    // ---------------------------------------------------------------
    // The ":int" constraint means "only match when {id} is a whole number".
    // "Name" gives this route a name, so other actions can link to it (see Create).
    [HttpGet("one/{id:int}", Name = "GetPetById")]
    public async Task<ActionResult<Pet>> GetOne(int id)
    {
        var pet = await _repository.ReadAsync(id);

        // A null result means the pet does not exist, so we return 404 Not Found.
        return pet is null ? NotFound() : Ok(pet);
    }

    // ---------------------------------------------------------------
    // POST /api/pet/create
    // Adds a new pet. POST is the usual verb for "create something new".
    //   Request body: form-data fields "Name" and "Weight" (Id is ignored)
    //   Response:     201 Created with the new pet, including its generated Id.
    //                 The Location header gives the URL of the new pet.
    // ---------------------------------------------------------------
    [HttpPost("create")]
    public async Task<ActionResult<Pet>> Create([FromForm] Pet newPet)
    {
        // [FromForm] tells ASP.NET Core to read the pet's fields from the
        // submitted form data rather than from the URL or a JSON body.

        // The database creates the Id for us, so we clear any Id the client sent.
        newPet.Id = 0;

        var created = await _repository.CreateAsync(newPet);

        // 201 Created is the correct status for a successful POST. We also add a
        // Location header that points to the new pet, using the "GetPetById" route name.
        return CreatedAtRoute("GetPetById", new { id = created.Id }, created);
    }

    // ---------------------------------------------------------------
    // PUT /api/pet/update
    // Replaces the Name and Weight of an existing pet. PUT is the usual verb for "update".
    //   Request body: form-data fields "Id", "Name" and "Weight".
    //                 The Id says which pet to change.
    //   Response:     204 No Content on success (no body), or 404 Not Found if the Id does not exist.
    // ---------------------------------------------------------------
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] Pet updatedPet)
    {
        // The pet to change is identified by the Id sent in the form data.
        var updated = await _repository.UpdateAsync(updatedPet.Id, updatedPet);

        // 204 No Content means "it worked, and there is nothing to send back."
        return updated ? NoContent() : NotFound();
    }

    // ---------------------------------------------------------------
    // DELETE /api/pet/delete/{id}
    // Removes a pet. DELETE is the usual verb for "remove something".
    //   Example: DELETE /api/pet/delete/3
    //   Request body: none
    //   Response:     204 No Content on success (no body), or 404 Not Found if the Id does not exist.
    // ---------------------------------------------------------------
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

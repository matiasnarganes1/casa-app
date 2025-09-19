using CasaApp.Api.DTOs;
using CasaApp.Api.Models;
using CasaApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CasaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _service;

    public MenuController(IMenuService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MenuDto), StatusCodes.Status201Created)]
    [SwaggerOperation(Summary = "Crear un nuevo menú")]
    public async Task<ActionResult<MenuDto>> Create([FromBody] CreateMenuDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var created = await _service.CreateMenuAsync(dto);
        return created is null
            ? Problem(statusCode: StatusCodes.Status500InternalServerError, title: "No se pudo crear el menú.")
            : CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MenuDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Obtener un menú por ID")]
    public async Task<ActionResult<MenuDto>> GetById(int id)
    {
        var menu = await _service.GetMenuByIdAsync(id);
        return menu is null ? NotFound() : Ok(menu);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MenuDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Listar menús paginados")]
    public async Task<ActionResult<PagedResult<MenuDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var result = await _service.GetMenusPagedAsync(page, pageSize);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Eliminar un menú por ID")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteMenuAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("platos")]
    [ProducesResponseType(typeof(PagedResult<PlatoDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtiene platos paginados")]
    public async Task<ActionResult<PagedResult<PlatoDto>>> GetPlatos([FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var result = await _service.GetPlatosPagedAsync(page, pageSize);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpPost("platos")]
    [ProducesResponseType(typeof(PlatoDto), StatusCodes.Status201Created)]
    [SwaggerOperation(Summary = "Crea un nuevo plato")]
    public async Task<ActionResult<PlatoDto>> CreatePlato([FromBody] PlatoDto plato)
    {
        if (string.IsNullOrWhiteSpace(plato.Nombre)) return ValidationProblem("El nombre del plato es obligatorio.");
        var created = await _service.CreatePlatoAsync(plato);
        return CreatedAtAction(nameof(GetPlatoById), new { id = created.Id }, created);
    }

    [HttpGet("platos/{id:int}")]
    [ProducesResponseType(typeof(PlatoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Obtiene un plato por ID")]
    public async Task<ActionResult<PlatoDto>> GetPlatoById(int id)
    {
        var plato = await _service.GetPlatoWithIngredientesAsync(id);
        return plato is null ? NotFound() : Ok(plato);
    }

    [HttpPut("platos/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Actualiza un plato")]
    public async Task<IActionResult> UpdatePlato(int id, [FromBody] PlatoDto plato)
    {
        var ok = await _service.UpdatePlatoAsync(id, plato);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("platos/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Elimina un plato")]
    public async Task<IActionResult> DeletePlato(int id)
    {
        var ok = await _service.DeletePlatoAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("ingredientes-all")]
    [ProducesResponseType(typeof(IEnumerable<IngredienteDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtiene todos los ingredientes")]
    public async Task<ActionResult<IEnumerable<IngredienteDto>>> GetIngredientes()
    {
        var ingredientes = await _service.GetAllIngredientesAsync();
        return Ok(ingredientes);
    }

    [HttpGet("ingredientes")]
    [ProducesResponseType(typeof(PagedResult<IngredienteDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtiene ingredientes paginados")]
    public async Task<ActionResult<PagedResult<IngredienteDto>>> GetIngredientes([FromQuery] int page = 1, [FromQuery] int pageSize = 12)
    {
        var result = await _service.GetIngredientesPagedAsync(page, pageSize);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpPost("ingredientes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [SwaggerOperation(Summary = "Crea un nuevo ingrediente")]
    public async Task<IActionResult> CreateIngrediente([FromBody] CreateIngredienteDto ingrediente)
    {
        if (string.IsNullOrWhiteSpace(ingrediente.Nombre)) return ValidationProblem("El nombre del ingrediente es obligatorio.");
        var ok = await _service.CreateIngredienteAsync(ingrediente);
        if (!ok) return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "No se pudo crear el ingrediente.");
        var created = await _service.GetIngredienteByNameAsync(ingrediente.Nombre);
        return created is null
            ? Problem(statusCode: StatusCodes.Status500InternalServerError, title: "No se pudo obtener el ingrediente creado.")
            : CreatedAtAction(nameof(GetIngredienteById), new { id = created.Id }, created);
    }

    [HttpGet("ingredientes/{id:int}")]
    [ProducesResponseType(typeof(IngredienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Obtiene un ingrediente por ID")]
    public async Task<ActionResult<IngredienteDto>> GetIngredienteById(int id)
    {
        var ingrediente = await _service.GetIngredienteAsync(id);
        return ingrediente is null ? NotFound() : Ok(ingrediente);
    }

    [HttpPut("ingredientes/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Actualiza un ingrediente")]
    public async Task<IActionResult> UpdateIngrediente(int id, [FromBody] CreateIngredienteDto ingrediente)
    {
        var ok = await _service.UpdateIngredienteAsync(id, ingrediente);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("ingredientes/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Elimina un ingrediente")]
    public async Task<IActionResult> DeleteIngrediente(int id)
    {
        var ok = await _service.DeleteIngredienteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("ingredientes/{platoId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Agrega ingredientes a un plato")]
    public async Task<IActionResult> AddIngredienteToPlato(int platoId, [FromBody] List<CreateIngredienteEnPlatoDto> ingredientes)
    {
        if (ingredientes is null || ingredientes.Count == 0) return ValidationProblem("Se requiere al menos un ingrediente.");
        var ok = await _service.AddIngredienteToPlatoAsync(platoId, ingredientes);
        return ok ? NoContent() : BadRequest();
    }

    [HttpDelete("{platoId:int}/ingredientes/{ingredienteId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Elimina un ingrediente de un plato")]
    public async Task<IActionResult> RemoveIngrediente(int platoId, int ingredienteId)
    {
        var ok = await _service.DeleteIngredienteFromPlatoAsync(platoId, ingredienteId);
        return ok ? NoContent() : NotFound();
    }
}
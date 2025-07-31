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
    #region Platos

    [HttpGet("platos")]
    [SwaggerOperation(Summary = "Obtiene todos los platos", Description = "Devuelve la lista completa de platos")]
    public async Task<IActionResult> GetPlatos()
    {
        var platos = await _service.GetAllPlatosAsync();
        return Ok(platos);
    }

    [HttpPost("platos")]
    [SwaggerOperation(Summary = "Crea un nuevo plato", Description = "Agrega un nuevo plato al menú")]
    public async Task<IActionResult> CreatePlato([FromBody] CreatePlatoDto plato)
    {
        var created = await _service.CreatePlatoAsync(plato);
        return CreatedAtAction(nameof(GetPlatoById), new { id = created.Id }, created);
    }

    [HttpGet("platos/{id}")]
    [SwaggerOperation(Summary = "Obtiene un plato por ID", Description = "Devuelve los detalles de un plato específico")]
    public async Task<IActionResult> GetPlatoById(int id)
    {
        var plato = await _service.GetPlatoWithIngredientesAsync(id);
        return plato == null ? NotFound() : Ok(plato);
    }

    [HttpPut("platos/{id}")]
    [SwaggerOperation(Summary = "Actualiza un plato", Description = "Modifica los detalles de un plato existente")]
    public async Task<IActionResult> UpdatePlato(int id, [FromBody] Plato plato)
    {
        var updated = await _service.UpdatePlatoAsync(id, plato);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("platos/{id}")]
    [SwaggerOperation(Summary = "Elimina un plato", Description = "Borra un plato del menú")]
    public async Task<IActionResult> DeletePlato(int id)
    {
        var deleted = await _service.DeletePlatoAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    #endregion

    #region Ingredientes
    [HttpGet("ingredientes")]
    [SwaggerOperation(Summary = "Obtiene todos los ingredientes", Description = "Devuelve la lista completa de ingredientes")]
    public async Task<IActionResult> GetIngredientes()
    {
        var ingredientes = await _service.GetAllIngredientesAsync();
        return Ok(ingredientes);
    }

    [HttpPost("ingredientes")]
    [SwaggerOperation(Summary = "Crea un nuevo ingrediente", Description = "Agrega un nuevo ingrediente al menú")]
    public async Task<IActionResult> CreateIngrediente([FromBody] CreateIngredienteDto ingrediente)
    {
        var created = await _service.CreateIngredienteAsync(ingrediente);
        return CreatedAtAction(nameof(GetIngredientes), new { id = created.Id }, created);
    }

    [HttpGet("ingredientes/{id}")]
    [SwaggerOperation(Summary = "Obtiene un ingrediente por ID", Description = "Devuelve los detalles de un ingrediente específico")]
    public async Task<IActionResult> GetIngredienteById(int id)
    {
        var ingrediente = await _service.GetIngredienteAsync(id);
        return ingrediente == null ? NotFound() : Ok(ingrediente);
    }

    [HttpPut("ingredientes/{id}")]
    [SwaggerOperation(Summary = "Actualiza un ingrediente", Description = "Modifica los detalles de un ingrediente existente")]
    public async Task<IActionResult> UpdateIngrediente(int id, [FromBody] Ingrediente ingrediente)
    {
        var updated = await _service.UpdateIngredienteAsync(id, ingrediente);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("ingredientes/{id}")]
    [SwaggerOperation(Summary = "Elimina un ingrediente", Description = "Borra un ingrediente del menú")]
    public async Task<IActionResult> DeleteIngrediente(int id)
    {
        var deleted = await _service.DeleteIngredienteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("platos/{platoId}/ingredientes/{ingredienteId}")]
    [SwaggerOperation(Summary = "Agrega un ingrediente a un plato", Description = "Asocia un ingrediente a un plato específico")]
    public async Task<IActionResult> AddIngredienteToPlato(int platoId, CreateIngredienteEnPlatoDto ingrediente)
    {
        var added = await _service.AddIngredienteToPlatoAsync(platoId, ingrediente);
        return added ? NoContent() : NotFound();
    }
    #endregion
}
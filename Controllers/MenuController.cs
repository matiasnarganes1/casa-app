using CasaApp.Api.DTOs;
using CasaApp.Api.Models;
using CasaApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetPlatos()
    {
        var platos = await _service.GetAllPlatosAsync();
        return Ok(platos);
    }

    [HttpPost("platos")]
    public async Task<IActionResult> CreatePlato([FromBody] CreatePlatoDto plato)
    {
        var created = await _service.CreatePlatoAsync(plato);
        return CreatedAtAction(nameof(GetPlatoById), new { id = created.Id }, created);
    }

    [HttpGet("platos/{id}")]
    public async Task<IActionResult> GetPlatoById(int id)
    {
        var plato = await _service.GetPlatoWithIngredientesAsync(id);
        return plato == null ? NotFound() : Ok(plato);
    }

    [HttpPut("platos/{id}")]
    public async Task<IActionResult> UpdatePlato(int id, [FromBody] Plato plato)
    {
        var updated = await _service.UpdatePlatoAsync(id, plato);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("platos/{id}")]
    public async Task<IActionResult> DeletePlato(int id)
    {
        var deleted = await _service.DeletePlatoAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    #endregion

    #region Ingredientes
    [HttpGet("ingredientes")]
    public async Task<IActionResult> GetIngredientes()
    {
        var ingredientes = await _service.GetAllIngredientesAsync();
        return Ok(ingredientes);
    }

    [HttpPost("ingredientes")]
    public async Task<IActionResult> CreateIngrediente([FromBody] CreateIngredienteDto ingrediente)
    {
        var created = await _service.CreateIngredienteAsync(ingrediente);
        return CreatedAtAction(nameof(GetIngredientes), new { id = created.Id }, created);
    }

    [HttpGet("ingredientes/{id}")]
    public async Task<IActionResult> GetIngredienteById(int id)
    {
        var ingrediente = await _service.GetIngredienteAsync(id);
        return ingrediente == null ? NotFound() : Ok(ingrediente);
    }

    [HttpPut("ingredientes/{id}")]
    public async Task<IActionResult> UpdateIngrediente(int id, [FromBody] Ingrediente ingrediente)
    {
        var updated = await _service.UpdateIngredienteAsync(id, ingrediente);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("ingredientes/{id}")]
    public async Task<IActionResult> DeleteIngrediente(int id)
    {
        var deleted = await _service.DeleteIngredienteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
    #endregion
}
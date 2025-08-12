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
    #region Menus
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Crear un nuevo menú")]
    public async Task<IActionResult> Create([FromBody] CreateMenuDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _service.CreateMenuAsync(dto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MenuDto), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtener un menú por ID")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var menu = await _service.GetMenuByIdAsync(id);
            return menu == null ? NotFound() : Ok(menu);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Listar todos los menús")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var menus = await _service.GetAllMenusAsync();
            return menus == null ? NotFound() : Ok(menus);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Eliminar un menú por ID")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteMenuAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    #endregion
    #region Platos

    [HttpGet("platos")]
    [ProducesResponseType(typeof(IEnumerable<PlatoDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtiene todos los platos", Description = "Devuelve la lista completa de platos")]
    public async Task<IActionResult> GetPlatos()
    {
        try
        {
            var platos = await _service.GetAllPlatosAsync();
            return platos == null ? NotFound() : Ok(platos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("platos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Crea un nuevo plato", Description = "Agrega un nuevo plato al menú")]
    public async Task<IActionResult> CreatePlato([FromBody] PlatoDto plato)
    {
        try
        {
            var created = await _service.CreatePlatoAsync(plato);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("platos/{id}")]
    [ProducesResponseType(typeof(PlatoDto), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtiene un plato por ID", Description = "Devuelve los detalles de un plato específico")]
    public async Task<IActionResult> GetPlatoById(int id)
    {
        try
        {
            var plato = await _service.GetPlatoWithIngredientesAsync(id);
            return plato == null ? NotFound() : Ok(plato);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("platos/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Actualiza un plato", Description = "Modifica los detalles de un plato existente")]
    public async Task<IActionResult> UpdatePlato(int id, PlatoDto plato)
    {
        try
        {
            await _service.UpdatePlatoAsync(id, plato);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("platos/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Elimina un plato", Description = "Borra un plato del menú")]
    public async Task<IActionResult> DeletePlato(int id)
    {
        try
        {
            await _service.DeletePlatoAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region Ingredientes
    [HttpGet("ingredientes")]
    [ProducesResponseType(typeof(IEnumerable<IngredienteDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtiene todos los ingredientes", Description = "Devuelve la lista completa de ingredientes")]
    public async Task<IActionResult> GetIngredientes()
    {
        try
        {
            var ingredientes = await _service.GetAllIngredientesAsync();
            return ingredientes == null ? NotFound() : Ok(ingredientes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("ingredientes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Crea un nuevo ingrediente", Description = "Agrega un nuevo ingrediente al menú")]
    public async Task<IActionResult> CreateIngrediente([FromBody] CreateIngredienteDto ingrediente)
    {
        try
        {
            await _service.CreateIngredienteAsync(ingrediente);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("ingredientes/{id}")]
    [ProducesResponseType(typeof(IngredienteDto), StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Obtiene un ingrediente por ID", Description = "Devuelve los detalles de un ingrediente específico")]
    public async Task<IActionResult> GetIngredienteById(int id)
    {
        try
        {
            var ingrediente = await _service.GetIngredienteAsync(id);
            return ingrediente == null ? NotFound() : Ok(ingrediente);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("ingredientes/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Actualiza un ingrediente", Description = "Modifica los detalles de un ingrediente existente")]
    public async Task<IActionResult> UpdateIngrediente(int id, [FromBody] CreateIngredienteDto ingrediente)
    {
        try
        {
            await _service.UpdateIngredienteAsync(id, ingrediente);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("ingredientes/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Elimina un ingrediente", Description = "Borra un ingrediente del menú")]
    public async Task<IActionResult> DeleteIngrediente(int id)
    {
        try
        {
            await _service.DeleteIngredienteAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("ingredientes/{platoId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Agrega un ingrediente a un plato", Description = "Asocia un ingrediente a un plato específico")]
    public async Task<IActionResult> AddIngredienteToPlato(int platoId, List<CreateIngredienteEnPlatoDto> ingrediente)
    {
        try
        {
            await _service.AddIngredienteToPlatoAsync(platoId, ingrediente);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{platoId}/ingredientes/{ingredienteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(Summary = "Elimina un ingrediente a un plato", Description = "Elmina un ingrediente de un plato específico")]
    public async Task<IActionResult> RemoveIngrediente(int platoId, int ingredienteId)
    {
        try
        {
            var result = await _service.DeleteIngredienteFromPlatoAsync(platoId, ingredienteId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion
}
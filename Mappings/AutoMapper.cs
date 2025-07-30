using AutoMapper;
using CasaApp.Api.DTOs;
using CasaApp.Api.Models;

namespace CasaApp.Api.Mappings;

public class MenuProfile : Profile
{
    public MenuProfile()
    {
        CreateMap<CreatePlatoDto, Plato>()
            .ForMember(dest => dest.Ingredientes, opt => opt.MapFrom(src =>
                src.Ingredientes.Select(i => new PlatoIngrediente
                {
                    IngredienteId = i.IngredienteId,
                    Cantidad = i.Cantidad,
                    UnidadMedida = i.UnidadMedida
                }).ToList()));

        CreateMap<Plato, CreatePlatoDto>();

        CreateMap<Plato, PlatoDto>()
            .ForMember(dest => dest.Ingredientes, opt => opt.MapFrom(src =>
                src.Ingredientes.Select(pi => new IngredienteEnPlatoDto
                {
                    IngredienteId = pi.IngredienteId,
                    Nombre = pi.Ingrediente.Nombre,
                    Cantidad = $"{pi.Cantidad} {pi.UnidadMedida}"
                }).ToList()));
    }
}
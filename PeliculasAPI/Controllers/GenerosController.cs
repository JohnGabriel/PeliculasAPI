using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTO;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController:ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDBContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            var entidades=await context.Generos.ToListAsync();
            var dto = mapper.Map<List<GeneroDTO>>(entidades);
            return dto;
        }

        [HttpGet]
        [Route("listar")]
        public async Task<ActionResult<List<Genero>>> listarGeneros()
        {
            return await context.Generos.ToListAsync();
        }
        [HttpGet("{id:int}",Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            var entidad = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
            if (entidad == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<GeneroDTO>(entidad);

            return dto;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var entidad = mapper.Map<Genero>(generoCreacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();

            var generoDTO = mapper.Map<Genero>(entidad);

            return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var reg = await context.Generos.AnyAsync(d => d.Id == id);
            if (!reg)
            {
                return NotFound();
            }
            context.Remove(new Genero() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var reg = mapper.Map<Genero>(generoCreacionDTO);
            reg.Id = id;
            context.Entry(reg).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        
        }
    }
}

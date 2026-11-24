using InventarioService.Data;
using InventarioService.DTOs;
using InventarioService.Interfaces;
using InventarioService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly InventarioDbContext _context;

        public CategoriaService(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaDto>> ConsultarTodos()
        {
            return await _context.Categorias
                .AsNoTracking()
                .Select(x => new CategoriaDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    Activo = x.Activo
                })
                .ToListAsync();
        }

        public async Task<CategoriaDto?> ConsultarPorId(int id)
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CategoriaDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    Activo = x.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoriaDto> Crear(CrearCategoriaDto dto)
        {
            var categoria = new Categoria
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                Activo = true
            };

            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();

            return new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activo = categoria.Activo
            };
        }

        public async Task<bool> Actualizar(
            int id,
            ActualizarCategoriaDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return false;

            categoria.Nombre = dto.Nombre.Trim();
            categoria.Descripcion = dto.Descripcion;
            categoria.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                return false;

            // Eliminación lógica
            _context.Categorias.Remove(categoria);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
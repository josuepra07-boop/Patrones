using InventarioService.Data;
using InventarioService.DTOs;
using InventarioService.Interfaces;
using InventarioService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly InventarioDbContext _context;

        public MaterialService(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<List<MaterialDto>> ConsultarTodos()
        {
            return await _context.Materiales
                .AsNoTracking()
                .Include(x => x.Categoria)
                .Select(x => new MaterialDto
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion,
                    CategoriaId = x.CategoriaId,
                    Categoria = x.Categoria != null
                        ? x.Categoria.Nombre
                        : string.Empty,
                    UnidadMedida = x.UnidadMedida,
                    StockMinimo = x.StockMinimo,
                    Activo = x.Activo
                })
                .ToListAsync();
        }

        public async Task<MaterialDto?> ConsultarPorId(int id)
        {
            return await _context.Materiales
                .AsNoTracking()
                .Include(x => x.Categoria)
                .Where(x => x.Id == id)
                .Select(x => new MaterialDto
                {
                    Id = x.Id,
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion,
                    CategoriaId = x.CategoriaId,
                    Categoria = x.Categoria != null
                        ? x.Categoria.Nombre
                        : string.Empty,
                    UnidadMedida = x.UnidadMedida,
                    StockMinimo = x.StockMinimo,
                    Activo = x.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MaterialDto> Crear(CrearMaterialDto dto)
        {
            var categoriaExiste =
                await _context.Categorias.AnyAsync(
                    x => x.Id == dto.CategoriaId && x.Activo);

            if (!categoriaExiste)
                throw new Exception("La categoría no existe o se encuentra inactiva.");

            var material = new Material
            {
                Codigo = dto.Codigo.Trim(),
                Descripcion = dto.Descripcion.Trim(),
                CategoriaId = dto.CategoriaId,
                UnidadMedida = dto.UnidadMedida.Trim(),
                StockMinimo = dto.StockMinimo,
                Activo = true
            };

            _context.Materiales.Add(material);

            await _context.SaveChangesAsync();

            var categoria =
                await _context.Categorias.FindAsync(dto.CategoriaId);

            return new MaterialDto
            {
                Id = material.Id,
                Codigo = material.Codigo,
                Descripcion = material.Descripcion,
                CategoriaId = material.CategoriaId,
                Categoria = categoria?.Nombre ?? string.Empty,
                UnidadMedida = material.UnidadMedida,
                StockMinimo = material.StockMinimo,
                Activo = material.Activo
            };
        }

        public async Task<bool> Actualizar(
            int id,
            ActualizarMaterialDto dto)
        {
            var material =
                await _context.Materiales.FindAsync(id);

            if (material == null)
                return false;

            var categoriaExiste =
                await _context.Categorias.AnyAsync(
                    x => x.Id == dto.CategoriaId && x.Activo);

            if (!categoriaExiste)
                throw new Exception("La categoría no existe.");

            material.Codigo = dto.Codigo.Trim();
            material.Descripcion = dto.Descripcion.Trim();
            material.CategoriaId = dto.CategoriaId;
            material.UnidadMedida = dto.UnidadMedida.Trim();
            material.StockMinimo = dto.StockMinimo;
            material.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var material =
                await _context.Materiales.FindAsync(id);

            if (material == null)
                return false;

            _context.Materiales.Remove(material);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
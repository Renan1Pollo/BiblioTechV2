using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioTech_v2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json.Linq;

namespace BiblioTech_v2.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly Contexto _context;

        public LivroController(Contexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string busca, string tipo)
        {
            List<Livro> livros = _context.Livros.ToList();

            if (busca != null && tipo != null)
            {

                if (tipo == "titulo")
                {
                    livros = _context.Livros.Where(l => l.Titulo.Contains(busca)).ToList();
                }
                else if (tipo == "codGenero")
                {
                    livros = _context.Livros.Where(l => l.IdGenero == Convert.ToInt32(busca)).ToList();
                }
                else if (tipo == "genero")
                {
                    Genero genero = await _context.Generos.FirstOrDefaultAsync(g => g.Descricao.Contains(busca));
                    livros = _context.Livros.Where(l => l.IdGenero == genero.Id).ToList();
                }
                else if (tipo == "autor")
                {
                    livros = _context.Livros.Where(l => l.Autor.Contains(busca)).ToList();
                }
            }

            return View(livros);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Livros == null)
            {
                return NotFound();
            }

            var livro = await _context.Livros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdGenero,Titulo,Sinopse,Autor,Volume,Quantidade")] Livro livro)
        {
            if (ModelState.IsValid)
            {
                var generoExists = await _context.Generos.AnyAsync(g => g.Id == livro.IdGenero);

                if (!generoExists)
                {
                    ModelState.AddModelError("IdGenero", "O gênero selecionado não existe.");
                    return View(livro);
                }

                _context.Add(livro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Livros == null)
            {
                return NotFound();
            }

            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
            {
                return NotFound();
            }
            return View(livro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdGenero,Titulo,Sinopse,Autor,Volume,Quantidade")] Livro livro)
        {
            if (id != livro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var generoExists = await _context.Generos.AnyAsync(g => g.Id == livro.IdGenero);

                if (!generoExists)
                {
                    ModelState.AddModelError("IdGenero", "O gênero selecionado não existe.");
                    return View(livro);
                }

                try
                {
                    _context.Update(livro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivroExists(livro.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Livros == null)
            {
                return NotFound();
            }

            var livro = await _context.Livros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livro == null)
            {
                return NotFound();
            }

            return View(livro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Livros == null)
            {
                return Problem("Entity set 'Contexto.Livros'  is null.");
            }
            var livro = await _context.Livros.FindAsync(id);
            if (livro != null)
            {
                _context.Livros.Remove(livro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LivroExists(int id)
        {
            return (_context.Livros?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

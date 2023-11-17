using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioTech_v2.Models;
using Microsoft.AspNetCore.Authorization;

namespace BiblioTech_v2.Controllers
{
    [Authorize]
    public class EmprestimoController : Controller
    {
        private readonly Contexto _context;

        public EmprestimoController(Contexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string busca, string tipo)
        {
            List<Emprestimo> emprestimos = _context.Emprestimos.ToList();

            if (busca != null && tipo != null)
            {

                if (tipo == "usuario")
                {
                    Usuario usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nome.Contains(busca));
                    emprestimos = _context.Emprestimos.Where(e => e.IdLivro == usuario.Id).ToList();
                }
                else if (tipo == "titulo")
                {
                    Livro livro = await _context.Livros.FirstOrDefaultAsync(l => l.Titulo.Contains(busca));
                    emprestimos = _context.Emprestimos.Where(e => e.IdLivro == livro.Id).ToList();
                }
                else if (tipo == "codUsuario")
                {
                    emprestimos = _context.Emprestimos.Where(e => e.IdUsuario == Convert.ToInt32(busca)).ToList();
                }
                else if (tipo == "codLivro")
                {
                    emprestimos = _context.Emprestimos.Where(e => e.IdLivro == Convert.ToInt32(busca)).ToList();
                }
            }

            return View(emprestimos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Emprestimos == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUsuario,IdLivro,DataRetirada,Devolvido")] Emprestimo emprestimo)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    var userExists = await _context.Usuarios.AnyAsync(u => u.Id == emprestimo.IdUsuario);
                    if (!userExists)
                    {
                        ModelState.AddModelError("IdUsuario", "O usuário selecionado não existe.");
                        return View(emprestimo);
                    }

                    var livro = await _context.Livros.FindAsync(emprestimo.IdLivro);
                    if (livro == null)
                    {
                        ModelState.AddModelError("IdLivro", "O livro selecionado não existe.");
                        return View(emprestimo);
                    }

                    if (livro.Quantidade <= 0)
                    {
                        ModelState.AddModelError("IdLivro", "O livro selecionado não está disponível.");
                        return View(emprestimo);
                    }

                    _context.Add(emprestimo);
                    livro.Quantidade--;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return View(emprestimo);

            }
            return View(emprestimo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Emprestimos == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
            {
                return NotFound();
            }
            return View(emprestimo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUsuario,IdLivro,DataRetirada,Devolvido")] Emprestimo emprestimo)
        {
            if (id != emprestimo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userExists = await _context.Usuarios.AnyAsync(u => u.Id == emprestimo.IdUsuario);

                if (!userExists)
                {
                    ModelState.AddModelError("IdUsuario", "O usuário selecionado não existe.");
                    return View(emprestimo);
                }

                var livroExists = await _context.Livros.AnyAsync(l => l.Id == emprestimo.IdLivro);

                if (!livroExists)
                {
                    ModelState.AddModelError("IdLivro", "O livro selecionado não existe.");
                    return View(emprestimo);
                }

                try
                {
                    _context.Update(emprestimo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmprestimoExists(emprestimo.Id))
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
            return View(emprestimo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Emprestimos == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Emprestimos == null)
            {
                return Problem("Entity set 'Contexto.Emprestimos'  is null.");
            }

            var emprestimo = await _context.Emprestimos.FindAsync(id);

            if (emprestimo != null)
            {
                _context.Emprestimos.Remove(emprestimo);

                var livro = await _context.Livros.FindAsync(emprestimo.IdLivro);

                if (livro != null)
                {
                    livro.Quantidade++;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmprestimoExists(int id)
        {
            return (_context.Emprestimos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

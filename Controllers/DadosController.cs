using BiblioTech_v2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace BiblioTech_v2.Controllers
{
    [Authorize]
    public class DadosController : Controller
    {
        private readonly Contexto _context;
        private List<string> vGeneros;

        public DadosController(Contexto context)
        {
            _context = context;
        }

        public IActionResult Usuarios()
        {
            _context.Database.ExecuteSqlRaw("delete from Usuarios");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Usuarios', RESEED, 0)");

            List<string> vNomeMas = new List<string> { "Miguel", "Jonas", "Bernardo", "Higor", "Davi", "Enzo", "Renan", "Pedro", "Gabriel", "Luan", "Matheus", "Lucas", "Benjamin", "Nicolas", "Guilherme", "Rafael", "Joaquim", "Samuel", "Enzo Gabriel", "João Miguel", "Henrique", "Gustavo", "Murilo", "Pedro Henrique", "Pietro", "Lucca", "Felipe", "João Pedro", "Isaac", "Benício", "Daniel", "Anthony", "Leonardo", "Davi Lucca", "Bryan", "Eduardo", "João Lucas", "Victor", "João", "Cauã", "Antônio", "Vicente", "Caleb", "Gael", "Bento", "Caio", "Emanuel", "Vinícius", "João Guilherme", "Davi Lucas", "Noah", "João Gabriel", "João Victor", "Luiz Miguel", "Francisco", "Kaique", "Otávio", "Augusto", "Levi", "Yuri", "Enrico", "Thiago", "Ian", "Victor Hugo", "Thomas", "Henry", "Luiz Felipe", "Ryan", "Arthur Miguel", "Davi Luiz", "Nathan", "Pedro Lucas", "Davi Miguel", "Raul", "Pedro Miguel", "Luiz Henrique", "Luan", "Erick", "Martin", "Bruno", "Rodrigo", "Luiz Gustavo", "Arthur Gabriel", "Breno", "Kauê", "Enzo Miguel", "Fernando", "Arthur Henrique", "Luiz Otávio", "Carlos Eduardo", "Tomás", "Lucas Gabriel", "André", "José", "Yago", "Danilo", "Anthony Gabriel", "Ruan", "Miguel Henrique", "Oliver" };
            List<string> vNomeFem = new List<string> { "Alice", "Sophia", "Helena", "Valentina", "Laura", "Isabella", "Manuela", "Júlia", "Heloísa", "Luiza", "Maria Luiza", "Lorena", "Lívia", "Giovanna", "Maria Eduarda", "Beatriz", "Maria Clara", "Cecília", "Eloá", "Lara", "Maria Júlia", "Isadora", "Mariana", "Emanuelly", "Ana Júlia", "Ana Luiza", "Ana Clara", "Melissa", "Yasmin", "Maria Alice", "Isabelly", "Lavínia", "Esther", "Sarah", "Elisa", "Antonella", "Rafaela", "Maria Cecília", "Liz", "Marina", "Nicole", "Maitê", "Isis", "Alícia", "Luna", "Rebeca", "Agatha", "Letícia", "Maria-", "Gabriela", "Ana Laura", "Catarina", "Clara", "Ana Beatriz", "Vitória", "Olívia", "Maria Fernanda", "Emilly", "Maria Valentina", "Milena", "Maria Helena", "Bianca", "Larissa", "Mirella", "Maria Flor", "Allana", "Ana Sophia", "Clarice", "Pietra", "Maria Vitória", "Maya", "Laís", "Ayla", "Ana Lívia", "Eduarda", "Mariah", "Stella", "Ana", "Gabrielly", "Sophie", "Carolina", "Maria Laura", "Maria Heloísa", "Maria Sophia", "Fernanda", "Malu", "Analu", "Amanda", "Aurora", "Maria Isis", "Louise", "Heloise", "Ana Vitória", "Ana Cecília", "Ana Liz", "Joana", "Luana", "Antônia", "Isabel", "Bruna" };

            for (int i = 0; i < 100; i++)
            {
                Usuario usuario = new Usuario();

                usuario.Nome = (i % 2 == 0) ? vNomeMas[i / 2] : vNomeFem[i / 2];
                usuario.Email = GerarEmails(usuario.Nome);
                usuario.RA = GerarRAs();

                _context.Usuarios.Add(usuario);
            }
            _context.SaveChanges();

            return View(_context.Usuarios.OrderBy(o => o.Nome).ToList());
        }

        public IActionResult Generos()
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM Generos");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Generos', RESEED, 0)");

            vGeneros = new List<string> {
                "Ação",
                "Comédia",
                "Drama",
                "Ficção Científica",
                "Terror",
                "Romance",
                "Animação",
                "Documentário",
                "Suspense",
                "Fantasia"
            };

            foreach (string tipoGenero in vGeneros)
            {
                Genero genero = new Genero { Descricao = tipoGenero };
                _context.Generos.Add(genero);
            }
            _context.SaveChanges();

            return View(_context.Generos.OrderBy(g => g.Descricao).ToList());
        }

        public IActionResult Livros()
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM Livros");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Livros', RESEED, 0)");

            Random random = new Random();

            if (vGeneros.IsNullOrEmpty())
            {
                vGeneros = _context.Generos.ToList().Select(g => g.Descricao).ToList();
            }

            foreach (string tipoGenero in vGeneros)
            {
                for (int i = 1; i < 3; i++)
                {
                    Livro livro = new Livro
                    {
                        IdGenero = _context.Generos.Single(g => g.Descricao == tipoGenero).Id,
                        Titulo = $"Livro de {tipoGenero} {i}",
                        Sinopse = $"Sinopse do Livro de {tipoGenero} {i}",
                        Autor = $"Autor {i}",
                        Volume = $"{i}",
                        Quantidade = random.Next(1, 20)
                    };
                    _context.Livros.Add(livro);
                }
            }

            _context.SaveChanges();
            return View(_context.Livros.OrderBy(l => l.Titulo).ToList());
        }

        public IActionResult Emprestimos()
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM Emprestimos");
            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Emprestimos', RESEED, 0)");

            List<int> usuarioIds = _context.Usuarios.Select(u => u.Id).ToList();
            List<int> livrosIds = _context.Livros.Select(l => l.Id).ToList();

            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                int usuarioId = usuarioIds[random.Next(usuarioIds.Count)];
                int livroId = livrosIds[random.Next(livrosIds.Count)];

                Livro livro = _context.Livros.Find(livroId);

                Emprestimo emprestimo = new Emprestimo
                {
                    IdUsuario = usuarioId,
                    DataRetirada = DateTime.Now.AddDays(-random.Next(1, 30)),
                    IdLivro = livroId,
                };

                livro.Quantidade--;
                _context.Emprestimos.Add(emprestimo);
            }
            _context.SaveChanges();
            return View(_context.Emprestimos.OrderBy(e => e.Id).ToList());
        }

        public string GerarEmails(string nome)
        {
            Random rand = new Random();
            string[] dominios = { "gmail.com", "outlook.com", "hotmail.com" };

            string descricao = nome.Trim();
            string dominio = dominios[rand.Next(dominios.Length)];
            string email = descricao + "@" + dominio;

            return email;
        }

        public string GerarRAs()
        {
            Random rand = new Random();
            string ra = rand.Next(100000, 1000000).ToString();

            return ra;
        }
    }
}
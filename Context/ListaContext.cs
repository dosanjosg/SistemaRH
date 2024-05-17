using Microsoft.EntityFrameworkCore;
using SistemaCadastro.Models;

namespace SistemaCadastro.Context
{
    public class ListaContext : DbContext 
    {
        public ListaContext(DbContextOptions<ListaContext> options) : base(options) 
        {

        }
        public DbSet<Funcionario> Funcionarios { get; set; }
    }
}
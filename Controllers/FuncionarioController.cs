using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

using SistemaCadastro.Context;
using SistemaCadastro.Models;

namespace SistemaCadastro.Controllers
{
    public class FuncionarioController : Controller 
    {
        private readonly ListaContext _context;
        private readonly string _saConnectionString;
        private readonly string _tableName;

        public FuncionarioController(ListaContext context, IConfiguration configuration) 
        {
            _context = context;
            _saConnectionString = configuration.GetValue<string>("ConnectionStrings:SAConnectionString");
            _tableName = configuration.GetValue<string>("ConnectionStrings:AzureTableName");
        }

        private TableClient GetTableClient() 
        {
            var serviceClient = new TableServiceClient(_saConnectionString);
            var tableClient = serviceClient.GetTableClient(_tableName);

            tableClient.CreateIfNotExists();
            return tableClient;
        }

        public IActionResult Index() 
        {
            var funcionarios = _context.Funcionarios.ToList();
            return View(funcionarios);
        }
        public IActionResult Adicionar() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Adicionar(Funcionario funcionario) 
        {
            if(ModelState.IsValid)
            {
                _context.Funcionarios.Add(funcionario);
                _context.SaveChanges();

                var tableClient = GetTableClient();

                DateTime dataAdmissaoUtc = DateTime.SpecifyKind(funcionario.DataAdmissao, DateTimeKind.Utc);

                double salarioFormatDouble = (double) funcionario.Salario;


                var funcionarioLog = new FuncionarioLog(funcionario, TipoAcao.Inclusao, funcionario.Departamento, Guid.NewGuid().ToString())
                {
                    DataAdmissao = dataAdmissaoUtc,
                    Salario = (decimal)(double)salarioFormatDouble
                };

                var table = tableClient;

                table.AddEntity(funcionarioLog);

                return RedirectToAction("Index");
            }
            
            

            return View(funcionario);
        }

        public IActionResult Atualizar(int id)
        {
            var funcionario = _context.Funcionarios.Find(id);

            if(funcionario == null)
                return RedirectToAction("Index");
            return View(funcionario);
        }

        [HttpPost]
        public IActionResult Atualizar(Funcionario funcionario) 
        {
            var infoFuncionario = _context.Funcionarios.Find(funcionario.Id);
            
            if(infoFuncionario is null) 
                return NotFound();
            

            infoFuncionario.Nome = funcionario.Nome;
            infoFuncionario.Endereco = funcionario.Endereco;
            infoFuncionario.Ramal = funcionario.Ramal;
            infoFuncionario.EmailProfissional = funcionario.EmailProfissional;
            infoFuncionario.Departamento = funcionario.Departamento;
            infoFuncionario.Salario = funcionario.Salario;
            infoFuncionario.DataAdmissao = funcionario.DataAdmissao;

            _context.Funcionarios.Update(infoFuncionario);
            _context.SaveChanges();

            var tableClient = GetTableClient();

            DateTime dataAdmissaoUtc = DateTime.SpecifyKind(funcionario.DataAdmissao, DateTimeKind.Utc);

            double salarioFormatDouble = (double) funcionario.Salario;

            var funcionarioLog = new FuncionarioLog(funcionario, TipoAcao.Atualizacao, funcionario.Departamento, Guid.NewGuid().ToString()) 
            {
                DataAdmissao = dataAdmissaoUtc,
                Salario = (decimal)(double)salarioFormatDouble
            };

            var table = tableClient;

            table.AddEntity(funcionarioLog);

            return RedirectToAction("Index");
        }

        public IActionResult Detalhes(int id) 
        {
            var funcionario = _context.Funcionarios.Find(id);

            if(funcionario is null)
                return RedirectToAction("Index");

            return View(funcionario);
        }

        public IActionResult Deletar(int id) 
        {
            var funcionario = _context.Funcionarios.Find(id);

            if(funcionario is null)
                return RedirectToAction("Index");

            return View(funcionario);
        } 

        [HttpPost]
        public IActionResult Deletar(Funcionario funcionario) 
        {
            var funcionarioInfo = _context.Funcionarios.Find(funcionario.Id);

            if (funcionarioInfo == null)
                return RedirectToAction("Index"); // Funcionário não encontrado, redireciona para a página principal

            _context.Funcionarios.Remove(funcionarioInfo);
            _context.SaveChanges();

            var tableClient = GetTableClient();

            DateTime dataAdmissaoUtc = DateTime.SpecifyKind(funcionarioInfo.DataAdmissao, DateTimeKind.Utc);

            double salarioFormatDouble = (double) funcionario.Salario;

            var funcionarioLog = new FuncionarioLog(funcionario, TipoAcao.Remocao, funcionarioInfo.Departamento, Guid.NewGuid().ToString())
            {
                DataAdmissao = dataAdmissaoUtc,
                Salario = (decimal)(double)salarioFormatDouble
            };

            var table = tableClient;

            table.AddEntity(funcionarioLog);

            return RedirectToAction("Index");
        }
    }
}
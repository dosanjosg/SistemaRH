using System.Text.Json;
using Azure;
using Azure.Data.Tables;

namespace SistemaCadastro.Models
{
    public class FuncionarioLog : Funcionario, ITableEntity
    {

        public FuncionarioLog(){}
        public FuncionarioLog(Funcionario funcionario, TipoAcao tipoAcao, string partitionKey, string rowkey) 
        {
            base.Id = funcionario.Id;
            base.Nome = funcionario.Nome;
            base.Ramal = funcionario.Ramal;
            base.EmailProfissional = funcionario.EmailProfissional;
            base.Departamento = funcionario.Departamento;
            base.DataAdmissao = funcionario.DataAdmissao;
            TipoAcao = tipoAcao;
            JSON = JsonSerializer.Serialize(funcionario);
            PartitionKey = partitionKey;
            RowKey = rowkey;
        }
        public TipoAcao TipoAcao { get; set; }
        public string JSON { get; set; }
        public string PartitionKey { get ; set ; }
        public string RowKey { get ; set ; }
        public DateTimeOffset? Timestamp { get ; set ; }
        public ETag ETag { get ; set ; }
    }
}
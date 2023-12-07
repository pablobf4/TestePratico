using MediatR;
using Dapper;
using Questao5.Infrastructure.Sqlite;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Enumerators;
using Questao5.Domain.Entities;

namespace Questao5.Application.Queries
{
    public class GetContaCorrenteQueryHandler : IRequestHandler<GetContaCorrenteQuery, ContaCorrente>
    {
        private readonly DatabaseConfig _databaseConfig;

        public GetContaCorrenteQueryHandler(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig ?? throw new ArgumentNullException(nameof(databaseConfig));
        }

        public async Task<ContaCorrente> Handle(GetContaCorrenteQuery query, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            try
            {
                if (!ValidarContaCorrente(query.IdContaCorrente, connection, query))
                {
                    throw new Exception(TipoErro.INACTIVE_ACCOUNT.ToString());// Valor inválido
                }

                decimal saldo = SaldoContaCorrente(query.IdContaCorrente, connection);
                var resultado = ContaCorrente(query.IdContaCorrente, connection);
                return new ContaCorrente
                {
                    Ativo = 1,
                    Nome = resultado.Item2,
                    Numero = resultado.Item1,
                    IdContaCorrente = query.IdContaCorrente,
                    Saldo = saldo
                };
            }
            catch (Exception ex)
            {
                connection.Execute("INSERT INTO idempotencia (requisicao, resultado) VALUES (@Requisicao, @Resultado);",
                   new { Requisicao = query, Resultado = ex.Message });
                throw new Exception(ex.ToString());
            }
        }


        private bool ValidarContaCorrente(string idContaCorrente, SqliteConnection connection, GetContaCorrenteQuery command)
        {
            connection.Open();
         
            string query = "SELECT  COUNT(*) AS Quantidade, CASE  WHEN EXISTS(SELECT 1 FROM contacorrente WHERE idcontacorrente = @IdContaCorrente and ativo = 1 ) THEN 1  ELSE 0 END AS Ativo ";
            var result = connection.QueryFirstOrDefault<(int Quantidade, int Ativo)>(query, new { IdContaCorrente = command.IdContaCorrente });
            connection.Close();

            if (result.Equals(default))
            {
                throw new Exception(TipoErro.INACTIVE_ACCOUNT.ToString());
            }
            else if (result.Ativo == 0)
            {
                throw new Exception(TipoErro.INACTIVE_ACCOUNT.ToString());
            }
            else if (result.Quantidade > 0)
            {
                return true ;
            }
            else
            {
                throw new Exception(TipoErro.INVALID_TYPE.ToString());
            }
        }

        private decimal SaldoContaCorrente(string idContaCorrente, SqliteConnection connection)
        {
            connection.Open();

            string query = "SELECT SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END) - SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END) AS Saldo FROM movimento WHERE idcontacorrente = @IdContaCorrente";


            var saldo = connection.QueryFirstOrDefault<decimal>(query, new { IdContaCorrente = idContaCorrente });
            if (saldo == null)
            {
                saldo = 0;
            }

            connection.Close();
            
            return saldo;
           
        }

        private (int,string) ContaCorrente(string idContaCorrente, SqliteConnection connection)
        {
            connection.Open();

            string query = "SELECT numero, nome from contacorrente WHERE idcontacorrente = @IdContaCorrente";

            var result = connection.QueryFirstOrDefault<(int numero, string nome)>(query, new { IdContaCorrente = idContaCorrente });

            connection.Close();

            return result;
        }
    }
}

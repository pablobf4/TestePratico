using Questao5.Application.Commands;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Domain.Enumerators;
using Questao5.Infrastructure.Sqlite;
using MediatR;

namespace Questao5.Application.Handlers
{
    public class CriarMovimentoCommandHandler : IRequestHandler<MovimentoCriarCommand, Movimento>
    {
        private readonly DatabaseConfig _databaseConfig;
        public CriarMovimentoCommandHandler(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig ?? throw new ArgumentNullException(nameof(databaseConfig));
        }


        public async Task<Movimento> Handle(MovimentoCriarCommand command,CancellationToken cancellationToken)
        {
            ValidarContaCorrente(command);
            using var connection = new SqliteConnection(_databaseConfig.Name);
                try
                {
                   connection.Open();
                   using var transaction = connection.BeginTransaction();
                   connection.Execute("INSERT INTO movimento  (tipomovimento , datamovimento  , valor , idcontacorrente) VALUES (@TipoMovimento, @DataMovimento, @Valor, @IdContaCorrente);",
                        new { TipoMovimento = command.TipoMovimento, DataMovimento = DateTime.Now, Valor = command.Valor, IdContaCorrente = command.IdContaCorrente });
                   transaction.Commit();
                    var lastInsertId = connection.QueryFirstOrDefault<int>("SELECT last_insert_rowid()");
                    var novoMovimento = connection.QueryFirstOrDefault<Movimento>("SELECT * FROM movimento WHERE idmovimento = @IdMovimento", new { IdMovimento = lastInsertId });
                    
                
                 return novoMovimento;
                }

                catch (Exception ex)
                {
                connection.Execute("INSERT INTO idempotencia (requisicao, resultado) VALUES (@Requisicao, @Resultado);",
                new { Requisicao = command, Resultado = ex.Message });
                throw new Exception(ex.ToString());
                 }
                finally
                {
                    connection.Close();
                }
            }


      

        private string ValidarContaCorrente(MovimentoCriarCommand command)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            if (command.Valor <= 0)
                 throw new Exception(TipoErro.INVALID_VALUE.ToString());// Valor inválido

            if (command.TipoMovimento != 'D' && command.TipoMovimento != 'C')
                throw new Exception(TipoErro.INVALID_TYPE.ToString());
            connection.Open();
            string query = "SELECT  COUNT(*) AS Quantidade, CASE  WHEN EXISTS(SELECT 1 FROM contacorrente WHERE idcontacorrente = @IdContaCorrente and ativo = 1) THEN '1'   ELSE '0' END AS Ativo ";

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
                return null;
            }
            else
            {
                throw new Exception(TipoErro.INVALID_TYPE.ToString());
            }
        }
    }
}

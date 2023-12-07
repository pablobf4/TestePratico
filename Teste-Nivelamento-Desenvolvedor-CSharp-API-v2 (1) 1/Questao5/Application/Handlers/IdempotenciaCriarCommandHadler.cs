using Dapper;
using MediatR;
using Microsoft.Data.Sqlite;
using Questao5.Application.Commands;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Application.Handlers
{
    public class IdempotenciaCriarCommandHandler : IRequestHandler<IdempotenciaCriarCommand, Idempotencia>
    {
        private readonly DatabaseConfig _databaseConfig;

        public IdempotenciaCriarCommandHandler(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig ?? throw new ArgumentNullException(nameof(databaseConfig));
        }

        public async Task<Idempotencia> Handle(IdempotenciaCriarCommand command, CancellationToken cancellationToken)
        {
            using var connection = new SqliteConnection(_databaseConfig.Name);
            try
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                connection.Execute("INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado);",
                    new { ChaveIdempotencia = command.ChaveIdempotencia, Requisicao = command.Requisicao, Resultado = command.Resultado });
                transaction.Commit();

                return default;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }
    }

}

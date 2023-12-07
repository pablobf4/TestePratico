using MediatR;
using Questao5.Domain.Entities;

namespace Questao5.Application.Commands
{
    public class IdempotenciaCriarCommand : IRequest<Idempotencia>
    {
        public string ChaveIdempotencia { get; set; }
        public string Requisicao { get; set; }
        public string Resultado { get; set; }
    }
}

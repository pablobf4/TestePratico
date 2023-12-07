using MediatR;
using Questao5.Domain.Entities;

namespace Questao5.Application.Commands
{
    public class MovimentoCriarCommand:IRequest<Movimento>
    {
        public int IdMovimento { get; set; }
        public string IdContaCorrente { get; set; }
        public char TipoMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}

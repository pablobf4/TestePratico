using MediatR;
using Questao5.Domain.Entities;

namespace Questao5.Application.Queries
{
    public class GetContaCorrenteQuery : IRequest<ContaCorrente>
    {
        public string IdContaCorrente { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; }
        public int Ativo { get; set; }

    }
}

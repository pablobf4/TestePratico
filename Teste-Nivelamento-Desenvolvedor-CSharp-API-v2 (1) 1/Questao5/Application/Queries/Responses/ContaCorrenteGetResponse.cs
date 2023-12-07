namespace Questao5.Application.Queries.Responses
{
    public class ContaCorrenteGetResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string IdContaCorrente { get; set; }
        public DateTime DataHoraResposta { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; }
        public string Ativo { get; set; }
        public decimal Saldo { get; set; }

    }
}

namespace Questao5.Domain.Entities
{
    public class Movimento
    {
       
            public int IdMovimento { get; set; }
            public string IdContaCorrente { get; set; }
            public DateTime DataMovimento { get; set; }
            public char TipoMovimento { get; set; }
            public float Valor { get; set; }
    }
}

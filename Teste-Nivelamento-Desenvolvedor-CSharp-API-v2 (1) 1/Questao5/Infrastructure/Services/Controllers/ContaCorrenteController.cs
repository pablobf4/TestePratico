using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Queries;
using MediatR;
using Questao5.Application.Queries.Responses;
using Questao5.Domain.Entities;

namespace SuaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContaCorrenteController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("saldo")]
        public async Task<IActionResult> ObterSaldoContaCorrente([FromQuery] string idContaCorrente)
        {
            try
            {
                var query = new GetContaCorrenteQuery { IdContaCorrente = idContaCorrente };
                var contaCorrente = await _mediator.Send(query);

                return Ok(new ContaCorrenteGetResponse
                {
                    Sucesso = true,
                    Mensagem = "Saldo retornando ",
                    IdContaCorrente = contaCorrente.IdContaCorrente,
                    Numero = contaCorrente.Numero,
                    Nome = contaCorrente.Nome,
                    DataHoraResposta = DateTime.Now, 
                    Saldo = contaCorrente.Saldo,
                    Ativo = "Conta Ativa"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ContaCorrenteGetResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    IdContaCorrente = null
                });
            }
        }
    }
}

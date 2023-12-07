using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Questao5.Application.Commands;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Handlers;
using MediatR;

namespace SuaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentoController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MovimentoController(IMediator mediator)
        {
              _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("criar-movimento")]
        public async Task<IActionResult> CriarMovimento([FromBody] MovimentoCriarCommand command)
        {
            try 
            {
                var query = new MovimentoCriarCommand { IdContaCorrente = command.IdContaCorrente,TipoMovimento = command.TipoMovimento,Valor = command.Valor };
                var contaCorrente = await _mediator.Send(query);

             //   var resultadoMovimento = await _mediator.Handle(command, new CancellationToken());

                return Ok(new MovimentoCriarResponse
                {
                    Sucesso = true,
                    Mensagem = "Movimento feito com sucesso",
                    IdMovimento = contaCorrente.IdMovimento
                }); 
            }
            catch (Exception ex)
            {
                var erroResponse = new MovimentoCriarResponse
                {
                    Sucesso = false,
                    Mensagem = ex.Message,
                    IdMovimento = null 
                };
                return BadRequest(erroResponse); 
            }
        }
    }
}

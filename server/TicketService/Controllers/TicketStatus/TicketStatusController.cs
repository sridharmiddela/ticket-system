using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicketService.QueryStack.TicketStatus;
using TicketService.RealTime;
using Microsoft.AspNetCore.SignalR.Infrastructure;

namespace TicketService.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class TicketStatusController : ApiHubController<Broadcaster>
    {

        private readonly IMediator _mediator;
        private ILogger<TicketStatusController> _logger;
        public TicketStatusController(IConnectionManager signalRConnectionManager,IMediator mediator, ILogger<TicketStatusController> logger):base(signalRConnectionManager)
        {
            _mediator = mediator;
             _logger = logger;
        }

        public async Task<IEnumerable<TicketStatusModel>> Get(GetTicketStatusList.Query query)
        {
            try{
            var model = await _mediator.Send(query);

            return model;
            }catch(Exception ex){
                Console.WriteLine("Index :" + ex.ToString());
            }
            return  new List<TicketStatusModel>();
            
        }

        [HttpGet("{id}", Name = "GetTicketStatus")]
        public async Task<TicketStatusModel> GetTicketStatus(GetTicketStatus.Query query)
        {
            Console.WriteLine("GetTicketStatus.id::" + query.Id);

            var model = await _mediator.Send(query);
            if (model == null)
                NotFound();

            await Clients.Group(model.Id.ToString()).AddfEED(model);
            await Clients.All.UpdateStatus(model);

            Console.WriteLine("GetTicketStatus :" + model);

            return model;

        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<int> Create([FromBody] CommandStack.TicketStatus.Create.Command model)
        {
            _logger.LogInformation("StatusController:Create => " + model.Name);
                
            return await _mediator.Send(model);
        }

    }
}
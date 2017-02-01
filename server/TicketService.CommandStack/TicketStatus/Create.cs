using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using TicketService.Repository;
using Microsoft.Extensions.Logging;


namespace TicketService.CommandStack.TicketStatus
{
    public class Create
    {
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(m => m.Name).NotNull().Length(3, 50);
                RuleFor(m => m.SortOrder).GreaterThan(0);
                RuleFor(m => m.IsDefault).NotNull();
            }
        }

        public class Command : IRequest<int>
        {
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Name { get; set; }

            [Required]
            public int IsDefault { get; set; }

            [Required]
            public int SortOrder { get; set; }

            public override string ToString()
            {
                return "Name:{Name}" + Name;
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, int>
        {
            private readonly IDatabaseService _context;
            private ILogger<CommandHandler> _logger;
            public CommandHandler(IDatabaseService context, ILogger<CommandHandler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<int> Handle(Command message)
            {
                _logger.LogInformation("Handle:Command => " + message.ToString());
                //Console.WriteLine();
                var status = Mapper.Map<Command, Domain.TicketStatus>(message);
                var newStatus = _context.TicketStatuses.Add(status);
                await _context.SaveAsync();
                return newStatus.Entity.Id;
            }
            // public async Task<int> Handle(Command message)
            // {
            //     var status = Mapper.Map<Command, Models.TicketStatus>(message);
            //     var newStatus = _context.TicketStatuses.Add(status);
            //     await _context.SaveChangesAsync();
            //     return newStatus.Entity.Id; 
            // }
        }
    }
}
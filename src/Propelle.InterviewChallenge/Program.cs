using FastEndpoints;
using Propelle.InterviewChallenge.Application;
using Propelle.InterviewChallenge.Application.Domain;
using Propelle.InterviewChallenge.Application.Domain.Events;
using Propelle.InterviewChallenge.Application.EventBus;
using Propelle.InterviewChallenge.Application.EventHandlers;
using Propelle.InterviewChallenge.EventHandling;

namespace Propelle.InterviewChallenge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<PaymentsContext>();
            builder.Services.AddSingleton<ISmartInvestClient, SmartInvestClient>();
            builder.Services.AddSingleton<InMemoryEventExchange>();
            builder.Services.AddSingleton<FileDeadLetterEventExchange>();
            builder.Services.AddSingleton<Application.EventBus.IEventBus, SimpleEventBus>();
            builder.Services.AddTransient<EventHandling.IEventHandler<DepositMade>, SubmitDeposit>();
            builder.Services.AddTransient<EventHandling.IEventHandler<DepositMade>, PersistDeposit>();
            builder.Services.AddFastEndpoints();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseFastEndpoints(x =>
            {
                /* Allow anonymous access globally for the purposes of this exercise */
                x.Endpoints.Configurator = epd => epd.AllowAnonymous();
            });
            app.Run();
        }
    }
}
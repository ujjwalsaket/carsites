using System;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer(IMapper mapper) : IConsumer<AuctionCreated>
{
    // private readonly IMapper _mapper = mapper;

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("---> Consuming auction created: " + context.Message.Id);

        var item = mapper.Map<Item>(context.Message);

        if (item.Model == "Foo") throw new ArgumentException("Cannot create a new car with the name Foo");

        await item.SaveAsync();
    }
}

using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper, IPublishEndpoint publishEndpoint) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
        {
            //temp code for synchronous communication
            var query = auctionDbContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            // var auctions = await auctionDbContext.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();

            // return mapper.Map<List<AuctionDto>>(auctions);
            return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await auctionDbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound("Unable to find the auction");

            return mapper.Map<AuctionDto>(auction);

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = mapper.Map<Auction>(createAuctionDto);

            // TODO add current user as seller
            auction.Seller = User.Identity.Name;

            auctionDbContext.Auctions.Add(auction);
            var newAuction = mapper.Map<AuctionDto>(auction);
            await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            // Happy Path
            // var newAuction = mapper.Map<AuctionDto>(auction);
            // await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

            if (!result) return BadRequest("Could not save the changes to auction db");

            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, newAuction);

        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await auctionDbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            // TODO: check seller  == username
            if (auction.Seller != User.Identity.Name) return Forbid();

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem updating the changes..");

        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAction(Guid id)
        {
            var auction = await auctionDbContext.Auctions.FindAsync(id);

            if (auction == null) return NotFound();

            // TODO: check seller == username
            if (auction.Seller != User.Identity.Name) return Forbid();

            auctionDbContext.Auctions.Remove(auction);

            await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem deleting the auction..");

        }

    }
}

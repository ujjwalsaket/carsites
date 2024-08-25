using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await auctionDbContext.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();

            return mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await auctionDbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound("Unable to find the auction");

            return mapper.Map<AuctionDto>(auction);

        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var auction = mapper.Map<Auction>(createAuctionDto);

            // TODO add current user as seller
            auction.Seller = "test";

            auctionDbContext.Auctions.Add(auction);

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save the changes to auction db");

            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, mapper.Map<AuctionDto>(auction));

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await auctionDbContext.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            // TODO: check seller  == username

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem updating the changes..");

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAction(Guid id)
        {
            var auction = await auctionDbContext.Auctions.FindAsync(id);

            if (auction == null) return NotFound();

            // TODO: check seller == username

            auctionDbContext.Auctions.Remove(auction);

            var result = await auctionDbContext.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem deleting the auction..");

        }

    }
}

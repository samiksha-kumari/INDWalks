using INDWalks.API.Data;
using INDWalks.API.Models.Domain;
using INDWalks.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INDWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;

        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        //GET ALL REGIONS
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            //GET data from db
            var regionsDomain = await dbContext.Regions.ToListAsync();

            //Map domain models to DTOs
            var regionsDTO = new List<RegionDto>();
            foreach (var regionDomain in regionsDomain)
            {
                regionsDTO.Add(new RegionDto()
                {
                    Id = regionDomain.Id,
                    Name = regionDomain.Name,
                    Code = regionDomain.Code,
                    RegionImageUrl = regionDomain.RegionImageUrl
                });
            }

            //Return DTOs
            return Ok(regionsDTO);
        }

        //get region by ID
        [HttpGet]
        [Route("{id:Guid}")]  // get mapped from route to a input parameter
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region =  dbContext.Regions.Find(id);
            var regionDoamin = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (regionDoamin == null)
            {
                return NotFound();
            }

            //Map/Convert Region Domail Model to Region DTO
            var regionDTO = new RegionDto()
            {
                Id = regionDoamin.Id,
                Name = regionDoamin.Name,
                Code = regionDoamin.Code,
                RegionImageUrl = regionDoamin.RegionImageUrl
            };

            return Ok(regionDTO);
        }

        //Post to create new region
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //Map or Convert DTO to Domain Model
            var regionDomailModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };


            //Use DomainModel to create Region

            await dbContext.Regions.AddAsync(regionDomailModel);
            await dbContext.SaveChangesAsync(); // EF saves the changes and it will reflected in SQL server.

            //Map Domain Model back to DTO
            var regionDto = new RegionDto
            {
                Id = regionDomailModel.Id,
                Name = regionDomailModel.Name,
                Code = regionDomailModel.Code,
                RegionImageUrl = regionDomailModel.RegionImageUrl
            };
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        //Update Region
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //Get the region from database // check if region exists or not
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Map DTO to Domain Model
            regionDomainModel.Code = updateRegionRequestDto.Code;
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            //Update region using Domain Model
            await dbContext.SaveChangesAsync();

            //Convert Domain Model to DTO
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Name = regionDomainModel.Name,
                Code = regionDomainModel.Code,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return Ok(regionDto);
        }

        //Delete a Region
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
            if(regionDomainModel == null)
            {
                return NotFound();
            }

            //delete region
            dbContext.Regions.Remove(regionDomainModel);
            await dbContext.SaveChangesAsync();

            // return deleted region back //map domain model to DTO
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Name = regionDomainModel.Name,
                Code = regionDomainModel.Code,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return Ok(regionDto);
        }
    }
}
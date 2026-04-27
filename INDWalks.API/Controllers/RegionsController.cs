using INDWalks.API.Data;
using INDWalks.API.Models.Domain;
using INDWalks.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAllRegions()
        {
            //GET data from db
            var regionsDomain = dbContext.Regions.ToList();

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
        public IActionResult GetById([FromRoute] Guid id)
        {
            //var region =  dbContext.Regions.Find(id);
            var regionDoamin = dbContext.Regions.FirstOrDefault(x => x.Id == id);
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
        public IActionResult Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //Map or Convert DTO to Domain Model
            var regionDomailModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };


            //Use DomainModel to create Region

            dbContext.Regions.Add(regionDomailModel);
            dbContext.SaveChanges(); // EF saves the changes and it will reflected in SQL server.

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
        public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //Get the region from database // check if region exists or not
            var regionDomainModel =  dbContext.Regions.FirstOrDefault(x => x.Id == id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Map DTO to Domain Model
            regionDomainModel.Code = updateRegionRequestDto.Code;
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            //Update region using Domain Model
            dbContext.SaveChanges();

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
        public IActionResult Delete([FromRoute] Guid id)
        {
            var regionDomainModel = dbContext.Regions.FirstOrDefault(x => x.Id == id);
            if(regionDomainModel == null)
            {
                return NotFound();
            }

            //delete region
            dbContext.Regions.Remove(regionDomainModel);
            dbContext.SaveChanges();

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
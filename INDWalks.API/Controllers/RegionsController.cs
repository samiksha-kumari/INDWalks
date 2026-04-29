using INDWalks.API.Data;
using INDWalks.API.Models.Domain;
using INDWalks.API.Models.DTO;
using INDWalks.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace INDWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
        }
        //GET ALL REGIONS
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            //GET data from db
            var regionsDomain = await regionRepository.GetAllAsync();

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
            var regionDoamin = await regionRepository.GetByIdAsync(id);
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
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };


            //Use DomainModel to create Region
            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            //Map Domain Model back to DTO
            var regionDto = new RegionDto
            {
                Id = regionDomainModel.Id,
                Name = regionDomainModel.Name,
                Code = regionDomainModel.Code,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        //Update Region
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {

            var regionDomainModel = new Region
            {
                Code = updateRegionRequestDto.Code,
                Name = updateRegionRequestDto.Name,
                RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            };
            //Get the region from database // check if region exists or not
            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

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
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

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
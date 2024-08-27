using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Core.Models.Country;
using AutoMapper;
using HotelListing.API.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using HotelListing.API.Core.Exceptions;
using Asp.Versioning;
using HotelListing.API.Core.Models;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.API.Controllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CountriesV2Controller : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesV2Controller> _logger;

        public CountriesV2Controller(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesV2Controller> logger)
        {
            _mapper = mapper;
            _countriesRepository = countriesRepository;
            _logger = logger;
        }

        // GET: api/Countries/GetAll
        // Refer on this how to use those queries: https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html#_Toc31360994
        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync<GetCountryDto>();

            _logger.LogInformation("{Controller} - {Method} - recordsCount:{count}",
                nameof(CountriesV2Controller),
                nameof(GetCountries),
                countries.Count
            );

            return Ok(countries);
        }

        // GET: api/Countries?startIndex=0&pageSize=5&pageNumber=2
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries([FromQuery] PagedQueryParameters pagedQueryParameters)
        {
            var pagedCountries = await _countriesRepository.GetAllAsync<GetCountryDto>(pagedQueryParameters);

            _logger.LogInformation("{Controller} - {Method}- recordsCount:{count}",
                nameof(CountriesV2Controller),
                nameof(GetPagedCountries),
                pagedCountries.Items.Count
            );

            return Ok(pagedCountries);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var countryDto = await _countriesRepository.GetDetails(id);
            return Ok(countryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountryDto)
        {
            if (id != updateCountryDto.Id)
            {
                return BadRequest("Invalid Record Id");
            }

            try
            {
                await _countriesRepository.UpdateAsync(id, updateCountryDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                bool isExists = await _countriesRepository.Exists(id);
                if (!isExists)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CountryDto>> PostCountry(CreateCountryDto createCountry, ApiVersion version)
        {
            var country = await _countriesRepository.AddAsync<CreateCountryDto, GetCountryDto>(createCountry);
            return CreatedAtAction(nameof(GetCountry), new { id = country.Id, version = version.ToString() }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Adminstartor")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _countriesRepository.DeleteAsync(id);
            return NoContent();
        }

    }
}

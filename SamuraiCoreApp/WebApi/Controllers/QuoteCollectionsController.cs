using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteCollectionsController : ControllerBase
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public QuoteCollectionsController(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuoteCollection([FromBody] IEnumerable<QuoteCreationModel> quoteCollection) {
            var quoteEntities = _mapper.Map<IEnumerable<Quote>>(quoteCollection);

            foreach (var quoteEntity in quoteEntities)
            {
                _quoteRepository.AddQuote(quoteEntity);
            }

            await _quoteRepository.SaveChangeAsync();

            return Ok();
        }

        //api/quotecollections/(id1, id2, id3, ...)
        [HttpGet]
        [Route("({quoteIds})")]
        public async Task<IActionResult> GetQuoteCollections([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> quoteIds)
        {

            return Ok();
        }
    }
}
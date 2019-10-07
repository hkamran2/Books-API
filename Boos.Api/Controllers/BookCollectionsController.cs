using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Books.Api.Filters;
using Books.Api.Models;
using Books.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Books.Api.Controllers
{
    [Route("api/bookcollections")]
    [ApiController]
    //Use this at the controller level
    [BooksResultFilter]
    public class BookCollectionsController : Controller
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BookCollectionsController(IBooksRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //api/bookcollections/(id1,id2)
        [HttpGet("({bookIds})", Name = "GetBookCollection")]
        public async Task<IActionResult> GetBookCollection(
            //Pass in the custom model binder
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> bookIds)
        {
            var bookEntities = await _booksRepository.GetBooksAsync(bookIds);
            //Have check for comparing the number of results
            if (bookIds.Count() != bookEntities.Count())
            {
                return NotFound();
            }

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> CreateBookCollection(
            [FromBody] IEnumerable<BookCreation> bookCollection)
        {
            var bookEntities = _mapper.Map<IEnumerable<Entities.Book>>(bookCollection);
            foreach (var book in bookEntities)
            {
                _booksRepository.AddBook(book);
            }

            await _booksRepository.SaveChangesAsync();
            //Get the saved books again
            var booksToReturn = await _booksRepository.GetBooksAsync(
                bookEntities.Select(b => b.Id).ToList());
            //Extract the ids for the location header
            var bookIds = string.Join(",", booksToReturn.Select(a => a.Id));
            //Add the location header
            return CreatedAtRoute("GetBookCollection",
                new {bookIds},
                booksToReturn);
        }
    }
}

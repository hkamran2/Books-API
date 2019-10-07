using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.Api.Entities;

namespace Books.Api.Services
{
    public interface IBooksRepository
    {
        IEnumerable<Entities.Book> GetBooks();
//        Entities.Book GetBook(Guid id);
        Task<IEnumerable<Entities.Book>> GetBooksAsync();
        Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds);
        Task<Entities.Book> GetBookAsync(Guid id);
        void AddBook(Book bookToAdd);
        Task<bool> SaveChangesAsync();
    }
}

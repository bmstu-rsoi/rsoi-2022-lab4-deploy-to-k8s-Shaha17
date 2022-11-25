using LibrarySystem.LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.LibrarySystem.Context;

public class ContextHelper
{
    public static async Task Seed(LibrariesContext context)
    {
        if (!context.Libraries.Any())
        {
            var lib = new Library()
            {
                Id = 1,
                LibraryUid = Guid.Parse("83575e12-7ce0-48ee-9931-51919ff3c9ee"),
                Name = "Библиотека имени 7 Непьющих",
                City = "Москва",
                Address = "2-я Бауманская ул., д.5, стр.1",
            };
            await context.Libraries.AddAsync(lib);
            await context.SaveChangesAsync();
        }

        if (!context.Books.Any())
        {
            var book = new Book()
            {
                Id = 1,
                BookUid = Guid.Parse("f7cdc58f-2caf-4b15-9727-f89dcc629b27"),
                Name = "Краткий курс C++ в 7 томах",
                Author = "Бьерн Страуструп",
                Genre = "Научная фантастика",
                Condition = "EXCELLENT"
            };

            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
        }


        if (!context.LibraryBooks.Any())
        {
            var book = await context.Books.FirstOrDefaultAsync(b => b.Id == 1);
            var library = await context.Libraries.FirstOrDefaultAsync(l => l.Id == 1);
            var libBook = new LibraryBook()
            {
                Book = book,
                Library = library,
                AvailableCount = 1,
            };
            await context.LibraryBooks.AddAsync(libBook);
        }

        await context.SaveChangesAsync();
    }
}
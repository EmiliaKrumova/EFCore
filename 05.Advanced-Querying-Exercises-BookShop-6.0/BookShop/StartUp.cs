namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            // DbInitializer.ResetDatabase(db);
            //string input =  Console.ReadLine();
            // Console.WriteLine(GetBooksByAgeRestriction(db,command));
            Console.WriteLine(RemoveBooks(db));
            // IncreasePrices(db);
        }
        public static int RemoveBooks(BookShopContext context)
        {
            int removed = 0;
            var booksToRemove = context.Books
                .Where(b => b.Copies<4200)
                .ToList();
            removed = booksToRemove.Count;
            context.RemoveRange(booksToRemove);
            context.SaveChanges();

            return removed;
        }
        //Remove all books, which have less than 4200 copies. Return an int - the number of books that were deleted from the database.

        public static void IncreasePrices(BookShopContext context)
        {
            var booksToIncreacePrice = context.Books
                .Where(b=>b.ReleaseDate.Value.Year<2010)               
                .ToList();
            foreach(var book in booksToIncreacePrice)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }
        //Increase the prices of all books released before 2010 by 5.


        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c=>new
                {
                    CategoryName = c.Name,
                    ResentBooks = c.CategoryBooks.Select(cb=>new
                    {
                       BookDate =  cb.Book.ReleaseDate,
                       BookTitle = cb.Book.Title,
                       BookYear = cb.Book.ReleaseDate.Value.Year

                    }).OrderByDescending(cb=>cb.BookDate)
                    .Take(3).ToList()

                }).OrderBy(c=>c.CategoryName).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach(var book in category.ResentBooks)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.BookYear})");
                }
            }
            return sb.ToString().TrimEnd();
                
        }
        //Get the most recent books by categories. The categories should be ordered by name alphabetically. Only take the top 3 most recent books from each category – ordered by release date (descending). Select and print the category name and for each book – its title and release year.


        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProfitByCategory = c.CategoryBooks.Sum(cb=>cb.Book.Price *cb.Book.Copies)
                })
                .OrderByDescending(c=>c.ProfitByCategory)
                .ThenBy(c=>c.CategoryName)
                .Select(c=> $"{c.CategoryName} ${c.ProfitByCategory:f2}")
                .ToList();

            var result = string.Join(Environment.NewLine, categories);
            return result;
        }
        //Return the total profit of all books by category.
        //Profit for a book can be calculated by multiplying its number of copies by the price per single book. Order the results by descending by total profit for a category and ascending by category name. Print the total profit formatted to the second digit.
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var copies = context.Authors
                .Select(a=> new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books.Sum(b=>b.Copies)
                })
                .OrderByDescending(a=>a.TotalCopies)
                .Select(c => $"{c.AuthorName} - {c.TotalCopies}")
                .ToList();

            var result = string.Join(Environment.NewLine,copies);

            return result;
        }
        //Return the total number of book copies for each author. Order the results descending by total book copies.
        //Return all results in a single string, each on a new line.

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b=>b.Title.Length>lengthCheck)
                .ToArray();
            int count = 0;
            count = books.Length;
            return count;

        }
        //Return the number of books, which have a title longer than the number given as an input
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(a => new
                {
                    BooksTitles = a.Books
                    .OrderBy(b => b.BookId)
                    .Select(b => new
                    {
                        Title = b.Title,
                    })
                    .ToList(),
                    AuthorFullName = a.FirstName + " " + a.LastName,
                }).ToList(); 
                StringBuilder sb = new StringBuilder();

            foreach(var author in authors)
            {
                foreach(var book in author.BooksTitles)
                {
                    sb.AppendLine($"{book.Title} ({author.AuthorFullName})");
                }
            }
            return sb.ToString().TrimEnd();

        }
        //Return all titles of books and their authors' names for books, which are written by authors whose last names start with the given string.
       // Return a single string with each title on a new row.Ignore casing.Order by BookId ascending
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b=>b.Title)
                .Select(b=>b.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);
            return result;
        }
        //Return the titles of the book, which contain a given string. Ignore casing.
       // Return all titles in a single string, each on a new row, ordered alphabetically.


        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new {
                    FullName = a.FirstName + " " + a.LastName
                }).OrderBy(a=>a.FullName)                
                .Select(a=>a.FullName)
                .ToList();
            var result = string.Join(Environment.NewLine, authors);
            return result;
        }
        //Return the full names of authors, whose first name ends with a given string.
        //Return all names in a single string, each on a new row, ordered alphabetically.

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime inputDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b=>b.ReleaseDate<inputDate)
                .OrderByDescending(b=>b.ReleaseDate)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    BookEdition = b.EditionType,
                    Price = b.Price
                }).ToList();

            var result = string.Join(Environment.NewLine, books.Select(b=> $"{b.BookTitle} - {b.BookEdition} - ${b.Price:f2}"));
            return result;

        }
        //Return the title, edition type and price of all books that are released before a given date. The date will be a string in the format "dd-MM-yyyy".
       // Return all of the rows in a single string, ordered by release date(descending).
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.ToLower().Split(' ',StringSplitOptions.RemoveEmptyEntries);


            var books = context.BooksCategories.
                Where(bc => categories.Contains(bc.Category.Name.ToLower()))
                .Select(bc=>bc.Book.Title)
                .OrderBy(title=>title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);
            return result;

        }
        //Return in a single string the titles of books by a given list of categories. The list of categories will be given in a single line separated by one or more spaces. Ignore casing. Order by title alphabetically.



        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b=> b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year!=year)
                .OrderBy(b=>b.BookId)
                .Select(b=>b.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);
            return result;

        }
        //Return in a single string with all titles of books that are NOT released in a given year. Order them by bookId ascending.

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b=>b.Price>40)
                .Select(b=> new
                {
                    BookTitle = b.Title,
                    BookPrice = b.Price,
                }).OrderByDescending(b=>b.BookPrice)
            .ToList();
           // var result = string.Join(Environment.NewLine,books.Select(book=>$"{book.BookTitle} - ${book.BookPrice:f2}"));
            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.BookTitle} - ${book.BookPrice:f2}");
            }
            return sb.ToString().TrimEnd();
          //  return result;

        }
        //Return in a single string all titles and prices of books with a price higher than 40, each on a new row in the format given below. Order them by price descending.

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b=>b.EditionType== EditionType.Gold && b.Copies<5000)
                .OrderBy(b=>b.BookId)
                .Select(b=>b.Title)
                .ToList();
            var result = string.Join(Environment.NewLine, books);
            return result;

        }
        //Return in a single string the titles of the golden edition books that have less than 5000 copies, each on a new line. Order them by BookId ascending.


        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            //Try to parse enum property AgeRestriction with  command,
            //true is ignore casing,
            //and if suceed returns ageRestriction
            //if not suceed - returns string.Empty

            if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestrictionAsString))
            {
                return string.Empty;
            }
            var bookTitles = context.Books
                .Where(b=>b.AgeRestriction==ageRestrictionAsString)                
                .OrderBy(b=>b.Title)
                .Select(b=>b.Title)
                .ToList();
            var result = string.Join(Environment.NewLine, bookTitles);
            return result;
        }
        //Return in a single string all book titles, each on a new line, that have an age restriction, equal to the given command. Order the titles alphabetically.
        //Read input from the console in your main method and call your method with the necessary arguments.Print the returned string to the console.Ignore the casing of the input.

    }
}



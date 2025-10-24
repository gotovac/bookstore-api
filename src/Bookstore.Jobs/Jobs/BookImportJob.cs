using Bookstore.Core.Interfaces.Services;
using Bookstore.Jobs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Jobs.Jobs
{
    public class BookImportJob : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookImportJob> _logger;

        public BookImportJob(IServiceScopeFactory scopeFactory, ILogger<BookImportJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

            var importBooks = BookImportService.GenerateBooks(100000, 200000);

            _logger.LogInformation("Quartz job starting import of {Count} books", importBooks.Count);
            await bookService.ImportBooksAsync(importBooks);
            _logger.LogInformation("Quartz job finished import");
        }
    }
}

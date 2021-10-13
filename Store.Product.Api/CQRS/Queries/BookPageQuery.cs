using MediatR;
using Store.Books.Domain;
using Store.Books.Infrastructure.Interfaces;
using Store.Product.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Product.Api.CQRS.Queries
{
    public class BookPageQuery : IRequest<IEnumerable<Book>>, ICacheableQuery
    {
        public int Page { get; set; }
        public int PerPage { get; set; }


        public bool BypassCache => false;

        public string CacheKey => $"books-page-{Page}";
        public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);

    }
    public class BookPageQueryHandler : IRequestHandler<BookPageQuery, IEnumerable<Book>>
    {
        private readonly IBookRepository _repository;

            public BookPageQueryHandler(IBookRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Book>> Handle(BookPageQuery request, CancellationToken cancellationToken)
        {
            //return await _repository.Get(p => p.Title == request.Title);
            return await Task.FromResult(_repository.GetPaged(request.Page, request.PerPage));
            
        }
    }
}

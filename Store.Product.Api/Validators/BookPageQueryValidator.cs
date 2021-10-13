using FluentValidation;
using Store.Product.Api.CQRS.Queries;

namespace Store.Product.Api.Validators
{
    public class BookPageQueryValidator : AbstractValidator<BookPageQuery>
    {
        public BookPageQueryValidator()
        {
            RuleFor(c => c.Page).NotEmpty();
            RuleFor(c => c.Page).GreaterThanOrEqualTo(0);
            RuleFor(c => c.PerPage).NotEmpty();
            RuleFor(c => c.PerPage).GreaterThan(0);
        }
    }
}

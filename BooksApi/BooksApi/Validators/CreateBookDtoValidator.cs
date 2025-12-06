using FluentValidation;
using BooksApi.DTOs;
using System.Data;

namespace BooksApi.Validators;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Book name cannot be empy!")
            .MinimumLength(3).WithMessage("Name cannot be so small (3 letter need)")
            .MaximumLength(100). WithMessage("Name too long");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price cannot be empy")
            .LessThan(10000).WithMessage("Price is too big");
    }


}
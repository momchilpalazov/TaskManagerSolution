

using FluentValidation;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Validators
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Заглавието е задължително.")
                .MaximumLength(100).WithMessage("Заглавието не може да е по-дълго от 100 символа.");

            RuleFor(x => x.Priority)
                .Must(p => new[] { "Low", "Medium", "High" }.Contains(p))
                .WithMessage("Позволени стойности за приоритет: Low, Medium, High.");

            RuleFor(x => x.Status)
                .Must(s => new[] { "Todo", "InProgress", "Done" }.Contains(s))
                .WithMessage("Невалиден статус.");
        }
    }
}

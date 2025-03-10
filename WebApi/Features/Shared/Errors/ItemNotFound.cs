using FluentResults;

namespace WebApi.Features.Shared.Errors
{
    public class ItemNotFound : Error
    {
        public ItemNotFound(string name) : base($"{name} not found")
        {
        }
    }
}
using MediatR;

namespace HoraDaBeleza.Application.Queries.SearchQuery;

public record SearchQuery(string Query, string Filter, int Page, int Limit) : IRequest<IEnumerable<object>>;
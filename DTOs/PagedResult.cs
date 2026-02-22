namespace VH_2ND_TASK.DTOs;

public record PagedResult<T>(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    IReadOnlyList<T> Items
);
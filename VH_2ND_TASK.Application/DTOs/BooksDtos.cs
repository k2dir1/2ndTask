namespace VH_2ND_TASK.Application.DTOs;



public record CreateBookRequest(string Title, string Author);
public record UpdateBookRequest(string Title, string Author);
public record PatchBookRequest(string? Title, string? Author);
public record BookResponse(int Id, string Title, string Author);
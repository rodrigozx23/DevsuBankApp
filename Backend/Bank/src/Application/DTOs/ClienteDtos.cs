namespace Bank.Application.DTOs;

public record ClienteCreateDto(string Nombre, string Genero, int Edad, string Identificacion, string Direccion, string Telefono, string Contrasena, bool Estado);
public record ClienteReadDto(int ClienteId, string Nombre, string Identificacion, string Genero, int Edad, string Direccion, string Telefono, string Contrasena, bool Estado);
public record ClienteUpdateDto( string Nombre, string? Genero, int? Edad, string Identificacion, string? Direccion, string? Telefono, string Contrasena, bool Estado);
public record ClienteListItemDto(int ClienteId, string Nombre, string Identificacion, bool Estado);

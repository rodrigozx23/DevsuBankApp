using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Api.Tests;

public class ClientesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public ClientesEndpointsTests(CustomWebApplicationFactory factory) => _factory = factory;
    private record ClienteCreatedDto(int personaId, string nombre, string identificacion, bool estado);

    [Fact]
    public async Task Post_crea_cliente_y_retorna_201()
    {
        var client = _factory.CreateClient();

        var dto = new
        {
            nombre = "María Test",
            genero = "F",
            edad = 25,
            identificacion = "CLI-T2",
            direccion = "Av Test",
            telefono = "123",
            contrasena = "abc",
            estado = true
        };

        var resp = await client.PostAsJsonAsync("/api/clientes", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await resp.Content.ReadFromJsonAsync<ClienteCreatedDto>();
        body.Should().NotBeNull();
        body!.nombre.Should().Be("María Test");
        body.personaId.Should().BeGreaterThan(0);
    }
}

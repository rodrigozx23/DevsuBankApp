using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Api.Tests;

public class MovimientosEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public MovimientosEndpointsTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Debito_mayor_al_saldo_devuelve_400_saldo_no_disponible()
    {
        var client = _factory.CreateClient();

        var dto = new { cuentaId = 1, tipoMovimiento = "debito", valor = 200m };

        var resp = await client.PostAsJsonAsync("/api/movimientos", dto);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var err = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        err.Should().NotBeNull();
        err!["detail"].ToString()!.ToLower().Should().Contain("saldo no disponible");
    }
}


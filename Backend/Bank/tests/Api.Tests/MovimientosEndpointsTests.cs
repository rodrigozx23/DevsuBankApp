using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Api.Tests;

public class MovimientosEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public MovimientosEndpointsTests(CustomWebApplicationFactory factory) => _factory = factory;

    private record ClienteCreatedDto(int personaId, string nombre, string identificacion, bool estado);
    private record CuentaCreatedDto(int cuentaId, int clienteId, string numeroCuenta, string tipoCuenta, decimal saldo, bool estado);

    [Fact]
    public async Task Debito_mayor_al_saldo_devuelve_400_saldo_no_disponible()
    {
        var client = _factory.CreateClient();

        var clienteDto = new {
            nombre = "María Test",
            genero = "F",
            edad = 25,
            identificacion = "CLI-T2",
            direccion = "Av Test",
            telefono = "123",
            contrasena = "abc",
            estado = true
        };
        var respCliente = await client.PostAsJsonAsync("/api/clientes", clienteDto);
        respCliente.StatusCode.Should().Be(HttpStatusCode.Created);
        var cliente = await respCliente.Content.ReadFromJsonAsync<ClienteCreatedDto>();

        var cuentaDto = new {
            clienteId = cliente.personaId,
            tipoCuenta = "Ahorros",
            numeroCuenta = "AUTO-TST-001",
            saldo = 100m,
            estado = true
        };
        var respCuenta = await client.PostAsJsonAsync("/api/cuentas", cuentaDto);
        respCuenta.StatusCode.Should().Be(HttpStatusCode.Created);
        var cuenta = await respCuenta.Content.ReadFromJsonAsync<CuentaCreatedDto>();

        var movDto = new { cuentaId = cuenta.cuentaId, tipoMovimiento = "debito", valor = 200m };
        var respMov = await client.PostAsJsonAsync("/api/movimientos", movDto);

        respMov.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var err = await respMov.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        err.Should().NotBeNull();
        err!["detail"].ToString()!.ToLower().Should().Contain("saldo no disponible");
    }
}
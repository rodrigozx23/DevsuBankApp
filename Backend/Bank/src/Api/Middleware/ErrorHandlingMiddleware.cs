using System.Net;
using Bank.Domain.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
namespace Bank.Api.Middleware;
public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        try {
            await next(ctx);
        }
        catch (DomainException ex)
        {
            await WriteProblem(ctx, (int)HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblem(ctx, (int)HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            await WriteProblem(ctx, (int)HttpStatusCode.InternalServerError, "Error interno del servidor.");
        }
    }

    private static async Task WriteProblem(HttpContext ctx, int status, string detail)
    {
        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = status;
        var problem = new
        {
            type = "about:blank",
            title = ReasonPhrases.GetReasonPhrase(status),
            status,
            detail,
            traceId = ctx.TraceIdentifier
        };
        await ctx.Response.WriteAsJsonAsync(problem);
    }
}
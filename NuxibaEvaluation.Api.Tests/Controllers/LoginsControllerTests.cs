using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuxibaEvaluation.Api.Controllers;
using NuxibaEvaluation.Api.Data;
using NuxibaEvaluation.Api.Dtos;
using NuxibaEvaluation.Api.Models;

namespace NuxibaEvaluation.Api.Tests.Controllers;

public class LoginsControllerTests
{
    private AppDbContext CreateDbContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAll_ReturnsAllLogins()
    {
        var context = CreateDbContext(nameof(GetAll_ReturnsAllLogins));

        context.Logins.Add(new Login
        {
            Id = 1,
            UserId = 70,
            Extension = 1,
            TipoMov = 1,
            Fecha = new DateTime(2024, 10, 5, 8, 0, 0)
        });

        context.Logins.Add(new Login
        {
            Id = 2,
            UserId = 70,
            Extension = 1,
            TipoMov = 0,
            Fecha = new DateTime(2024, 10, 5, 17, 0, 0)
        });

        await context.SaveChangesAsync();

        var controller = new LoginsController(context);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var records = Assert.IsAssignableFrom<IEnumerable<Login>>(okResult.Value);

        Assert.Equal(2, records.Count());
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        var context = CreateDbContext(nameof(Create_ReturnsBadRequest_WhenUserDoesNotExist));
        var controller = new LoginsController(context);

        var dto = new CreateLoginDto
        {
            UserId = 9999,
            Extension = 1,
            TipoMov = 1,
            Fecha = new DateTime(2024, 10, 5, 8, 0, 0)
        };

        var result = await controller.Create(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("The provided UserId does not exist in ccUsers.", badRequestResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenLogoutHasNoPreviousLogin()
    {
        var context = CreateDbContext(nameof(Create_ReturnsBadRequest_WhenLogoutHasNoPreviousLogin));

        context.Users.Add(new User
        {
            UserId = 70,
            Login = "adriAgent"
        });

        await context.SaveChangesAsync();

        var controller = new LoginsController(context);

        var dto = new CreateLoginDto
        {
            UserId = 70,
            Extension = 1,
            TipoMov = 0,
            Fecha = new DateTime(2024, 10, 5, 17, 0, 0)
        };

        var result = await controller.Create(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Cannot register a logout without a previous open login.", badRequestResult.Value);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuxibaEvaluation.Api.Data;
using NuxibaEvaluation.Api.Dtos;
using NuxibaEvaluation.Api.Models;

namespace NuxibaEvaluation.Api.Controllers;

[ApiController]
[Route("logins")]
public class LoginsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Login>>> GetAll()
    {
        var records = await _context.Logins
            .OrderBy(x => x.UserId)
            .ThenBy(x => x.Fecha)
            .ThenBy(x => x.Id)
            .ToListAsync();

        return Ok(records);
    }

    [HttpPost]
    public async Task<ActionResult<Login>> Create(CreateLoginDto dto)
    {
        var validationError = await ValidateLoginMovement(dto.UserId, dto.TipoMov, dto.Fecha, null);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        var login = new Login
        {
            UserId = dto.UserId,
            Extension = dto.Extension,
            TipoMov = dto.TipoMov,
            Fecha = dto.Fecha
        };

        _context.Logins.Add(login);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = login.Id }, login);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Login>> Update(int id, UpdateLoginDto dto)
    {
        var existingRecord = await _context.Logins.FindAsync(id);
        if (existingRecord == null)
        {
            return NotFound();
        }

        var validationError = await ValidateLoginMovement(dto.UserId, dto.TipoMov, dto.Fecha, id);
        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        existingRecord.UserId = dto.UserId;
        existingRecord.Extension = dto.Extension;
        existingRecord.TipoMov = dto.TipoMov;
        existingRecord.Fecha = dto.Fecha;

        await _context.SaveChangesAsync();

        return Ok(existingRecord);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingRecord = await _context.Logins.FindAsync(id);
        if (existingRecord == null)
        {
            return NotFound();
        }

        _context.Logins.Remove(existingRecord);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<string?> ValidateLoginMovement(int userId, int tipoMov, DateTime fecha, int? currentId)
    {
        if (tipoMov is not (0 or 1))
        {
            return "TipoMov must be 1 for login or 0 for logout.";
        }

        if (fecha == default)
        {
            return "Fecha is required and must be valid.";
        }

        var userExists = await _context.Users.AnyAsync(x => x.UserId == userId);
        if (!userExists)
        {
            return "The provided UserId does not exist in ccUsers.";
        }

        var userMovements = await _context.Logins
            .Where(x => x.UserId == userId && (!currentId.HasValue || x.Id != currentId.Value))
            .OrderBy(x => x.Fecha)
            .ThenBy(x => x.Id)
            .ToListAsync();

        if (!currentId.HasValue)
        {
            var lastMovement = userMovements.LastOrDefault();

            if (lastMovement != null && fecha < lastMovement.Fecha)
            {
                return "Fecha cannot be earlier than the last movement for this user.";
            }

            if (tipoMov == 1)
            {
                if (lastMovement != null && lastMovement.TipoMov == 1)
                {
                    return "Cannot register a login when there is already an open session.";
                }
            }
            else
            {
                if (lastMovement == null || lastMovement.TipoMov != 1)
                {
                    return "Cannot register a logout without a previous open login.";
                }
            }

            return null;
        }

        var candidateMovements = userMovements
            .Append(new Login
            {
                Id = currentId.Value,
                UserId = userId,
                Extension = 0,
                TipoMov = tipoMov,
                Fecha = fecha
            })
            .OrderBy(x => x.Fecha)
            .ThenBy(x => x.Id)
            .ToList();

        int balance = 0;

        foreach (var movement in candidateMovements)
        {
            if (movement.TipoMov == 1)
            {
                balance++;
            }
            else
            {
                balance--;
            }

            if (balance < 0)
            {
                return "The movement sequence would become invalid. A logout cannot exist without a previous login.";
            }

            if (balance > 1)
            {
                return "The movement sequence would become invalid. A user cannot have more than one open session.";
            }
        }

        return null;
    }
}
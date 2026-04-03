using System.Text;
using Microsoft.EntityFrameworkCore;
using NuxibaEvaluation.Api.Data;
using NuxibaEvaluation.Api.Models;

namespace NuxibaEvaluation.Api.Services;

public class CsvReportService
{
    private readonly AppDbContext _context;

    public CsvReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateWorkedHoursCsvAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        var areas = await _context.Areas
            .AsNoTracking()
            .ToListAsync();

        var logins = await _context.Logins
            .AsNoTracking()
            .OrderBy(x => x.UserId)
            .ThenBy(x => x.Fecha)
            .ThenBy(x => x.Id)
            .ToListAsync();

        var uniqueAreas = areas
            .GroupBy(x => x.IDArea)
            .Select(g => g.First())
            .ToDictionary(x => x.IDArea, x => x.AreaName ?? "Unknown");

        var totalSecondsByUser = CalculateWorkedSeconds(logins);

        var sb = new StringBuilder();
        sb.AppendLine("Login,FullName,Area,TotalHoursWorked");

        foreach (var user in users.OrderBy(x => x.UserId))
        {
            totalSecondsByUser.TryGetValue(user.UserId, out var totalSeconds);

            var fullName = BuildFullName(user);
            var areaName = GetAreaName(user.IDArea, uniqueAreas);
            var totalHoursWorked = Math.Round(totalSeconds / 3600.0, 2);

            sb.AppendLine(
                $"{EscapeCsv(user.Login)},{EscapeCsv(fullName)},{EscapeCsv(areaName)},{totalHoursWorked}"
            );
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static Dictionary<int, long> CalculateWorkedSeconds(List<Login> logins)
    {
        var result = new Dictionary<int, long>();

        foreach (var userGroup in logins.GroupBy(x => x.UserId))
        {
            var movements = userGroup
                .OrderBy(x => x.Fecha)
                .ThenBy(x => x.Id)
                .ToList();

            for (int i = 0; i < movements.Count - 1; i++)
            {
                var current = movements[i];
                var next = movements[i + 1];

                if (current.TipoMov == 1 && next.TipoMov == 0 && next.Fecha > current.Fecha)
                {
                    var sessionSeconds = (long)(next.Fecha - current.Fecha).TotalSeconds;

                    if (!result.ContainsKey(userGroup.Key))
                    {
                        result[userGroup.Key] = 0;
                    }

                    result[userGroup.Key] += sessionSeconds;
                }
            }
        }

        return result;
    }

    private static string BuildFullName(User user)
    {
        return string.Join(" ", new[]
        {
            user.Nombres?.Trim(),
            user.ApellidoPaterno?.Trim(),
            user.ApellidoMaterno?.Trim()
        }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static string GetAreaName(int? areaId, Dictionary<int, string> areas)
    {
        if (!areaId.HasValue)
        {
            return "Unknown";
        }

        return areas.TryGetValue(areaId.Value, out var areaName)
            ? areaName
            : "Unknown";
    }

    private static string EscapeCsv(string? value)
    {
        value ??= string.Empty;
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
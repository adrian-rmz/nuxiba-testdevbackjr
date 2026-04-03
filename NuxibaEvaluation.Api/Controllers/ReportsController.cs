using Microsoft.AspNetCore.Mvc;
using NuxibaEvaluation.Api.Services;

namespace NuxibaEvaluation.Api.Controllers;

[ApiController]
[Route("reports")]
public class ReportsController : ControllerBase
{
    private readonly CsvReportService _csvReportService;

    public ReportsController(CsvReportService csvReportService)
    {
        _csvReportService = csvReportService;
    }

    [HttpGet("worked-hours-csv")]
    public async Task<IActionResult> DownloadWorkedHoursCsv()
    {
        var fileBytes = await _csvReportService.GenerateWorkedHoursCsvAsync();

        return File(fileBytes, "text/csv", "worked-hours-report.csv");
    }
}
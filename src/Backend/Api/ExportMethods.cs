using System.Globalization;
using System.Net.Mime;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Backend.Api;

internal static class ExportMethods
{
    private const string XlsMediaType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static IActionResult ToCsv(IEnumerable<PlayerResponse> players)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter    = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        // Write header row
        csvWriter.WriteField("Nickname");
        csvWriter.WriteField("ID");
        csvWriter.WriteField("Rating");
        csvWriter.WriteField("Date");
        csvWriter.NextRecord();

        foreach (PlayerResponse player in players)
        {
            foreach (RatingHistory ratingsHistory in player.RatingsHistories)
            {
                foreach (RatingPerDate ratingPerDate in ratingsHistory.RatingPerDates)
                {
                    csvWriter.WriteField(player.Nickname);
                    csvWriter.WriteField(player.Id);
                    csvWriter.WriteField(ratingPerDate.Rating);
                    csvWriter.WriteField(ratingPerDate.ActualityDate.ToString("yyyy-MM-dd"));
                    csvWriter.NextRecord();
                }
            }
        }

        streamWriter.Flush();
        memoryStream.Position = 0;

        var fileContentResult = new FileContentResult(memoryStream.ToArray(),
                                                      MediaTypeNames.Text.Csv)
                                {
                                    FileDownloadName = "players.csv"
                                };

        return fileContentResult;
    }

    public static IActionResult ToExcel(IEnumerable<PlayerResponse> players)
    {
        using var       package   = new ExcelPackage();
        ExcelWorksheet? worksheet = package.Workbook.Worksheets.Add("Players");

        worksheet.Cells[1, 1].Value = "Nickname";
        worksheet.Cells[1, 2].Value = "ID";
        worksheet.Cells[1, 3].Value = "Rating";
        worksheet.Cells[1, 4].Value = "Date";

        var row = 2;
        foreach (PlayerResponse player in players)
        {
            worksheet.Cells[row, 1].Value = player.Nickname;
            worksheet.Cells[row, 2].Value = player.Id;

            int ratingsRow = row;
            foreach (RatingHistory ratingsHistory in player.RatingsHistories)
            {
                foreach (RatingPerDate ratingPerDate in ratingsHistory.RatingPerDates)
                {
                    worksheet.Cells[ratingsRow, 3].Value = ratingPerDate.Rating;
                    worksheet.Cells[ratingsRow, 4].Value = ratingPerDate.ActualityDate;
                    ratingsRow++;
                }
            }

            row = ratingsRow;
        }

        var memoryStream = new MemoryStream();
        package.SaveAs(memoryStream);
        memoryStream.Position = 0;

        var fileContentResult = new FileContentResult(memoryStream.ToArray(),
                                                      XlsMediaType)
                                {
                                    FileDownloadName = "players.xlsx"
                                };

        return fileContentResult;
    }
}

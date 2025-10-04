using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;



// DIRECTORIES (FOLDERS)
var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesSummaryDir = Path.Combine(currentDirectory, "salesSummary");

var salesFiles = FindFiles(storesDirectory);

// CALCULATE TOTAL SALES
var salesTotal = CalculateSalesTotal(salesFiles);

// SAVE TOTAL SALES
File.AppendAllText(Path.Combine(salesTotalDir, "total.txt"), $"{salesTotal:F2}{Environment.NewLine}");

// GENERATE SALES SUMMARY REPORT
GenerateSalesSummary(salesFiles, salesTotal, salesSummaryDir);



// -------------------- METHODS --------------------

static void GenerateSalesSummary(IEnumerable<string> salesFiles, double salesTotal, string outputDir)
{
    Directory.CreateDirectory(outputDir);
    string salesSummaryFile = Path.Combine(outputDir, "sales_summary.txt");

    var sb = new StringBuilder();
    sb.AppendLine("Sales Summary");
    sb.AppendLine("----------------------------");
    sb.AppendLine($" Total Sales: {salesTotal:C2}{Environment.NewLine}");
    sb.AppendLine(" Details:");

    foreach (var file in salesFiles)
    {

        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        sb.AppendLine($"  {Path.GetFileName(Path.GetDirectoryName(file))} {Path.GetFileName(file)}: {data?.Total:C2}");
    }

    File.WriteAllText(salesSummaryFile, sb.ToString());
}

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "sales.json", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        // The file name will contain the full path, so only check the end of it
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}


double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

record SalesData (double Total);




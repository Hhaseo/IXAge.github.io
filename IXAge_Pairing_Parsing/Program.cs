// See https://aka.ms/new-console-template for more information
using IXAge_IHM.Shared.Pairing;
using IXAge_Pairing_Parsing;
using Newtonsoft.Json;




var acceptedFileTypes = new List<string>
{
    ".xlsx"
};

var files = Directory.GetFiles("./Input/");
List<string> supportedFiles = files.Where(
    file => acceptedFileTypes.Contains(Path.GetExtension(file))
).ToList();

foreach (var f in supportedFiles)
{
    var id = f.LastIndexOf('/');
    var id2 = f.LastIndexOf('.');
    var folderName = "./Data_" + f.Substring(id + 1, id2 - 1 - id) + "/";
    if (!System.IO.Directory.Exists(folderName))
        System.IO.Directory.CreateDirectory(folderName);
    var excelFile = new LinqToExcel.ExcelQueryFactory(f);

    foreach (var sheet in excelFile.GetWorksheetNames())
    {
        ParsWorsheet.Pars(excelFile, sheet, folderName);
    }
}


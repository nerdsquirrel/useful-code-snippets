using OfficeOpenXml;

public void GenerateExcelFromData<T>(IEnumerable<T> data)
{
    ExcelPackage excel = new ExcelPackage();
    var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
    workSheet.Cells[1, 1].LoadFromCollection(data, true);

    using (var stream = new MemoryStream())
    {
        excel.SaveAs(stream);
    }
}
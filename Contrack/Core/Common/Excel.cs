using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class Excel
    {
        public int AppendHeadTableCount = 1;
        private static int LastColumnNo(ISheet sheet)
        {
            int lastFilledColumn = -1;
            try
            {
                for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    int cellCount = row.LastCellNum;
                    if (cellCount >= lastFilledColumn)
                        lastFilledColumn = cellCount;

                    //for (int j = row.LastCellNum - 1; j >= row.FirstCellNum; j--)
                    //{
                    //    ICell cell = row.GetCell(j);
                    //    if (cell != null && cell.CellType != CellType.Blank)
                    //    {
                    //        if (j > lastFilledColumn)
                    //            lastFilledColumn = j;
                    //        break; // No need to check earlier cells in this row
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            { }

            return lastFilledColumn;
        }
        public static DataTable ExcelToDataTable(string filePath)
        {
            DataTable dt = new DataTable();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                string Ext = Path.GetExtension(filePath);
                IWorkbook workbook;
                if (Ext == ".xlsx")
                    workbook = new XSSFWorkbook(fs); // for .xlsx
                else
                    workbook = new HSSFWorkbook(fs); // for .xls

                ISheet sheet = workbook.GetSheetAt(0);     // get first sheet

                IRow headerRow = sheet.GetRow(0);
                int cellCount = LastColumnNo(sheet);
                if (cellCount == -1)
                    cellCount = headerRow.LastCellNum;

                // Add columns to DataTable
                for (int i = 0; i < cellCount; i++)
                {
                    var cellValue = headerRow.GetCell(i)?.ToString();
                    dt.Columns.Add(cellValue ?? $"Column{i + 1}");
                }

                // Add rows to DataTable
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    DataRow dataRow = dt.NewRow();

                    for (int j = 0; j < cellCount; j++)
                    {
                        dataRow[j] = row.GetCell(j)?.ToString();
                    }

                    dt.Rows.Add(dataRow);
                }
            }

            return dt;
        }

        public void DataTable_To_Excel(DataSet pDatosSet, string pFilePath, DataTable additional = null)
        {
            try
            {
                int AppendHeadTableCountTemp = 0;
                if (pDatosSet != null && pDatosSet.Tables.Count > 0)
                {
                    IWorkbook workbook = null;
                    ISheet worksheet = null;

                    using (FileStream stream = new FileStream(pFilePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        string Ext = System.IO.Path.GetExtension(pFilePath);
                        switch (Ext.ToLower())
                        {
                            case ".xls":
                                HSSFWorkbook workbookH = new HSSFWorkbook();
                                DocumentSummaryInformation dsi = new DocumentSummaryInformation();
                                dsi.Company = "CPG"; dsi.Manager = "CPG";
                                workbookH.DocumentSummaryInformation = dsi;
                                workbook = workbookH;
                                break;

                            case ".xlsx": workbook = new XSSFWorkbook(); break;
                        }

                        HSSFCellStyle style1 = (HSSFCellStyle)workbook.CreateCellStyle();
                        // cell background
                        style1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                        style1.BorderLeft = BorderStyle.Thin;
                        style1.BorderRight = BorderStyle.Thin;
                        style1.BorderTop = BorderStyle.Thin;
                        style1.BorderBottom = BorderStyle.Thin;
                        style1.FillPattern = FillPattern.SolidForeground;

                        // font color
                        HSSFFont font1 = (HSSFFont)workbook.CreateFont();
                        font1.Color = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                        style1.SetFont(font1);

                        HSSFCellStyle style2 = (HSSFCellStyle)workbook.CreateCellStyle();
                        // cell background
                        style2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                        style2.BorderLeft = BorderStyle.Thin;
                        style2.BorderRight = BorderStyle.Thin;
                        style2.BorderTop = BorderStyle.Thin;
                        style2.BorderBottom = BorderStyle.Thin;
                        style2.FillPattern = FillPattern.SolidForeground;

                        // font color
                        HSSFFont font2 = (HSSFFont)workbook.CreateFont();
                        font2.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                        style2.SetFont(font2);

                        int iRow = 0;
                        foreach (DataTable pDatos in pDatosSet.Tables)
                        {
                            if (AppendHeadTableCountTemp >= AppendHeadTableCount)
                            {
                                additional = null;
                            }
                            AppendHeadTableCountTemp++;

                            worksheet = workbook.CreateSheet(pDatos.TableName);
                            iRow = 0;
                            if (additional != null)
                            {
                                iRow = 0;
                                foreach (DataRow row in additional.Rows)
                                {
                                    IRow fila = worksheet.CreateRow(iRow);
                                    for (int iCol = 0; iCol < additional.Columns.Count; iCol++)
                                    {
                                        ICell cell = fila.CreateCell(iCol, CellType.String);
                                        cell.SetCellValue(row[iCol].ToString());
                                    }
                                    iRow++;
                                }
                            }

                            //iRow = 0;
                            if (pDatos.Columns.Count > 0)
                            {
                                int iCol = 0;
                                IRow fila = worksheet.CreateRow(iRow);

                                HSSFFont hFont = (HSSFFont)workbook.CreateFont();
                                hFont.IsBold = true;
                                hFont.Color = HSSFColor.White.Index;

                                HSSFCellStyle hStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                                hStyle.SetFont(hFont);
                                hStyle.FillPattern = FillPattern.SolidForeground;

                                foreach (DataColumn columna in pDatos.Columns)
                                {
                                    ICell cell = fila.CreateCell(iCol, CellType.String);
                                    cell.SetCellValue(columna.ColumnName);
                                    cell.CellStyle = hStyle;
                                    iCol++;
                                }
                                iRow++;
                            }

                            //FORMATOS PARA CIERTOS TIPOS DE DATOS
                            ICellStyle _doubleCellStyle = workbook.CreateCellStyle();
                            _doubleCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.###");

                            ICellStyle _intCellStyle = workbook.CreateCellStyle();
                            _intCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0");

                            ICellStyle _boolCellStyle = workbook.CreateCellStyle();
                            _boolCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("BOOLEAN");

                            ICellStyle _dateCellStyle = workbook.CreateCellStyle();
                            _dateCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("MM-dd-yyyy");

                            ICellStyle _dateTimeCellStyle = workbook.CreateCellStyle();
                            _dateTimeCellStyle.DataFormat = workbook.CreateDataFormat().GetFormat("MM-dd-yyyy HH:mm:ss");
                            int maxcol = 0;
                            //AHORA CREAR UNA FILA POR CADA REGISTRO DE LA TABLA
                            foreach (DataRow row in pDatos.Rows)
                            {
                                IRow fila = worksheet.CreateRow(iRow);
                                int iCol = 0;
                                foreach (DataColumn column in pDatos.Columns)
                                {
                                    ICell cell = null;
                                    object cellValue = row[iCol];

                                    switch (column.DataType.ToString())
                                    {
                                        case "System.Boolean":
                                            if (cellValue != DBNull.Value)
                                            {
                                                cell = fila.CreateCell(iCol, CellType.Boolean);

                                                if (Convert.ToBoolean(cellValue)) { cell.SetCellFormula("TRUE()"); }
                                                else { cell.SetCellFormula("FALSE()"); }

                                                cell.CellStyle = _boolCellStyle;
                                            }
                                            break;

                                        case "System.String":
                                            if (cellValue != DBNull.Value)
                                            {
                                                cell = fila.CreateCell(iCol, CellType.String);
                                                cell.SetCellValue(Convert.ToString(cellValue));
                                                if (Convert.ToString(cellValue).Trim() == "0.00" || Convert.ToString(cellValue) == "$0.00")
                                                    cell.CellStyle = style1;
                                                else
                                                    cell.CellStyle = style2;
                                            }
                                            break;

                                        case "System.Int32":
                                            if (cellValue != DBNull.Value)
                                            {
                                                cell = fila.CreateCell(iCol, CellType.Numeric);
                                                cell.SetCellValue(Convert.ToInt32(cellValue));
                                                cell.CellStyle = _intCellStyle;
                                            }
                                            break;
                                        case "System.Int64":
                                            if (cellValue != DBNull.Value)
                                            {
                                                cell = fila.CreateCell(iCol, CellType.Numeric);
                                                cell.SetCellValue(Convert.ToInt64(cellValue));
                                                cell.CellStyle = _intCellStyle;
                                            }
                                            break;
                                        case "System.Decimal":
                                            if (cellValue != DBNull.Value)
                                            {
                                                cell = fila.CreateCell(iCol, CellType.Numeric);
                                                cell.SetCellValue(Convert.ToDouble(cellValue));
                                                cell.CellStyle = _doubleCellStyle;
                                            }
                                            break;
                                        case "System.Double":
                                            if (cellValue != DBNull.Value)
                                            {
                                                cell = fila.CreateCell(iCol, CellType.Numeric);
                                                cell.SetCellValue(Convert.ToDouble(cellValue));
                                                cell.CellStyle = _doubleCellStyle;
                                            }
                                            break;

                                        case "System.DateTime":
                                            if (cellValue != DBNull.Value)
                                            {
                                                cell = fila.CreateCell(iCol, CellType.Numeric);
                                                cell.SetCellValue(Convert.ToDateTime(cellValue));

                                                //Si No tiene valor de Hora, usar formato MM-dd-yyyy
                                                DateTime cDate = Convert.ToDateTime(cellValue);
                                                if (cDate != null && cDate.Hour > 0) { cell.CellStyle = _dateTimeCellStyle; }
                                                else { cell.CellStyle = _dateCellStyle; }
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    iCol++;
                                }
                                if (maxcol == 0)
                                {
                                    for (int i = 0; i < iCol; i++)
                                        worksheet.AutoSizeColumn(i);
                                    maxcol = iCol;
                                }
                                iRow++;
                            }
                            //var cra = new NPOI.SS.Util.CellRangeAddress(1, 1, 1, 4);

                            //worksheet.AddMergedRegion(cra);

                        }

                        workbook.Write(stream);
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLog("Excel Download :" + ex.ToString());
            }
        }

    }
}
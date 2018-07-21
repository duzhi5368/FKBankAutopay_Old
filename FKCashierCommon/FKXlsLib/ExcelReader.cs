using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace FKXlsLib
{
    /*
    * Excel读取类，该类负责打开一个excel进程，进行读写活动，当前该类只支持
    * 单独实例，不支持同时刻多文件读写
    */
    public class ExcelReader
    {
        static Excel.Application xlApp;
        Excel.Workbook xlWorkbook;
        Excel._Worksheet xlWorksheet;
        Excel.Range xlRange;
        int iCurSheet = -1;
        protected ExcelReader() { }
        public static ExcelReader CreateReader(string strFile)
        {
            if (File.Exists(strFile))
            {
                if (null == xlApp)
                    xlApp = new Excel.Application();

                ExcelReader reader = new ExcelReader();
                if (!reader.OpenFile(strFile))
                    return null;

                return reader;
            }

           
            return null;
        }


        protected bool OpenFile(string strFile)
        {
            if (null == xlApp)
                return false;
            try
            {
                try
                {
                    xlWorkbook = xlApp.Workbooks.Open(strFile, null, true);
                }
                catch 
                {
                    return false;
                }

                if (xlWorkbook.Sheets.Count <= 0)
                {
                    return false;
                }
            }
            catch 
            {
                return false;
            }
            return true;
        }

        //
        //excel file 数据索引不是0，是由1开始的。
        //
        public bool Read(int isheet, int line, ref List<Object> datas)
        {
            if (null == xlWorkbook)
                return false;
            if (isheet < 0 || isheet > xlWorkbook.Sheets.Count || line < 0)
                return false;

            if (iCurSheet != isheet)
            {
                if (null != xlWorksheet)
                    Marshal.ReleaseComObject(xlWorksheet);
                xlWorksheet = null;

                if (null != xlRange)
                    Marshal.ReleaseComObject(xlRange);
                xlRange = null;
            }

            if (xlWorksheet == null)
            {
                iCurSheet = isheet;
                xlWorksheet = xlWorkbook.Sheets[iCurSheet];
                xlRange = xlWorksheet.UsedRange;
            }

            if (null != xlRange)
            {
                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                if (line > rowCount)
                    return false;

                for (int j = 1; j <= colCount; j++)
                {
                    //write the value to the console
                    if (xlRange.Cells[line, j] != null && xlRange.Cells[line, j].Value2 != null)
                        datas.Add(xlRange.Cells[line, j].Value2);
                }

            }

            return true;
        }
        public bool Close()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            if (null != xlRange)
                Marshal.ReleaseComObject(xlRange);

            if (null != xlWorksheet)
                Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            if (null != xlWorkbook)
            {
                xlWorkbook.Close();
                Marshal.ReleaseComObject(xlWorkbook);
            }
            //quit and release
            if (null != xlApp)
            {
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
                xlApp = null;
            }

            System.GC.Collect();
            return true;
        }
    }
}

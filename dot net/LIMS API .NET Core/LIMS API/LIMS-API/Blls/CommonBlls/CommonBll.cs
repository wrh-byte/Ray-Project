using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Cells;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Layout;
using Aspose.Words.Reporting;
using Aspose.Words.Tables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nancy;
using Cell = Aspose.Words.Tables.Cell;
using Document = Aspose.Words.Document;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Row = Aspose.Words.Tables.Row;
using RowCollection = Aspose.Words.Tables.RowCollection;
using SaveFormat = Aspose.Words.SaveFormat;
using Table = Aspose.Words.Tables.Table;

namespace LIMS_API.Blls.CommonBlls
{
    public class CommonBll
    {
        /// <summary>
        /// List转换成DataTable 列顺序根据创建Model顺序
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(IList list)
        {
            DataTable result = null;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    result = new DataTable();
                    PropertyInfo[] propertys = list[0].GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        //获取类型
                        Type colType = pi.PropertyType;
                        //当类型为Nullable<>时
                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        result.Columns.Add(pi.Name, colType);
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        ArrayList tempList = new ArrayList();
                        foreach (PropertyInfo pi in propertys)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        object[] array = tempList.ToArray();
                        result.LoadDataRow(array, true);
                    }
                }
                return result;
            }
            return result;
        }



        /// <summary>
        /// 根据datatable创建表格(末尾添加)
        /// </summary>
        /// <param name="table"></param>
        /// <param name="list"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool CreateTable(Table table, IList list, Document doc)
        {
            try
            {
                DataTable dataTable = ListToDataTable(list);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    //Row
                    Row LastRow = (Row)table.LastRow.Clone(true);
                    foreach (Cell cell in LastRow.Cells)
                    {
                        cell.RemoveAllChildren();
                    }
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {

                        var cl = LastRow.Cells[j];
                        Paragraph p = new Paragraph(doc);
                        p.AppendChild(new Run(doc, dataTable.Rows[i][j].ToString()));
                        cl.AppendChild(p);
                    }
                    table.AppendChild(LastRow);
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 在表格中插入行
        /// </summary>
        /// <param name="table"></param>
        /// <param name="cellAlignment"></param>
        /// <param name="FontName"></param>
        /// <param name="startRow"></param>
        /// <param name="list"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool InsertRowToTable(Table table, ParagraphAlignment cellAlignment, string FontName, int startRow, IList list, Document doc)
        {
            try
            {
                LogHelper.Info("start insert row");
                int positionRow = startRow + 1;
                DataTable dataTable = ListToDataTable(list);
                if (dataTable == null)
                {
                    return false;
                }
                Row cloneRow = (Row)table.Rows[startRow].Clone(true);
                foreach (Cell cell in cloneRow.Cells)
                {
                    cell.RemoveAllChildren();
                }
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Row insertRow = (Row)cloneRow.Clone(true);
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        LogHelper.Info("data" + dataTable.Rows[i][j].ToString());
                        var cl = insertRow.Cells[j];
                        Paragraph p = new Paragraph(doc);
                        p.ParagraphFormat.Alignment = cellAlignment;
                        p.ParagraphFormat.Style.Font.Name = FontName;
                        p.AppendChild(new Run(doc, dataTable.Rows[i][j].ToString()));
                        cl.AppendChild(p);
                    }
                    table.Rows.Insert(positionRow, insertRow);
                    positionRow++;
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 填充Bookmark 根据model中与bookmark中名称相同的部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="model"></param>
        public static bool FillBookmark<T>(Aspose.Words.Document doc, T model)
        {
            try
            {
                //Get All bookmark into bookmarkName List
                List<string> bookmarkList = new List<string>();
                BookmarkCollection bookmarks = doc.Range.Bookmarks;
                foreach (var bm in bookmarks)
                {
                    bookmarkList.Add(bm.Name);
                }

                PropertyInfo[] info = model.GetType().GetProperties();
                foreach (var item in info)
                {
                    String type = item.PropertyType.FullName;
                    if (type == "System.String" && bookmarkList.Contains(item.Name))
                    {

                        Bookmark bookmark = doc.Range.Bookmarks[item.Name];
                        if (item.GetValue(model) == null)
                        {
                            bookmark.Text = "";
                        }
                        else
                        {
                            bookmark.Text = item.GetValue(model).ToString();
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 创建文件保存路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateSaveFilePath(string fileName, string filetype)
        {
            string now = DateTime.Now.ToString("yyyyMMddhhmmss");
            string responsePath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath +
                                   "/ResponseFile";
            if (Directory.Exists(responsePath) == false)
            {
                Directory.CreateDirectory(responsePath);
            }
            string savePath = responsePath + "/" + fileName + "_" + now + "." + filetype;
            return savePath;
        }

        /// <summary>
        /// 创建文件保存路径（多租户情况重载1）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CreateSaveFilePath(string fileName, string filetype, string orgName, string orgId)
        {
            string now = DateTime.Now.ToString("yyyyMMddhhmmss");
            string responsePath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath +
                                  "/ResponseFile/" + orgName + "_" + orgId;
            if (Directory.Exists(responsePath) == false)
            {
                Directory.CreateDirectory(responsePath);
            }
            string savePath = responsePath + "/" + fileName + "_" + now + "." + filetype;
            return savePath;
        }

        /// <summary>
        /// CreateDraftReportSavePath
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public static string CreateDraftReportSavePath(string fileName, string filetype)
        {
            string now = DateTime.Now.ToString("yyyyMMddhhmmss");
            string responsePath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath +
                                  "/DraftReports";
            if (Directory.Exists(responsePath) == false)
            {
                Directory.CreateDirectory(responsePath);
            }
            string savePath = responsePath + "/" + fileName + "_" + now + "." + filetype;
            return savePath;
        }

        /// <summary>
        /// CreateDraftReportSavePath(Overload with orgId and name)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public static string CreateDraftReportSavePath(string fileName, string filetype, string orgName, string orgId)
        {
            string now = DateTime.Now.ToString("yyyyMMddhhmmss");
            string responsePath = ServiceProvider.Provider.GetRequiredService<IHostEnvironment>().ContentRootPath +
                                  "/DraftReports/" + orgName + "_" + orgId;
            if (Directory.Exists(responsePath) == false)
            {
                Directory.CreateDirectory(responsePath);
            }
            string savePath = responsePath + "/" + fileName + "_" + now + "." + filetype;
            return savePath;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string HMACSHA1Text(string key, string text)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);

            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            var enText = new StringBuilder();
            foreach (byte iByte in hashBytes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
        }

        /// <summary>
        /// 创建checkbox
        /// </summary>
        /// <param name="bookmarkStrings"></param>
        public static void CreateCheckBox(Document doc, string[] bookmarkStrings)
        {
            DocumentBuilder documentBuilder = new DocumentBuilder(doc);
            for (int i = 0; i < bookmarkStrings.Length; i++)
            {
                documentBuilder.MoveToBookmark(bookmarkStrings[i]);
                documentBuilder.InsertCheckBox(bookmarkStrings[i], false, 0);
            }
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        public static void MergeCells(Cell startCell, Cell endCell)
        {
            Table parentTable = startCell.ParentRow.ParentTable;

            // Find the row and cell indices for the start and end cell.
            Point startCellPos = new Point(startCell.ParentRow.IndexOf(startCell), parentTable.IndexOf(startCell.ParentRow));
            Point endCellPos = new Point(endCell.ParentRow.IndexOf(endCell), parentTable.IndexOf(endCell.ParentRow));
            // Create the range of cells to be merged based off these indices. Inverse each index if the end cell if before the start cell. 
            Rectangle mergeRange = new Rectangle(System.Math.Min(startCellPos.X, endCellPos.X), System.Math.Min(startCellPos.Y, endCellPos.Y),
                System.Math.Abs(endCellPos.X - startCellPos.X) + 1, System.Math.Abs(endCellPos.Y - startCellPos.Y) + 1);

            foreach (Row row in parentTable.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    Point currentPos = new Point(row.IndexOf(cell), parentTable.IndexOf(row));

                    // Check if the current cell is inside our merge range then merge it.
                    if (mergeRange.Contains(currentPos))
                    {
                        if (currentPos.X == mergeRange.X)
                            cell.CellFormat.HorizontalMerge = CellMerge.First;
                        else
                            cell.CellFormat.HorizontalMerge = CellMerge.Previous;

                        if (currentPos.Y == mergeRange.Y)
                            cell.CellFormat.VerticalMerge = CellMerge.First;
                        else
                            cell.CellFormat.VerticalMerge = CellMerge.Previous;
                    }
                }
            }
        }

        /// <summary>
        /// MergeSameValueCol
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="table"></param>
        /// <param name="startRow"></param>
        /// <param name="col"></param>
        public static void MergeSameValueCol(Document doc, Table table, int startRow, int col)
        {
            LayoutCollector layoutCollector = new LayoutCollector(doc);
            RowCollection rows = table.Rows;
            for (int i = startRow; i < rows.Count; i++)
            {
                Row row = rows[i];
                string value = row.Cells[col].GetText();
                Cell cell = row.Cells[col];
                int pageNum = layoutCollector.GetEndPageIndex(row);
                if (i == startRow)
                {
                    cell.CellFormat.VerticalMerge = CellMerge.First;
                }
                if (i > startRow)
                {
                    string preVal = rows[i - 1].Cells[col].GetText();
                    int prePageNum = layoutCollector.GetEndPageIndex(rows[i - 1]);
                    if (preVal != value)//||prePageNum!=pageNum)
                    {
                        cell.CellFormat.VerticalMerge = CellMerge.First;
                    }
                    else
                    {
                        cell.CellFormat.VerticalMerge = CellMerge.Previous;
                    }
                }

            }
        }

        /// <summary>
        /// get file formate
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static SaveFormat GetSaveFormat(string fileType)
        {
            if (fileType.ToLower() == "doc")
            {
                return SaveFormat.Doc;
            }
            if (fileType.ToLower() == "docx")
            {
                return SaveFormat.Docx;
            }
            if (fileType.ToLower() == "pdf")
            {
                return SaveFormat.Pdf;
            }

            return SaveFormat.Doc;
        }

        /// <summary>
        /// 将srcDoc文档内容添加到dstDoc最后 
        /// steps of how a document is appended to another.
        /// </summary>
        /// <param name="dstDoc">The destination document where to append to.</param>
        /// <param name="srcDoc">The source document.</param>
        /// <param name="mode">The import mode to use when importing content from another document.</param>
        public static void AppendDocument(Document dstDoc, Document srcDoc, ImportFormatMode mode)
        {
            // Loop through all sections in the source document.
            // Section nodes are immediate children of the Document node so we can just enumerate the Document.
            foreach (Section srcSection in srcDoc)
            {
                // Because we are copying a section from one document to another,
                // it is required to import the Section node into the destination document.
                // This adjusts any document-specific references to styles, lists, etc.
                //
                // Importing a node creates a copy of the original node, but the copy
                // is ready to be inserted into the destination document.
                Node dstSection = dstDoc.ImportNode(srcSection, true, mode);

                // Now the new section node can be appended to the destination document.
                dstDoc.AppendChild(dstSection);
            }
        }

        /// <summary>
        /// InsertNodeAtBookmark
        /// </summary>
        /// <param name="mainDoc"></param>
        /// <param name="subDoc"></param>
        /// <param name="insertnode"></param>
        /// <param name="bookmarkName"></param>
        public static void InsertNodeAtBookmark(Document mainDoc, Document subDoc, Table insertNode, string bookmarkName)
        {
            //定位到书签
            Bookmark bookmark = mainDoc.Range.Bookmarks[bookmarkName];
            //将subDoc中的内容插入到mainDoc中的书签“insertionPlace”位置
            InsertNode(bookmark.BookmarkStart.ParentNode, subDoc, insertNode);
        }

        /// <summary>
        /// InsertNode
        /// </summary>
        /// <param name="insertAfterNode">Node in the destination document</param>
        /// <param name="srcDoc">The document to insert</param>
        public static void InsertNode(Node insertAfterNode, Document srcDoc, Node insertNode)
        {
            // Make sure that the node is either a paragraph or table.
            if ((!insertAfterNode.NodeType.Equals(NodeType.Paragraph)) &
            (!insertAfterNode.NodeType.Equals(NodeType.Table)))
                throw new ArgumentException("The destination node should be either a paragraph or table.");
            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;
            // This object will be translating styles and lists during the import.
            NodeImporter importer = new NodeImporter(srcDoc, insertAfterNode.Document, ImportFormatMode.KeepSourceFormatting);
            // Loop through all sections in the source document.

            Node newNode = importer.ImportNode(insertNode, true);
            dstStory.InsertAfter(newNode, insertAfterNode);
        }


        /// <summary>
        /// InsertDocumentAtBookmark
        /// </summary>
        /// <param name="mainDoc"></param>
        /// <param name="subDoc"></param>
        /// <param name="insertnode"></param>
        /// <param name="bookmarkName"></param>
        public static void InsertDocumentAtBookmark(Document mainDoc, Document subDoc, string bookmarkName)
        {
            //定位到书签
            Bookmark bookmark = mainDoc.Range.Bookmarks[bookmarkName];
            //将subDoc中的内容插入到mainDoc中的书签“insertionPlace”位置
            InsertDocument(bookmark.BookmarkStart.ParentNode, subDoc);
        }



        /// <summary>
        /// InsertDocument
        /// </summary>
        /// <param name="insertAfterNode">Node in the destination document</param>
        /// <param name="srcDoc">The document to insert</param>
        public static void InsertDocument(Node insertAfterNode, Document srcDoc)
        {
            // Make sure that the node is either a paragraph or table.
            if ((!insertAfterNode.NodeType.Equals(NodeType.Paragraph)) &
            (!insertAfterNode.NodeType.Equals(NodeType.Table)))
                throw new ArgumentException("The destination node should be either a paragraph or table.");
            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;
            // This object will be translating styles and lists during the import.
            NodeImporter importer = new NodeImporter(srcDoc, insertAfterNode.Document, ImportFormatMode.KeepSourceFormatting);
            // Loop through all sections in the source document.

            foreach (Section srcSection in srcDoc.Sections)
            {
                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Node srcNode in srcSection.Body)
                {
                    //Let's skip the node if it is a last empty paragraph in a section.
                    if (srcNode.NodeType.Equals(NodeType.Paragraph))
                    { Paragraph para = (Paragraph)srcNode; if (para.IsEndOfSection && !para.HasChildNodes) continue; }
                    //This creates a clone of the node, suitable for insertion into the destination document.

                    Node newNode = importer.ImportNode(srcNode, true);
                    // Insert new node after the reference node.
                    dstStory.InsertAfter(newNode, insertAfterNode);
                    insertAfterNode = newNode;
                }
            }
        }

        /// <summary>
        /// InsertContentInCells
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="cellName"></param>
        /// <param name="words"></param>
        public static void InsertContentInCells(Worksheet ws, string cellName, string words)
        {
            Aspose.Cells.Cell cell = ws.Cells[cellName];
            cell.PutValue(words);
        }

        /// <summary>
        /// SetCellsStyle
        /// </summary>
        /// <param name="cellList"></param>
        public static void SetCellsStyle(List<Aspose.Cells.Cell> cellList)
        {
            foreach (var cell in cellList)
            {
                Aspose.Cells.Style style = cell.GetStyle();
                style.ShrinkToFit = true;
                style.HorizontalAlignment = TextAlignmentType.Center;
                style.VerticalAlignment = TextAlignmentType.Center;
                style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.LeftBorder].Color = System.Drawing.Color.Black;

                style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.TopBorder].Color = System.Drawing.Color.Black;

                style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.RightBorder].Color = System.Drawing.Color.Black;

                style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                style.Borders[Aspose.Cells.BorderType.BottomBorder].Color = System.Drawing.Color.Black;
                cell.SetStyle(style);
            }
        }



        /// <summary>
        /// 替换文档中单位成为上下标
        /// </summary>
        /// <param name="doc"></param>
        public static void ReplaceTheUnit(Document doc)
        {
            List<string> units = new List<string> { "mg/m3", "mg/m3", "μg/m3", "m3/h", "m/s2", "ng TEQ/Nm3", "cmol+/kg", "m3" };
            //遍历文档处理公式字段
            foreach (Paragraph paragraph in doc.GetChildNodes(NodeType.Paragraph, true))
            {

                string pText = paragraph.GetText();
                for (int i = 0; i < units.Count; i++)
                {
                    if (pText.Contains(units[i]))
                    {
                        //包含需替换的公式其为
                        string oldUnit = units[i];
                        Run node = (Run)paragraph.GetChild(NodeType.Run, 0, true);
                        node.Text = node.Text.Replace(oldUnit, "");
                        CreateNewUnit(doc, oldUnit, paragraph);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 创建新单位
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oldUnit"></param>
        /// <param name="paragraph"></param>
        public static void CreateNewUnit(Document doc, String oldUnit, Paragraph paragraph)
        {
            if (oldUnit == "mg/m3")
            {
                Run run = new Run(doc);
                run.Text = "mg/m";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "3"));
            }
            else if (oldUnit == "m3")
            {
                Run run = new Run(doc);
                run.Text = "m";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "3"));
            }
            else if (oldUnit == "mg/m3")
            {
                Run run = new Run(doc);
                run.Text = "mg/m";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "3"));
            }
            else if (oldUnit == "μg/m3")
            {
                Run run = new Run(doc);
                run.Text = "μg/m";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "3"));
            }
            else if (oldUnit == "m3/h")
            {
                Run run = new Run(doc);
                run.Text = "m";
                Run run2 = new Run(doc);
                run2.Text = "/h";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "3"));
                paragraph.AppendChild(run2);
            }
            else if (oldUnit == "m/s2")
            {
                Run run = new Run(doc);
                run.Text = "m/s";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "2"));
            }
            else if (oldUnit == "ng TEQ/Nm3")
            {
                Run run = new Run(doc);
                run.Text = "ng TEQ/Nm";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "3"));
            }
            else if (oldUnit == "cmol+/kg")
            {
                Run run = new Run(doc);
                run.Text = "cmol";
                Run run2 = new Run(doc);
                run2.Text = "/kg";
                paragraph.AppendChild(run);
                paragraph.AppendChild(Superscript(doc, "+"));
                paragraph.AppendChild(run2);
            }
        }

        /// <summary>
        /// 上标生成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Run Superscript(Document doc, string str)
        {
            Run run = new Run(doc);
            run.Text = str;
            run.Font.Superscript = true;
            return run;
        }

        /// <summary>
        /// 下标生成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Run Subscript(Document doc, String str)
        {
            Run run = new Run(doc);
            run.Text = str;
            run.Font.Subscript = true;
            return run;
        }

        /// <summary>
        /// 通过Report Engine 创建表格附加到doc后
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dstDoc"></param>
        /// <param name="srcDocPath"></param>
        /// <param name="model"></param>
        /// <param name="dataSourceName"></param>
        public static void GenerateTableAppendToDoc<T>(Document dstDoc, string srcDocPath, T model, string dataSourceName)
        {
            //Create SrcDocument
            Document srcDocument = new Document(srcDocPath);
            ReportingEngine engine = new ReportingEngine();
            engine.BuildReport(srcDocument, model, dataSourceName);

            //Append To Doc
            //AppendDocument(dstDoc,srcDocument, ImportFormatMode.KeepSourceFormatting);
            DocumentBuilder builder = new DocumentBuilder(dstDoc);
            builder.MoveToDocumentEnd();
            builder.InsertDocument(srcDocument, ImportFormatMode.KeepSourceFormatting);
        }


        /// <summary>
        /// 插入附表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tableMap"></param>
        /// <returns></returns>
        public static void InsertSchedule(Document doc, Dictionary<string, int> tableMap)
        {

            //Extension table
            if (tableMap.Count > 0)
            {
                //Insert The PageBreak
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.MoveToDocumentEnd();
                //Insert the Break Page
                builder.InsertBreak(BreakType.SectionBreakNewPage);

                Table attachedTable = builder.StartTable();

                builder.CellFormat.Borders.LineStyle = LineStyle.None;
                builder.InsertCell();
                builder.EndRow();

                attachedTable.Alignment = TableAlignment.Center;
                attachedTable.PreferredWidth = PreferredWidth.FromPercent(95);

                foreach (KeyValuePair<string, int> kvp in tableMap)
                {
                    int index = kvp.Value;
                    string content = kvp.Key;
                    builder.CellFormat.Borders.LineStyle = LineStyle.None;
                    builder.InsertCell();
                    builder.Write("【附表" + index + "】");
                    builder.EndRow();
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.InsertCell();
                    builder.Write(content);
                    builder.EndRow();
                }

                builder.EndTable();
            }
        }

        /// <summary>
        /// GetHanNumFromString
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetHanNumFromString(string str)
        {
            if (str!=null)
            {
                int count = 0;
                Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");
                for (int i = 0; i < str.Length; i++)
                {
                    if (regex.IsMatch(str[i].ToString()))
                    {
                        count++;
                    }
                }
                return count;
            }

            return 0;
        }


        /// <summary>
        /// CreateWatermarkIntoDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="watermarkText"></param>
        public static void CreateWatermarkIntoDocument(Document doc,string watermarkText)
        {
            Shape watermark = new Shape(doc, ShapeType.TextPlainText);
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 500;
            watermark.Height = 100;
            watermark.Rotation = -40;
            watermark.Fill.Color = System.Drawing.Color.DarkGray;
            watermark.StrokeColor = System.Drawing.Color.DarkGray;
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark.WrapType = WrapType.None;
            watermark.VerticalAlignment = VerticalAlignment.Center;
            watermark.HorizontalAlignment = HorizontalAlignment.Center;

            Paragraph watermarkPara = new Paragraph(doc);
            watermarkPara.AppendChild(watermark);

            foreach (Section sect in doc.Sections)
            {
                // There could be up to three different headers in each section, since we want
                // the watermark to appear on all pages, insert into all headers.
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
            }
        }

        /// <summary>
        /// insertWatermarkIntoHeader
        /// </summary>
        /// <param name="watermarkPara"></param>
        /// <param name="sect"></param>
        /// <param name="headerType"></param>
        public static void InsertWatermarkIntoHeader(Paragraph watermarkPara, Section sect, HeaderFooterType headerType)
        {
            HeaderFooter header = sect.HeadersFooters[headerType];
            if (header == null)
            {
                // There is no header of the specified type in the current section, create it.
                header = new HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }

            // Insert a clone of the watermark into the header.
            header.AppendChild(watermarkPara.Clone(true));
        }

    }
}
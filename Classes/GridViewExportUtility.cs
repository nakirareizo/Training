using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Reflection;

namespace BUILD.Training.Classes
{
    public class GridViewExportUtility
    {
        #region "GV to EXCEL"
        public static void ExportToExcel(string Filename, GridView gv, HttpResponse response)
        {
            gv.AllowPaging = false;
            gv.DataBind();

            response.Clear();
            response.AddHeader("content-disposition", "attachment;filename=" + Filename + ".xls");
            response.Charset = "";
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.ms-excel";
            foreach (GridViewRow row in gv.Rows)
            {
                PrepareControlForExport(row);
            }
            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            gv.RenderControl(htmlWrite);
            response.Write(stringWrite.ToString());
            //response.End();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        public static void Export(string fileName, GridView gv, HttpResponse response)
        {
            gv.AllowPaging = false;
            gv.DataBind();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename=" + fileName + ".xls"));
            HttpContext.Current.Response.Charset = "";
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //  Create a form to contain the grid
                    Table table = new Table();

                    //  add the header row to the table
                    if (gv.HeaderRow != null)
                    {
                        PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }

                    //  add each of the data rows to the table
                    foreach (GridViewRow row in gv.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    //  add the footer row to the table
                    if (gv.FooterRow != null)
                    {
                        PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }

                    //  render the table into the htmlwriter
                    table.RenderControl(htw);

                    //  render the htmlwriter into the response
                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        /// <summary>
        /// Replace any of the contained controls with literals
        /// </summary>
        /// <param name="control"></param>
        private static void PrepareControlForExport(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                Control current = control.Controls[i];
                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text=""));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if (current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }
        #endregion
        #region "Object to Excel"
        public static void ExportToExcel(string fileName, List<Listing> AdminList)
        {
            //The Clear method erases any buffered HTML output.
            HttpContext.Current.Response.Clear();
            //The AddHeader method adds a new HTML header and value to the response sent to the client.
            HttpContext.Current.Response.AddHeader(
                 "content-disposition", string.Format("attachment; filename={0}", fileName + ".xls"));
            //The ContentType property specifies the HTTP content type for the response.
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //Implements a TextWriter for writing information to a string. The information is stored in an underlying StringBuilder.
            using (StringWriter sw = new StringWriter())
            {
                //Writes markup characters and text to an ASP.NET server control output stream. This class provides formatting capabilities that ASP.NET server controls use when rendering markup to clients.
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //  Create a form to contain the List
                    Table table = new Table();
                    TableRow row = new TableRow();
                    foreach (PropertyInfo proinfo in new Listing().GetType().GetProperties())
                    {
                        TableHeaderCell hcell = new TableHeaderCell();
                        hcell.Text = proinfo.Name;
                        row.Cells.Add(hcell);
                    }
                    table.Rows.Add(row);
                    //  add each of the data item to the table
                    foreach (Listing lst in AdminList)
                    {
                        TableRow row1 = new TableRow();
                        TableCell cellStaffID = new TableCell();
                        cellStaffID.Text = "" + lst.StaffID;
                        TableCell cellStaffName = new TableCell();
                        cellStaffName.Text = "" + lst.StaffName;
                        TableCell cellSuperName = new TableCell();
                        cellSuperName.Text = "" + lst.SupervisorName;
                        TableCell cellType = new TableCell();
                        cellType.Text = "" + lst.ApplicationType;
                        TableCell cellTitle = new TableCell();
                        cellTitle.Text = "" + lst.Title;
                        TableCell cellDate = new TableCell();
                        cellDate.Text = "" + lst.DateRequest;
                        TableCell cellStatus = new TableCell();
                        cellStatus.Text = "" + lst.Status;
                        TableCell cellSAP = new TableCell();
                        cellSAP.Text = "" + lst.PostedSAPStatus;
                        row1.Cells.Add(cellStaffID);
                        row1.Cells.Add(cellStaffName);
                        row1.Cells.Add(cellSuperName);
                        row1.Cells.Add(cellType);
                        row1.Cells.Add(cellTitle);
                        row1.Cells.Add(cellDate);
                        row1.Cells.Add(cellStatus);
                        row1.Cells.Add(cellSAP);
                        table.Rows.Add(row1);
                    }
                    //  render the table into the htmlwriter
                    table.RenderControl(htw);
                    //  render the htmlwriter into the response
                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;

namespace TrainingRequisition.ClassLibrary.Utilities
{
    public class UtilityUI
    {
        // Fetch a list from Viewstate, return an empty list if it does not exist
        public static List<ObjectType>
            GetListFromViewState<ObjectType>(string key, StateBag source)
        {
            if (source[key] != null)
                return (List<ObjectType>)source[key];
            return new List<ObjectType>();
        }

        // Fetch a Dictionary from Viewstate, return an empty Dictionary if it does not exist
        public static Dictionary<TKey, TValue>
            GetDictionaryFromViewState<TKey, TValue>(string key, StateBag source)
        {
            if (source[key] != null)
                return (Dictionary<TKey, TValue>)source[key];
            return new Dictionary<TKey, TValue>();
        }

        public static void ExportGridViewtToExcel(GridView gv, HttpResponse response)
        {
            gv.AllowPaging = false;
            gv.DataBind();

            response.Clear();
            response.AddHeader("content-disposition", "attachment;filename=FileName.xls");
            response.Charset = "";
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.ContentType = "application/vnd.xls";
            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            gv.RenderControl(htmlWrite);
            response.Write(stringWrite.ToString());
            response.End();
        }


        /// <summary>
        /// Convert a textbox into datetime object. If conversion fails (due to incorrect format etc, return null.
        /// </summary>
        /// <param name="txtDate"></param>
        /// <returns></returns>
        public static DateTime? GetDate(System.Web.UI.WebControls.TextBox txtDate)
        {
            DateTime output = new DateTime();
            bool success = DateTime.TryParseExact(txtDate.Text, "d/M/yyyy", null, DateTimeStyles.None, out output);
            if (success)
                return output;
            return null;
        }

        public static string ValidateFromToDates(TextBox txtFromDate, TextBox txtToDate,
            bool fromDateMandatory, bool toDateMandatory)
        {
            // check format of dates
            DateTime? fromDate = UtilityUI.GetDate(txtFromDate);

            if (fromDateMandatory && string.IsNullOrEmpty(txtFromDate.Text))
                return "Please enter a From Date (dd/mm/yyyy)";

            if (!fromDate.HasValue && !string.IsNullOrEmpty(txtFromDate.Text))
                return "Please enter a valid From Date (dd/mm/yyyy)";

            DateTime? toDate = UtilityUI.GetDate(txtToDate);
            if (toDateMandatory && string.IsNullOrEmpty(txtToDate.Text))
                return "Please enter a To Date (dd/mm/yyyy)";

            if (!toDate.HasValue && !string.IsNullOrEmpty(txtToDate.Text))
                return "Please enter a valid To Date (dd/mm/yyyy)";

            if (fromDate.HasValue && toDate.HasValue &&
                fromDate.Value.CompareTo(toDate.Value) > 0)
                return "From date should be earlier than to date.";

            return "";
        }
    }
}

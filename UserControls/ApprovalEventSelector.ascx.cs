using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrainingRequisition.Classes;
using TrainingRequisition.ClassLibrary.Utilities;

namespace TrainingRequisition.UserControls
{
    public partial class ApprovalEventSelector : System.Web.UI.UserControl
    {
        public class SelectEventArgs : EventArgs
        {
            public EventDate SelectedEventDate { get; set; }
        }

        public delegate void SelectEventHandler(object sender, SelectEventArgs args);
        public event SelectEventHandler SelectEvent;

        public List<EventDate> EventDates
        {
            get
            {
                return UtilityUI.GetListFromViewState<EventDate>("EventDates", ViewState);
            }
            set
            {
                ViewState["EventDates"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAndShowEvents();
            }
        }

        public void LoadAndShowEvents()
        {
            int stage = 2;
            List<EventDate> eventDates = BookedEvent.GetAllEventDates(stage);
            dlEventDates.DataValueField = "ID";
            dlEventDates.DataTextField = "DisplayName";
            dlEventDates.DataSource = eventDates;
            dlEventDates.DataBind();
            EventDates = eventDates;
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {

            if (SelectEvent != null)
            {
                SelectEventArgs args = new SelectEventArgs();
                if (dlEventDates.SelectedIndex == -1)
                   return;

                int eventDateID = Convert.ToInt32(dlEventDates.SelectedValue);

                foreach (EventDate ed in EventDates)
                {
                    if (ed.Id == eventDateID)
                    {
                        args.SelectedEventDate = ed;
                        SelectEvent(this, args);
                        break;
                    }
                }
                
            }
        }
    }
}
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
    public partial class EventDetails : System.Web.UI.UserControl
    {
        public class EventDetailsArgs : EventArgs
        {
            public TrainingEvent CurrentEvent{ get; set; }
            public EventDate CurrentEventDate { get; set; }
        }

        public delegate void AddEventHandler(object sender, EventDetailsArgs args);
        public event AddEventHandler AddEvent;

        public delegate void FailedValidatonHandler(object sender, EventArgs args);
        public event FailedValidatonHandler FailedValidation;

        public bool ReadOnly
        {
            set
            {
                txtTitle.ReadOnly = value;
                txtProvider.ReadOnly = value;
                txtPrice.ReadOnly = value;
                dlCurrencies.Enabled = value;
                txtDateFrom.ReadOnly = value;
                txtDateTo.ReadOnly = value;
                txtDateFrom.Enabled = value;
                txtDateTo.Enabled = value;
                dlTrainingTypes.Enabled = value;
                //btnAdd.Visible = !value;
            }
        }

        public TrainingEvent CurrentEvent
        {
            get
            {
                TrainingEvent output =(TrainingEvent)(ViewState["CurrentEvent"]);
                if (output == null)
                {
                    output = new TrainingEvent();
                    output.UserDefined = true;
                }
                
                
                output.Title = txtTitle.Text;
                return output;
            }
            set
            {
                ViewState["CurrentEvent"] = value;

                if (value != null)
                {
                    txtTitle.Text = value.Title;
                    //btnAdd.Text = "Update";
                }
                else
                {
                    txtTitle.Text = "";
                    txtProvider.Text = "";
                    txtPrice.Text = "";
                    dlTrainingTypes.SelectedIndex = 0;
                    //btnAdd.Text = "Add";
                }
            }
        }

        private void ResetCurrency()
        {
           dlCurrencies.Enabled = false;
            dlCurrencies.Items.Clear();
            ListItem item = new ListItem("RM");
            dlCurrencies.Items.Add(item);
            item.Selected = true;
            dlCurrencies.Enabled = false;
        }

        public EventDate CurrentEventDate
        {
            get
            {
                EventDate output = null;
                if (ViewState["CurrentEventDate"] == null)
                    output = new EventDate();
                else
                    output = (EventDate)ViewState["CurrentEventDate"];

                DateTime? startDate = UtilityUI.GetDate(txtDateFrom);
                DateTime? endDate = UtilityUI.GetDate(txtDateTo);
                if (startDate.HasValue)
                    output.StartDate = startDate.Value;
                if (endDate.HasValue)
                    output.EndDate = endDate.Value;
                return output;
            }

            set
            {
                ViewState["CurrentEventDate"] = value;

                if (value != null)
                {
                    txtDateFrom.Text = value.StartDate.ToString("d/M/yyyy");
                    txtDateFrom.ReadOnly = true;
                    txtDateFrom.Enabled = false;
                    txtDateTo.Text = value.EndDate.ToString("d/M/yyyy");
                    txtDateTo.ReadOnly = true;
                    txtDateTo.Enabled = false;
                    txtProvider.Text = value.Provider;

                    // show currency
                    ResetCurrency();
                    if (!string.IsNullOrEmpty(value.Currency))
                    {
                        if (dlCurrencies.SelectedItem != null)
                            dlCurrencies.SelectedItem.Selected = false;

                        ListItem item = new ListItem(value.Currency);
                        dlCurrencies.Items.Add(item);
                        item.Selected = true;
                    }

                    txtPrice.Text = value.Price.ToString("#.00");

                    dlTrainingTypes.SelectedIndex = -1;
                    ListItem trainingTypeItem = dlTrainingTypes.Items.FindByValue(value.TrainingType);
                    if (trainingTypeItem != null)
                        trainingTypeItem.Selected = true;
                }
                else
                {
                    txtDateTo.Text = "";
                    txtDateFrom.Text = "";
                    //btnAdd.Text = "Add";
                    ReadOnly = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<TrainingType> trainingTypes = TrainingType.GetAll();
                dlTrainingTypes.DataSource = trainingTypes;
                dlTrainingTypes.DataBind();
                dlTrainingTypes.Enabled = false;
            }
        }

        //protected void btnAdd_Click(object sender, EventArgs e)
        //{
        //    if (Validate())
        //    {
        //        EventDetailsArgs args = new EventDetailsArgs();
        //        args.CurrentEvent = CurrentEvent;
        //        args.CurrentEventDate = CurrentEventDate;
        //        if (AddEvent != null)
        //            AddEvent(this, args);
        //    }
        //    else
        //    {
        //        if (FailedValidation != null)
        //            FailedValidation(this, null);
        //    }
        //}

        //private bool Validate()
        //{
        //    vldTitle.Validate();
        //    vldProvider.Validate();
        //    vldPrice1.Validate();
        //    //vldPrice2.Validate();
        //    vldDates.Validate();

        //    return vldTitle.IsValid && vldProvider.IsValid &&
        //        vldPrice1.IsValid && vldDates.IsValid;
        //}


        //protected void vldDates_ServerValidate(object source, ServerValidateEventArgs args)
        //{
        //    string errorMessage = UtilityUI.ValidateFromToDates(txtDateFrom, txtDateTo, true, true);
        //    if (!string.IsNullOrEmpty(errorMessage))
        //    {
        //        args.IsValid = false;
        //        //vldDates.Text = errorMessage;
        //    }
        //}


    }
}
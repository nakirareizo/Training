using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingRequisition.ClassLibrary.Utilities;
using TrainingRequisition.ClassLibrary.Bases;
using System.Web.UI.WebControls;
using TrainingRequisition.ClassLibrary.Entities;

namespace TrainingRequisition.Classes
{
   public class EventSelectorBase<EventClass> : System.Web.UI.UserControl where EventClass : Base
   {
      private const string KeyCurrentStaffUsername = "CurrentStaffUsername";
      private const string KeyAvailableEvents = "AvailableEvents";
      private const string KeySelectedEvents = "SelectedEvents";
      private const string KeyUnselectedEvents = "UnselectedEvents";
      private const string KeyEventGroups = "EventGroups";

      protected List<EventClass> FilterOnlySelectedEvents(List<EventClass> preSubmissionList, List<int> selectedIDs)
      {

         List<EventClass> submissionList = new List<EventClass>();

         if (selectedIDs.Count == 0)
         {
            foreach (EventClass ev in preSubmissionList)
            {
               submissionList.Add(ev);
            }
         }

         else
         {
            foreach (int id in selectedIDs)
            {
               foreach (EventClass ev in preSubmissionList)
               {
                  if (ev.Id == Convert.ToInt32(id))
                     submissionList.Add(ev);
               }
            }
         }
         return submissionList;
      }


      public string CurrentStaffUsername
      {
         get
         {
            if (ViewState[KeyCurrentStaffUsername] == null)
               return null;
            return ViewState[KeyCurrentStaffUsername].ToString();
         }
         set
         {
            ViewState[KeyCurrentStaffUsername] = value;
         }
      }

      public List<EventClass> AvailableEvents
      {
         get
         {
            return UtilityUI.GetListFromViewState<EventClass>(KeyAvailableEvents, ViewState);
         }
         set
         {
            ViewState[KeyAvailableEvents] = value;
         }
      }

      public List<EventClass> SelectedEvents
      {
         get
         {
            return UtilityUI.GetListFromViewState<EventClass>
                (KeySelectedEvents, ViewState);
         }
         set
         {

            ViewState[KeySelectedEvents] = value;
         }
      }

      protected List<EventClass> FilterAvailableEvents()
      {
         List<EventClass> filtered = new List<EventClass>();
         foreach (EventClass availableEvent in AvailableEvents)
         {
            EventClass find = GetEventFromList(availableEvent.Id,
                SelectedEvents);
            if (find == null)
               filtered.Add(availableEvent);
         }
         return filtered;
      }

      /// <summary>
      /// Unselect an event. Remove it from the Selected Event list
      /// and transfer it into the UnselectedEvent list
      /// </summary>
      /// <param name="eventObj"></param>
      protected void UnselectEvent(EventClass toUnselect, bool RemoveFromSelected)
      {
         if (!toUnselect.IsNew)
            UnselectedEvents.Add(toUnselect);

         // remove it from selected events
         if (RemoveFromSelected)
            SelectedEvents.Remove(toUnselect);
      }

      protected void ShowEventGroups(List<EventGroup> eventGroups, int level, DropDownList dlEventGroups)
      {
         if (eventGroups == null)
            return;

         // if this is the root, set it in viewstate
         if (level == 0)
            EventGroups = eventGroups;

         foreach (EventGroup eventGroup in eventGroups)
         {
            string indent = "";
            for (int i = 0; i < level; i++)
               indent += "--";

            string itemText = indent + eventGroup.Title;
            ListItem item = new ListItem(itemText, eventGroup.ID.ToString());
            dlEventGroups.Items.Add(item);

            ShowEventGroups(eventGroup.Children, level + 1, dlEventGroups);
         }

      }

      protected Staff GetLoggedInUser()
      {
         string username = Page.User.Identity.Name;
         Staff output = Staff.GetFromUsername(username);
         return output;
      }

      protected void ShowSelectedEvents(ListBox lbSelected)
      {
         lbSelected.DataSource = SelectedEvents;
         lbSelected.DataBind();
      }

      /// <summary>
      /// Select an available event. If it does not yet exist in the selected
      /// event list, add it into the list.
      /// </summary>
      /// <param name="toSelect"></param>
      protected void SelectEvent(EventClass toSelect)
      {
         SelectedEvents.Add(toSelect);
      }


      protected void UnselectEventsInListbox(ListBox lbSelected)
      {
         foreach (ListItem item in lbSelected.Items)
         {
            if (item.Selected)
            {
               EventClass selectedEvent = GetEventFromList(Convert.ToInt32(item.Value), SelectedEvents);
               if (selectedEvent != null)
                  UnselectEvent(selectedEvent, true);
            }
         }
      }

      protected EventGroup GetSelectedEventGroup(DropDownList dlEventGroups)
      {
         int eventGroupId = Convert.ToInt32(dlEventGroups.SelectedValue);
         EventGroup selectedEventGroup = FindEventGroupById(eventGroupId);
         return selectedEventGroup;
      }

      private EventGroup FindEventGroupById(int eventGroupId)
      {
         List<EventGroup> eventGroups = EventGroups;
         foreach (EventGroup eventGroup in eventGroups)
         {
            EventGroup found = eventGroup.FindById(eventGroupId);
            if (found != null)
               return found;
         }
         return null;

      }


      protected void SelectAvailableEventsFromListbox(ListBox lbAvailable)
      {
         foreach (ListItem item in lbAvailable.Items)
         {
            if (item.Selected)
            {
               EventClass selectedEvent = GetEventFromList(Convert.ToInt32(item.Value),
                   AvailableEvents);
               if (selectedEvent != null)
                  SelectEvent(selectedEvent);
            }
         }
      }

      protected void UnselectAllEvents()
      {
         foreach (var eventObj in SelectedEvents)
            UnselectEvent(eventObj, false);
         SelectedEvents = new List<EventClass>(); // reset to empty
      }

      protected void SelectAllAvailableEvents()
      {
         List<EventClass> availableEvents = AvailableEvents;

         foreach (var eventObj in availableEvents)
            SelectEvent(eventObj);
      }


      /// <summary>
      /// Look for a event from a list matching the event ID given
      /// </summary>
      /// <param name="eventId"></param>
      /// <param name="source"></param>
      /// <returns></returns>
      protected EventClass GetEventFromList(int? eventId, List<EventClass> source)
      {
         // reserve for suggested events in the future
         //if (eventId.HasValue && eventId == ???)
         //    return null;

         foreach (EventClass eventObj in source)
         {
            if (eventObj.Id == eventId)
               return eventObj;
         }
         return null;
      }

      public List<EventClass> UnselectedEvents
      {
         get
         {
            return UtilityUI.GetListFromViewState<EventClass>
                (KeyUnselectedEvents, ViewState);
         }
         set
         {
            ViewState[KeyUnselectedEvents] = value;
         }
      }

      public List<EventGroup> EventGroups
      {
         get
         {
            return UtilityUI.GetListFromViewState<EventGroup>
                (KeyEventGroups, ViewState);
         }
         set
         {
            ViewState[KeyEventGroups] = value;
         }
      }

      protected void ShowMessage(string message, Label lblMessage)
      {
         lblMessage.Visible = !string.IsNullOrEmpty(message);
         lblMessage.Text = "<br/>" + message;
      }



      internal void ResetQuestions(UserControls.Questions uscQuestions)
      {
         uscQuestions.Reset();
      }
   }
}

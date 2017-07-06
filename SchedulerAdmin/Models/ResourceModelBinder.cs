using LNF.Models.Scheduler;
using System;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace SchedulerAdmin.Models
{
    public class ResourceModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(ResourceModel))
                return base.BindModel(controllerContext, bindingContext);
            
            var model = new ResourceModel();
            model.ResourceID = GetValueAsInt(bindingContext, "ResourceID");
            model.ResourceName = GetValueAsString(bindingContext, "ResourceName");
            model.ResourceDescription = GetValueAsString(bindingContext, "ResourceDescription");
            model.ProcessTechID = GetValueAsInt(bindingContext, "ProcessTechID");
            model.AuthDuration = GetValueAsInt(bindingContext, "AuthDuration");
            model.AuthState = GetValueAsBool(bindingContext, "AuthState");
            model.ReservFence = GetValueAsTimeSpanFromHours(bindingContext, "ReservFenceHours");
            model.Granularity = GetValueAsTimeSpanFromMinutes(bindingContext, "GranularityMinutes");
            model.Offset = GetValueAsTimeSpanFromHours(bindingContext, "OffsetHours");
            model.MinReservTime = GetValueAsTimeSpanFromMinutes(bindingContext, "MinReservTimeMinutes");
            model.MaxReservTime = GetValueAsTimeSpanFromHours(bindingContext, "MaxReservTimeHours");
            model.MaxAlloc = GetValueAsTimeSpanFromHours(bindingContext, "MaxSchedulableHours");
            model.MinCancelTime = GetValueAsTimeSpanFromMinutes(bindingContext, "MinCancelTimeMinutes");
            model.GracePeriod = GetGracePeriod(bindingContext);
            model.AutoEnd = GetValueAsTimeSpanFromMinutes(bindingContext, "AutoEndMinutes");
            model.UnloadTime = GetValueAsTimeSpanFromMinutes(bindingContext, "UnloadTimeMinutes");
            model.HelpdeskEmail = GetValueAsString(bindingContext, "HelpdeskEmail");
            model.WikiPageUrl = GetValueAsString(bindingContext, "WikiPageUrl");
            model.IsSchedulable = GetValueAsBool(bindingContext, "IsSchedulable");
            model.ResourceIsActive = GetValueAsBool(bindingContext, "ResourceIsActive");
            return model;
        }

        private string GetValueAsString(ModelBindingContext context, string key)
        {
            var result = context.ValueProvider.GetValue(key);
            if (result == null) return null;
            return result.AttemptedValue;
        }

        private bool GetValueAsBool(ModelBindingContext context, string key)
        {
            var result = context.ValueProvider.GetValue(key);
            if (result == null) return false;
            return result.AttemptedValue.Contains("true");
        }

        private TimeSpan GetValueAsTimeSpanFromHours(ModelBindingContext context, string key)
        {
            var result = context.ValueProvider.GetValue(key);
            if (result == null) return TimeSpan.Zero;
            int hours = int.Parse(result.AttemptedValue);
            return TimeSpan.FromHours(hours);
        }

        private TimeSpan GetValueAsTimeSpanFromMinutes(ModelBindingContext context, string key)
        {
            var result = context.ValueProvider.GetValue(key);
            if (result == null) return TimeSpan.Zero;
            int minutes = int.Parse(result.AttemptedValue);
            return TimeSpan.FromMinutes(minutes);
        }

        private int GetValueAsInt(ModelBindingContext context, string key)
        {
            var result = context.ValueProvider.GetValue(key);
            if (result == null) return 0;
            return int.Parse(result.AttemptedValue);
        }

        private TimeSpan GetGracePeriod(ModelBindingContext context)
        {
            int hour = GetValueAsInt(context, "GracePeriodHour");
            int minute = GetValueAsInt(context, "GracePeriodMinute");
            int gracePeriodMinutes = (hour * 60) + minute;
            return TimeSpan.FromMinutes(gracePeriodMinutes);
        }
    }
}
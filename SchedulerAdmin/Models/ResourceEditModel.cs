using System;
using LNF.Repository.Scheduler;

namespace SchedulerAdmin.Models
{
    public class ResourceEditModel
    {
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ResourceDescription { get; set; }
        public int ProcessTechID { get; set; }
        public int AuthDuration { get; set; }
        public bool RollingPeriod { get; set; }
        public int ReservationFenceHours { get; set; }
        public int GranularityMinutes { get; set; }
        public int OffsetHours { get; set; }
        public int MinReservationMinutes { get; set; }
        public int MaxReservationHours { get; set; }
        public int MaxSchedulableHours { get; set; }
        public int MinCancelTimeMinutes { get; set; }
        public int GracePeriodHours { get; set; }
        public int GracePeriodMinutes { get; set; }
        public int AutoEndMinutes { get; set; }
        public int UnloadTimeMinutes { get; set; }
        public string HelpdeskEmail { get; set; }
        public string WikiPageUrl { get; set; }
        public bool IsSchedulable { get; set; }
        public bool IsActive { get; set; }

        public static ResourceEditModel GetDefault()
        {
            return new ResourceEditModel()
            {
                ResourceID = 0,
                ResourceName = string.Empty,
                ResourceDescription = string.Empty,
                ProcessTechID = 0,
                AuthDuration = 12,
                RollingPeriod = false,
                ReservationFenceHours = 168,
                GranularityMinutes = 60,
                OffsetHours = 0,
                MinReservationMinutes = 60,
                MaxReservationHours = 1,
                MaxSchedulableHours = 48,
                MinCancelTimeMinutes = 5,
                GracePeriodHours = 1,
                GracePeriodMinutes = 0,
                AutoEndMinutes = 15,
                UnloadTimeMinutes = -1,
                HelpdeskEmail = string.Empty,
                WikiPageUrl = string.Empty,
                IsSchedulable = true,
                IsActive = true
            };
        }

        public static ResourceEditModel Create(Resource res)
        {
            return new ResourceEditModel()
            {
                ResourceID = res.ResourceID,
                ResourceName = res.ResourceName,
                ResourceDescription = res.Description,
                ProcessTechID = res.ProcessTech.ProcessTechID,
                AuthDuration = res.AuthDuration,
                RollingPeriod = res.AuthState,
                ReservationFenceHours = (int)TimeSpan.FromMinutes(res.ReservFence).TotalHours,
                GranularityMinutes = res.Granularity,
                OffsetHours = res.Offset,
                MinReservationMinutes = res.MinReservTime,
                MaxReservationHours = (int)TimeSpan.FromMinutes(res.MaxReservTime).TotalHours,
                MaxSchedulableHours = (int)TimeSpan.FromMinutes(res.MaxAlloc).TotalHours,
                MinCancelTimeMinutes = res.MinCancelTime,
                GracePeriodHours = TimeSpan.FromMinutes(res.GracePeriod).Hours,
                GracePeriodMinutes = TimeSpan.FromMinutes(res.GracePeriod).Minutes,
                AutoEndMinutes = res.AutoEnd,
                UnloadTimeMinutes = res.UnloadTime.GetValueOrDefault(-1),
                HelpdeskEmail = res.HelpdeskEmail,
                WikiPageUrl = res.WikiPageUrl,
                IsSchedulable = res.IsSchedulable,
                IsActive = res.IsActive
            };
        }
    }
}
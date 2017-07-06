using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using SchedulerAdmin.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchedulerAdmin.Controllers.Api
{
    public class ResourceController : ApiController
    {
        [Route("api/resource/{resourceId}")]
        public ResourceEditModel Get(int resourceId)
        {
            if (resourceId == 0)
            {
                return ResourceEditModel.GetDefault();
            }
            else
            {
                Resource res = DA.Current.Single<Resource>(resourceId);

                if (res == null)
                {
                    var response = Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Cannot find Resource with ResourceID = {0}", resourceId));
                    throw new HttpResponseException(response);
                }

                return ResourceEditModel.Create(res);
            }
        }

        [Route("api/resource/offset")]
        public IEnumerable<int> GetOffsets(int granularity)
        {
            var result = new List<int>();

            result.Add(0);

            if (granularity > 60)
                result.Add(1);

            if (granularity > 120)
                result.Add(2);

            return result;
        }

        [Route("api/resource/min-reservation-time")]
        public IEnumerable<ReservationTime> GetMinReservationTime(int granularity)
        {
            // Load Hours
            var result = new List<ReservationTime>();

            for (int i = 1; i <= 6; i++)
            {
                double minReservTime = i * granularity;
                TimeSpan ts = TimeSpan.FromMinutes(minReservTime);
                double day, hour, minute;

                //hour = Math.Floor(minReservTime / 60);
                //minute = minReservTime % 60;
                day = ts.Days;
                hour = ts.Hours;
                minute = ts.Minutes;

                string text = string.Empty;

                if (day > 0) text += string.Format("{0} day ", day);
                if (hour > 0) text += string.Format("{0} hr ", hour);
                if (minute > 0) text += string.Format("{0} min ", minute);

                result.Add(ReservationTime.Create(minReservTime, text.Trim()));
            }

            return result;
        }

        [Route("api/resource/max-reservation-time")]
        public IEnumerable<int> GetMaxReservTime(int granularity, int minReservTime)
        {
            //                                                                  1               2   3    6   12   24      days
            int[] maxReservTimeList = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 18, 24, 30, 36, 42, 48, 72, 144, 288, 576 }; //hours

            // the max is 576 because the max granularity is now 1440 (1440 * 24 / 60 = 576)

            int maxValue = Convert.ToInt32(granularity * 24 / 60);
            int minValue = Convert.ToInt32(Math.Ceiling((double)minReservTime / 60));

            var result = new List<int>();

            for (int i = 0; i < maxReservTimeList.Length; i++)
            {
                int h = maxReservTimeList[i];
                if (h > maxValue) break;
                if (h >= minValue && (h * 60) % granularity == 0)
                    result.Add(h);
            }

            return result;
        }

        [Route("api/resource/grace-period-hour")]
        public IEnumerable<int> GetGracePeriodHour(int granularity, int minReservTime)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            var stepSize = Convert.ToInt32(Math.Ceiling((double)granularity / 60));

            int minValue = 0;

            if (granularity >= 60) minValue = stepSize;

            for (int i = minValue; i <= maxHour; i += stepSize)
            {
                result.Add(i);
            }

            return result;
        }

        [Route("api/resource/grace-period-minute")]
        public IEnumerable<int> GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            if (gracePeriodHour == maxHour && granularity < 60)
            {
                var maxMinute = minReservTime % 60;
                for (int i = 0; i <= maxMinute; i += granularity)
                {
                    result.Add(i);
                }
            }
            else
            {
                var count = Convert.ToInt32(Math.Ceiling(60 / (double)granularity));
                for (int i = 0; i < count; i++)
                {
                    var minute = granularity * i;
                    result.Add(minute);
                }
            }

            return result;
        }

        [Route("api/resource")]
        public int Post([FromBody] ResourceEditModel model)
        {
            Resource res = DA.Current.Single<Resource>(model.ResourceID);
            bool insert;

            if (res == null)
            {
                insert = true;
                res = new Resource();
                res.ResourceID = model.ResourceID;
                res.State = ResourceState.Online;
                res.IsReady = true;
            }
            else
            {
                insert = false;
                res = DA.Current.Single<Resource>(model.ResourceID);
            }

            var pt = DA.Current.Single<ProcessTech>(model.ProcessTechID);
            res.ProcessTech = pt;
            res.Lab = pt.Lab;
            res.ResourceName = model.ResourceName;
            res.Description = model.ResourceDescription;
            res.ReservFence = (int)TimeSpan.FromHours(model.ReservationFenceHours).TotalMinutes;
            res.Granularity = model.GranularityMinutes;
            res.Offset = model.OffsetHours;
            res.MinReservTime = model.MinReservationMinutes;
            res.MaxReservTime = (int)TimeSpan.FromHours(model.MaxReservationHours).TotalMinutes;
            res.MaxAlloc = (int)TimeSpan.FromHours(model.MaxSchedulableHours).TotalMinutes;
            res.MinCancelTime = model.MinCancelTimeMinutes;
            res.GracePeriod = (model.GracePeriodHours * 60) + model.GracePeriodMinutes;
            res.AutoEnd = model.AutoEndMinutes;
            res.AuthDuration = model.AuthDuration;
            res.AuthState = model.RollingPeriod;
            res.UnloadTime = model.UnloadTimeMinutes;
            res.HelpdeskEmail = model.HelpdeskEmail;
            res.WikiPageUrl = model.WikiPageUrl;
            res.IsSchedulable = model.IsSchedulable;
            res.IsActive = model.IsActive;

            if (insert)
                DA.Current.Insert(res);

            return res.ResourceID;
        }
    }
}

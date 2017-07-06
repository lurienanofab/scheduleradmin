namespace SchedulerAdmin.Models
{
    public class PropertiesModel
    {
        public double LateChargePenaltyMultiplier { get; set; }
        public double AuthorizationExpirationWarning { get; set; }
        public string ResourceIpPrefix { get; set; }
        public int SchedulerAdministratorClientID { get; set; }
        public int GeneralLabAccountID { get; set; }
    }
}
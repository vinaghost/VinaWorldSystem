namespace API.Endpoints.Servers
{
    public class Response
    {
        public DateTime LastUpdate { get; set; }
        public int VillageCount { get; set; }
        public int PlayerCount { get; set; }
        public int AllianceCount { get; set; }
    }
}
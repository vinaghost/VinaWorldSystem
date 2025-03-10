namespace WebApi.Entities
{
    public class Server
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TileUpdateAt { get; set; }
        public DateTime VillageUpdateAt { get; set; }

        public ICollection<Tile> Tiles { get; } = new List<Tile>();
    }
}
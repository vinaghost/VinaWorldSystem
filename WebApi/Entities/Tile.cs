namespace WebApi.Entities
{
    public class Tile
    {
        public int Id { get; set; }

        public int ServerId { get; set; }

        public Server Server { get; set; } = null!;

        public int MapId { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public required string Type { get; set; }
        public required string Status { get; set; }
    }
}
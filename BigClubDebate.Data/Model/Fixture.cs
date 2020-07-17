namespace BigClubDebate.Data
{
    public class Fixture
    {
        public string Home { get; set; }
        public string Away { get; set; }
        public string[] Teams => new[] { Home, Away };
        public override string ToString() => $"{Home} vs {Away}";
    }
}
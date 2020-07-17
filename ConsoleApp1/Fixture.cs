namespace ConsoleApp1
{
    public class Fixture 
    {
        public string home { get; set; }
        public string away { get; set; }
        public string[] teams => new[] { home, away };
        public override string ToString() => $"{home} vs {away}";
    }
}
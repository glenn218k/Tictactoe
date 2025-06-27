using BlazorApp1.Enums;

namespace BlazorApp1.Models
{
    public class WinningPlay
    {
        public List<string> WinningMoves { get; set; } = [];
        public EvaluationDirection WinningDirection { get; set; }
        public PieceStyle WinningStyle { get; set; }
        public string? WinningName { get; set; }
        public string? WinningIcon { get; set; }
    }
}
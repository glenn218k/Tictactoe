using BlazorApp1.Enums;
using System.Drawing;

namespace BlazorApp1.Models
{
    public class Player
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public Color? Color { get; set; }
        public PieceStyle PieceStyle { get; set; }

        public Player() { }
    }
}
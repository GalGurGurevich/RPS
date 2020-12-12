using Windows.UI;

namespace GameLib
{
    public class Player
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public GamePiece[] Pieces { get; set; }
    }
}

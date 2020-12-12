namespace GameLib
{
    public abstract class GamePiece
    {
        public RockPaperScissorsGame Game { get; set; }
        public Player Owner { get; set; }
        public Position Position { get; set; }
        public bool IsRevealed { get; set; }
        public abstract bool IsMovable { get; }
        public bool IsDead { get; set; }

        public abstract void OnAttacked(GamePiece attacker);
    }
}

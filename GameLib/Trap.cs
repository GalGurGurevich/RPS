namespace GameLib
{
    public class Trap : GamePiece
    {
        public override bool IsMovable => false;

        public override void OnAttacked(GamePiece attacker)
        {
            IsRevealed = true;
            Game.RemovePiece(attacker);
        }
    }
}

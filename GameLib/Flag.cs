namespace GameLib
{
    public class Flag : GamePiece
    {
        public override bool IsMovable => false;

        public override void OnAttacked(GamePiece attacker)
        {
            Game.RemovePiece(this);
            Game.MovePiece(attacker, Position);
            Game.GameOver(Owner);
        }
    }
}

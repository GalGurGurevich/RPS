namespace GameLib
{
    public class Soldier : GamePiece
    {
        public Weapon Weapon { get; set; }

        public override bool IsMovable => true;

        public override void OnAttacked(GamePiece attacker)
        {
            Soldier opponent = (Soldier)attacker;

            while (!DoCombat(opponent))
            {
                // Do nothing here, a loop is used to do proper combat after a tie and weapon randomization
            }
        }

        // This returns true when the combar is resolved, or false in case when a winner cannot be determined and the battle needs to be done again
        private bool DoCombat(Soldier opponent)
        {
            // This is a tie and we need to reroll the weapons
            if (Weapon == opponent.Weapon)
            {
                opponent.Weapon = Game.GetRandomWeapon();
                opponent.IsRevealed = true;

                Weapon = Game.GetRandomWeapon();
                IsRevealed = true;
                return false;
            }
            // This is a victory for the attacked piece
            else if (
                (opponent.Weapon == Weapon.Rock && Weapon == Weapon.Paper) ||
                (opponent.Weapon == Weapon.Paper && Weapon == Weapon.Scissors) ||
                (opponent.Weapon == Weapon.Scissors && Weapon == Weapon.Rock)
            )
            {
                Game.RemovePiece(opponent);
                IsRevealed = true;
                return true;
            }
            // This is a victory for the attacker
            else if (
                (Weapon == Weapon.Rock && opponent.Weapon == Weapon.Paper) ||
                (Weapon == Weapon.Paper && opponent.Weapon == Weapon.Scissors) ||
                (Weapon == Weapon.Scissors && opponent.Weapon == Weapon.Rock)
            )
            {
                Game.RemovePiece(this);
                opponent.IsRevealed = true;
                Game.MovePiece(opponent, Position);
                return true;
            }
            // This should not happen + Can earase the Else and make it simple return false
            else
            {
                return false;
            }
        }
    }
}

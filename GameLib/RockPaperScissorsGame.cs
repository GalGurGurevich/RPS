using System;

namespace GameLib
{
    public class RockPaperScissorsGame
    {
        public const int BOARD_SIZE = 8;

        public GamePiece[,] _gameBoard;
        private Player _player1;
        private Player _player2;
        public bool _isGameOver;
        public Player _currentPlayer;
        public Player _winner;
        private Random _rand = new Random();

        public void InitNewGame(Player player1, Player player2)                 //Set board, Players, Game Conditions/Settings
        {
            _gameBoard = new GamePiece[BOARD_SIZE, BOARD_SIZE];
            _player1 = player1;
            _player2 = player2;
            _isGameOver = false;
            _currentPlayer = _player1;

            for (int i = 0; i < _player1.Pieces.Length; i++)
            {
                GamePiece piece = _player1.Pieces[i];
                _gameBoard[piece.Position.Row, piece.Position.Column] = piece;
            }

            for (int i = 0; i < _player2.Pieces.Length; i++)
            {
                GamePiece piece = _player2.Pieces[i];
                _gameBoard[piece.Position.Row, piece.Position.Column] = piece;
            }
        }

        public bool MakeMove(GamePiece piece, Direction direction)
        {
            if (!piece.IsMovable) return false;                                     //In case its flag/trap can't move
            Position position = GetDesiredPosition(piece.Position, direction);
            if (!IsMoveInGameBounds(position)) return false;

            GamePiece attackedPiece = _gameBoard[position.Row, position.Column];
            if (attackedPiece == null)                                              //Emptey slot, Possible movement
            {
                MovePiece(piece, position);
                if (!CheckLossDueToNoMorePieces())
                    SwitchTurn();
                return true;
            }
            else if (attackedPiece.Owner.Name == piece.Owner.Name)                  //If it's the same piece of the player
            {
                return false;
            }
            else
            {
                attackedPiece.OnAttacked(piece);
                if (CheckForTie())
                {
                    GameOver(null);
                }
                else
                {
                    if (!CheckLossDueToNoMorePieces())
                        SwitchTurn();
                }
                return true;
            }
        }

        private bool CheckLossDueToNoMorePieces()
        {
            bool player1HasPieces = PlayerHasMovablePieces(_player1);
            bool player2HasPieces = PlayerHasMovablePieces(_player2);
            if (!player1HasPieces && !player2HasPieces)
            {
                GameOver(null);
                return true;
            }
            else if (!player1HasPieces)
            {
                GameOver(_player1);
                return true;
            }
            else if (!player2HasPieces)
            {
                GameOver(_player2);
                return true;
            }
            return false;
        }

        private bool PlayerHasMovablePieces(Player player)
        {
            for (int i = 0; i < player.Pieces.Length; i++)
                if (player.Pieces[i].IsMovable && !player.Pieces[i].IsDead)
                    return true;

            return false;
        }

        private void SwitchTurn()
        {
            _currentPlayer = _currentPlayer == _player1 ? _player2 : _player1;
        }

        public void MovePiece(GamePiece piece, Position position)       //Move piece, Make slot emptey after movment, Place piece in new spot
        {
            _gameBoard[piece.Position.Row, piece.Position.Column] = null;      
            _gameBoard[position.Row, position.Column] = piece;
            piece.Position = position;
        }

        public void RemovePiece(GamePiece piece)                        //Emptey slot
        {
            _gameBoard[piece.Position.Row, piece.Position.Column] = null;
            piece.IsDead = true;
        }

        public Weapon GetRandomWeapon()                                 //0,1,2 stands for Rock, Paper, Scissors
        {
            return (Weapon)_rand.Next(3);
        }

        public void GameOver(Player loser)
        {
            if (loser == _player1) _winner = _player2;
            else if (loser == _player2) _winner = _player1;
            else _winner = null;
            _isGameOver = true;
        }

        private bool CheckForTie()                                      //In case no one left on board
        {
            for (int row = 0; row < _gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < _gameBoard.GetLength(1); col++)
                {
                    if (_gameBoard[row, col] != null && _gameBoard[row, col].IsMovable) return false;
                }
            }
            return true;
        }

        private Position GetDesiredPosition(Position startingPosition, Direction directionToMove)
        {
            int newX = startingPosition.Column;
            int newY = startingPosition.Row;
            switch (directionToMove)
            {
                case Direction.Up:
                    newY--;
                    break;
                case Direction.Right:
                    newX++;
                    break;
                case Direction.Down:
                    newY++;
                    break;
                case Direction.Left:
                    newX--;
                    break;
                default:
                    break;
            }

            return new Position { Column = newX, Row = newY };
        }

        private bool IsMoveInGameBounds(Position position)
        {
            return (
                position.Column >= 0 &&
                position.Column < _gameBoard.GetLength(0) &&
                position.Row >= 0 &&
                position.Row < _gameBoard.GetLength(1)
            );
        }
    }
}

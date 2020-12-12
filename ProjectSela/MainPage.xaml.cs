using GameLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ProjectSela
{
    // TODO: Add proper ctors and acccess modifiers to all classes
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RockPaperScissorsGame _game = new RockPaperScissorsGame();
        private Random _rand = new Random();
        private Button[,] _buttons = new Button[RockPaperScissorsGame.BOARD_SIZE, RockPaperScissorsGame.BOARD_SIZE];
        private GamePiece _selectedPiece = null;

        public MainPage()
        {
            InitializeComponent();
            InitButtonsArray();
            StartNewGame();
        }

        private void InitButtonsArray()
        {
            for (int i = 0; i < RockPaperScissorsGame.BOARD_SIZE; i++)
            {
                for (int j = 0; j < RockPaperScissorsGame.BOARD_SIZE; j++)
                {
                    Button btn = new Button();
                    btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    btn.VerticalAlignment = VerticalAlignment.Stretch;
                    btn.Background = new SolidColorBrush(i % 2 == j % 2 ? Colors.Black : Colors.White);     //Chess style coloring 
                    btn.BorderThickness = new Thickness(2);
                    btn.Click += OnTileClick;
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);
                    _buttons[i, j] = btn;
                    gridGameBoard.Children.Add(btn);
                }
            }
        }

        private void StartNewGame()
        {
            Player player1 = InitPlayer(Colors.Red, "Player1", false);
            Player player2 = InitPlayer(Colors.Blue, "Player2", true);
            _game.InitNewGame(player1, player2);
            DrawBoard();
        }

        private Player InitPlayer(Color color, string name, bool isReverse)             //Setting Board and position of peices
        {
            Player player = new Player { Color = color, Name = name };

            int flagColumn = _rand.Next(RockPaperScissorsGame.BOARD_SIZE);
            int trapColumn = _rand.Next(RockPaperScissorsGame.BOARD_SIZE);

            GamePiece[] pieces = new GamePiece[RockPaperScissorsGame.BOARD_SIZE * 2];
            for (int i = 0; i < pieces.Length; i++)
            {
                int col = i % RockPaperScissorsGame.BOARD_SIZE;
                int rowOffset = i / RockPaperScissorsGame.BOARD_SIZE;
                int row = isReverse
                    ? RockPaperScissorsGame.BOARD_SIZE - 1 - rowOffset
                    : rowOffset;

                if (rowOffset == 0 && col == flagColumn)
                {
                    pieces[i] = new Flag();
                }
                else if (rowOffset == 1 && col == trapColumn)
                {
                    pieces[i] = new Trap();
                }
                else
                {
                    pieces[i] = new Soldier { Weapon = _game.GetRandomWeapon() };
                }

                pieces[i].Game = _game;
                pieces[i].Owner = player;
                pieces[i].Position = new Position { Column = col, Row = row };
            }
            player.Pieces = pieces;
            return player;
        }

        private void OnTileClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Opacity < 1) return;

            int row = Grid.GetRow(btn);
            int col = Grid.GetColumn(btn);
            GamePiece piece = _game._gameBoard[row, col];

            // Selecting a piece
            if (_selectedPiece == null)
            {
                if (piece == null) return;
                if (piece.Owner.Name != _game._currentPlayer.Name) return;
                if (piece.IsMovable)
                    _selectedPiece = piece;
            }
            // Making a move (or reselectinga piece)
            else
            {
                if (piece != null && piece.Owner.Name == _game._currentPlayer.Name)
                {
                    _selectedPiece = piece;
                }
                else
                {
                    // Math.Abs gives the absolute value for a number (always positive)
                    int deltaX = col - _selectedPiece.Position.Column;
                    int deltaY = row - _selectedPiece.Position.Row;
                    int distanceX = Math.Abs(deltaX);
                    int distanceY = Math.Abs(deltaY);

                    if (distanceX > 1 || distanceY > 1 || distanceX + distanceY > 1) return; //Can't move more than 1 spot at a time, or diagonally

                    Direction direction;                            //Possible movement directions 
                    if (deltaX > 0)
                        direction = Direction.Right;
                    else if (deltaX < 0)
                        direction = Direction.Left;
                    else if (deltaY > 0)
                        direction = Direction.Down;
                    else
                        direction = Direction.Up;

                    if (!_game.MakeMove(_selectedPiece, direction)) return;

                    _selectedPiece = null;
                }
            }
            DrawBoard();
        }

        private void DrawBoard()                                                                      //Draw and place type if pieces
        {
            txtCurrentPlayer.Text = $"It's currently {_game._currentPlayer.Name}'s turn";
            txtCurrentPlayer.Foreground = new SolidColorBrush(_game._currentPlayer.Color);

            for (int i = 0; i < _game._gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < _game._gameBoard.GetLength(1); j++)
                {
                    _buttons[i, j].Background = new SolidColorBrush(i % 2 == j % 2 ? Colors.Black : Colors.White);
                    _buttons[i, j].BorderBrush = null;
                    _buttons[i, j].Opacity = 0.75;

                    if (_selectedPiece != null && _selectedPiece.IsMovable)
                    {
                        int offsetY = Math.Abs(_selectedPiece.Position.Row - i);
                        int offsetX = Math.Abs(_selectedPiece.Position.Column - j);
                        if (offsetX + offsetY == 1)
                        {
                            _buttons[i, j].Opacity = 1;
                            if (_game._gameBoard[i, j] == null || _game._gameBoard[i, j].Owner != _selectedPiece.Owner)
                                _buttons[i, j].BorderBrush = new SolidColorBrush(Colors.Yellow);
                        }
                    }

                    GamePiece piece = _game._gameBoard[i, j];
                    if (piece == null)
                    {
                        _buttons[i, j].Content = null;
                    }
                    else
                    {
                        _buttons[i, j].Background = new SolidColorBrush(piece.Owner.Color);
                        if (piece == _selectedPiece)
                        {
                            _buttons[i, j].BorderBrush = new SolidColorBrush(Colors.LimeGreen);
                        }
                        if (piece.Owner == _game._currentPlayer)
                            _buttons[i, j].Opacity = 1;

                        if (piece is Trap) _buttons[i, j].Content = CreateImage("Trap");
                        else if (piece is Flag) _buttons[i, j].Content = CreateImage("Flag");
                        else
                        {
                            Soldier soldier = (Soldier)piece;
                            if (soldier.Weapon == Weapon.Rock) _buttons[i, j].Content = CreateImage("Rock");
                            else if (soldier.Weapon == Weapon.Paper) _buttons[i, j].Content = CreateImage("Paper");
                            else if (soldier.Weapon == Weapon.Scissors) _buttons[i, j].Content = CreateImage("Scissors");
                        }
                    }
                }
            }
            if (_game._isGameOver)
                new MessageDialog(_game._winner == null ? "It's a Tie!" : $"{_game._winner.Name} Won!", "Game Over!").ShowAsync();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private Image CreateImage(string name)
        {
            Image image = new Image();
            image.Margin = new Thickness(10);
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            image.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{name}.jpg"));
            image.Stretch = Stretch.Fill;
            return image;
        }
    }
}

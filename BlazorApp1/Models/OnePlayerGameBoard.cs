using BlazorApp1.Enums;

namespace BlazorApp1.Models
{
    public class OnePlayerGameBoard : GameBoard
    {
        public OnePlayerGameBoard()
            : base(3) { }

        public override string GetIcon(PieceStyle style)
        {
            return style switch
            {
                PieceStyle.X => PlayerOne.Icon!,
                PieceStyle.O => PlayerAi.Icon!,
                _ => string.Empty
            };
        }

        public override string GetColor(PieceStyle style)
        {
            return style switch
            {
                PieceStyle.X => PlayerOne.Color!,
                PieceStyle.O => PlayerAi.Color!,
                _ => string.Empty
            };
        }

        protected override Player GetOtherPlayer(Player currentPlayer)
        {
            if (currentPlayer == PlayerOne)
            {
                return PlayerAi;
            }
            else
            {
                return PlayerOne;
            }
        }

        public Player PlayerAi { get; } = new Player() { PieceStyle = PieceStyle.O, Name = "CPU", Icon = "O", Color = "Blue" };

        //Given the coordinates of the space that was clicked...
        public override void PieceClicked(int x, int y)
        {
            //If the game is complete, do nothing
            if (IsGameComplete)
            {
                return;
            }

            //If the space is not already claimed...
            GamePiece clickedSpace = Board[x, y];
            if (clickedSpace.Style == PieceStyle.Blank)
            {
                //Set the marker to the current turn marker (X or O), then make it the other player's turn
                clickedSpace.Style = PlayerOne.PieceStyle;

                //This is equivalent to: if currently X's turn, 
                // make it O's turn, and vice-versa
                CurrentTurn = PlayerAi;

                MakeMove();
            }
        }

        private void MakeMove()
        {
            if (IsGameComplete)
            {
                //If the game is complete, do nothing
                return;
            }

            var mid = Board[1, 1];
            if (mid.Style == PieceStyle.Blank)
            {
                // always go middle if possible
                mid.Style = PlayerAi.PieceStyle;
                CurrentTurn = PlayerOne;
            }
            else if (NextMoveToWin(PlayerAi.PieceStyle) is (int,int) aiMove)
            {
                // always win if possible
                GamePiece clickedSpace = Board[aiMove.Item1, aiMove.Item2];

                clickedSpace.Style = PlayerAi.PieceStyle;
                CurrentTurn = PlayerOne;
            }
            else if (NextMoveToWin(PlayerOne.PieceStyle) is (int, int) blockingMove)
            {
                // always block if possible
                GamePiece clickedSpace = Board[blockingMove.Item1, blockingMove.Item2];

                clickedSpace.Style = PlayerAi.PieceStyle;
                CurrentTurn = PlayerOne;
            }
            else if (NextMoveToSetupToWin(PlayerAi.PieceStyle) is (int, int) setupMove)
            {
                // win in 2 moves
                GamePiece clickedSpace = Board[setupMove.Item1, setupMove.Item2];

                clickedSpace.Style = PlayerAi.PieceStyle;
                CurrentTurn = PlayerOne;
            }
            else
            {
                for (int i = 0; i < _rowsAndCols; i++)
                {
                    for (int j = 0; j < _rowsAndCols; j++)
                    {
                        var piece = Board[i, j];
                        if (piece.Style == PieceStyle.Blank)
                        {
                            piece.Style = PlayerAi.PieceStyle;
                            CurrentTurn = PlayerOne;

                            return;
                        }
                    }
                }
            }
        }

        private static readonly List<List<(int, int)>> WinningCombos =
            [
                [
                    new (0,0),
                    new (0,1),
                    new (0,2)
                ],
                [
                    new (1,0),
                    new (1,1),
                    new (1,2)
                ],
                [
                    new (2,0),
                    new (2,1),
                    new (2,2)
                ],

                [
                    new (0,0),
                    new (1,0),
                    new (2,0)
                ],
                [
                    new (0,1),
                    new (1,1),
                    new (2,1)
                ],
                [
                    new (0,2),
                    new (1,2),
                    new (2,2)
                ],

                [
                    new (0,0),
                    new (1,1),
                    new (2,2)
                ],
                [
                    new (2,0),
                    new (1,1),
                    new (0,2)
                ]
            ];

        private (int, int)? NextMoveToSetupToWin(PieceStyle pieceStyle)
        {
            var state = GetBoardState(pieceStyle);

            List<(int, int)> listOfAi = state.Item1;
            List<(int, int)> listOfPlayer = state.Item2;

            List<List<(int, int)>> combos = [];
            foreach (var wc in WinningCombos)
            {
                combos.Add([.. wc]);
            }

            var nextMoveToWin = combos.Where(c => listOfAi.Count(a => c.Contains(a)) == 1).ToList();

            if (nextMoveToWin is not null && nextMoveToWin.Count > 0)
            {
                foreach (var ai in listOfAi)
                {
                    nextMoveToWin.ForEach(w => w.Remove(ai));
                }
                foreach (var pl in listOfPlayer)
                {
                    nextMoveToWin.ForEach(w => w.Remove(pl));
                }

                if (nextMoveToWin?.Any() == true)
                {
                    var a = nextMoveToWin.FirstOrDefault(w => w.Count > 0);

                    if (a?.Any() == true)
                    {
                        var setupMove = a?.FirstOrDefault();

                        return setupMove;
                    }
                }
            }

            return null;
        }

        private (List<(int, int)>,List<(int, int)>) GetBoardState(PieceStyle pieceStyle)
        {
            List<(int, int)> listOfAi = [];
            List<(int, int)> listOfPlayer = [];

            for (int i = 0; i < _rowsAndCols; i++)
            {
                for (int j = 0; j < _rowsAndCols; j++)
                {
                    var piece = Board[i, j];

                    switch (piece.Style)
                    {
                        case PieceStyle.X:
                            if (pieceStyle == PieceStyle.X)
                            {
                                listOfAi.Add((i, j));
                            }
                            else
                            {
                                listOfPlayer.Add((i, j));
                            }
                            break;
                        case PieceStyle.O:
                            if (pieceStyle == PieceStyle.O)
                            {
                                listOfAi.Add((i, j));
                            }
                            else
                            {
                                listOfPlayer.Add((i, j));
                            }
                            break;
                        case PieceStyle.Blank:
                        default:
                            break;
                    }
                }
            }

            return (listOfAi, listOfPlayer);
        }

        private (int, int)? NextMoveToWin(PieceStyle pieceStyle)
        {
            var state = GetBoardState(pieceStyle);

            List<(int, int)> listOfAi = state.Item1;
            List<(int, int)> listOfPlayer = state.Item2;

            List<List<(int, int)>> combos = [];
            foreach(var wc in WinningCombos)
            {
                combos.Add([.. wc]);
            }

            var nextMoveToWin = combos.Where(c => listOfAi.Count(a => c.Contains(a)) == 2).ToList();

            if (nextMoveToWin is not null && nextMoveToWin.Count > 0)
            {
                foreach (var ai in listOfAi)
                {
                    nextMoveToWin.ForEach(w => w.Remove(ai));
                }
                foreach (var pl in listOfPlayer)
                {
                    nextMoveToWin.ForEach(w => w.Remove(pl));
                }

                if (nextMoveToWin?.Any() == true)
                {
                    var a = nextMoveToWin.FirstOrDefault(w => w.Count > 0);

                    if (a?.Any() == true)
                    {
                        var winningMove = a?.FirstOrDefault();

                        return winningMove;
                    }
                }
            }

            return null;
        }
    }
}
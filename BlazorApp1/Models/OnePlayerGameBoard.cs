using BlazorApp1.Enums;

namespace BlazorApp1.Models
{
    public class OnePlayerGameBoard : GameBoard
    {
        public OnePlayerGameBoard()
            : base(3) { }

        private const PieceStyle _aiPieceStyle = PieceStyle.O;

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
                clickedSpace.Style = PieceStyle.X;

                //This is equivalent to: if currently X's turn, 
                // make it O's turn, and vice-versa
                CurrentTurn = PieceStyle.O;

                MakeMove();
            }
        }

        private void MakeMove()
        {
            //If the game is complete, do nothing
            if (IsGameComplete)
            {
                return;
            }

            var mid = Board[1, 1];
            if (mid.Style == PieceStyle.Blank)
            {
                // always go middle if possible
                mid.Style = PieceStyle.O;
                CurrentTurn = PieceStyle.X;
                return;
            }

            var move = NextMoveToWin();
            if (move.HasValue)
            {
                // always win if possible
                GamePiece clickedSpace = Board[move.Value.Item1, move.Value.Item2];

                clickedSpace.Style = PieceStyle.O;
                CurrentTurn = PieceStyle.X;

                return;
            }
            else
            {
                // always block if possible

                for (int i = 0; i < _rowsAndCols; i++)
                {
                    for (int j = 0; j < _rowsAndCols; j++)
                    {
                        var piece = Board[i, j];
                        if (piece.Style == PieceStyle.Blank)
                        {
                            piece.Style = PieceStyle.O;
                            CurrentTurn = PieceStyle.X;

                            return;
                        }
                    }
                }
            }
        }

        private static List<List<(int, int)>> WinningCombos =
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


        private void DoNothing()
        {

        }

        private (int, int)? NextMoveToWin()
        {
            List<(int, int)> listOfAi = [];
            List<(int, int)> listOfPlayer = [];

            for (int i = 0; i < _rowsAndCols; i++)
            {
                for (int j = 0; j < _rowsAndCols; j++)
                {
                    var piece = Board[i,j];

                    switch (piece.Style)
                    {
                        case PieceStyle.Blank:
                            DoNothing();
                            break;
                        case PieceStyle.X:
                            if(_aiPieceStyle == PieceStyle.X)
                            {
                                listOfAi.Add((i, j));
                            }
                            else
                            {
                                listOfPlayer.Add((i, j));
                            }
                            break;
                        case PieceStyle.O:
                            if (_aiPieceStyle == PieceStyle.X)
                            {
                                listOfPlayer.Add((i, j));
                            }
                            else
                            {
                                listOfAi.Add((i, j));
                            }
                            break;
                        default:
                            DoNothing();
                            break;
                    };
                }
            }

            var nextMoveToWin = WinningCombos.ToList().Where(c => listOfAi.Count(a => c.Contains(a)) == 2).ToList();

            if (nextMoveToWin is not null && nextMoveToWin.Any())
            {
                listOfAi.ForEach(a => nextMoveToWin.ForEach(w => w.Remove(a)));
                listOfPlayer.ForEach(b => nextMoveToWin.ForEach(w => w.Remove(b)));

                var a = nextMoveToWin.FirstOrDefault();
                var b = a?.FirstOrDefault();

                var winningMove = b;
                return winningMove;
            }

            return null;
        }
    }
}
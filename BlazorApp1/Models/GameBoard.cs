using BlazorApp1.Enums;

namespace BlazorApp1.Models
{
    public class GameBoard
    {
        private const int _rowsAndCols = 3;

        public GamePiece[,] Board { get; private set; } = new GamePiece[_rowsAndCols, _rowsAndCols];

        public PieceStyle CurrentTurn = PieceStyle.X;

        public bool IsGameComplete => GetWinner() != null || IsADraw();

        public GameBoard()
        {
            Reset();
        }

        public void Reset()
        {
            int max = _rowsAndCols - 1;
            //Populate the Board with blank pieces
            for (int i = 0; i <= max; i++)
            {
                for (int j = 0; j <= max; j++)
                {
                    Board[i, j] = new GamePiece();
                }
            }
        }

        //Given the coordinates of the space that was clicked...
        public void PieceClicked(int x, int y)
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
                clickedSpace.Style = CurrentTurn;
                SwitchTurns();
            }
        }

        private void SwitchTurns()
        {
            //This is equivalent to: if currently X's turn, 
            // make it O's turn, and vice-versa
            CurrentTurn = CurrentTurn == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        }

        public bool IsADraw()
        {
            int pieceBlankCount = 0;

            //Count all the blank spaces. If the count is 0, this is a draw.
            for (int i = 0; i < _rowsAndCols; i++)
            {
                for (int j = 0; j < _rowsAndCols; j++)
                {
                    pieceBlankCount = Board[i, j].Style == PieceStyle.Blank
                                        ? pieceBlankCount + 1
                                        : pieceBlankCount;
                }
            }

            return pieceBlankCount == 0;
        }

        public WinningPlay? GetWinner()
        {
            WinningPlay? winningPlay = null;

            for (int i = 0; i < _rowsAndCols; i++)
            {
                for (int j = 0; j < _rowsAndCols; j++)
                {
                    foreach (EvaluationDirection evalDirection in (EvaluationDirection[])Enum.GetValues(typeof(EvaluationDirection)))
                    {
                        winningPlay = EvaluatePieceForWinner(i, j, evalDirection);
                        if (winningPlay != null)
                        {
                            return winningPlay;
                        }
                    }
                }
            }

            return winningPlay;

        }

        private WinningPlay? EvaluatePieceForWinner(int i, int j, EvaluationDirection dir)
        {
            GamePiece currentPiece = Board[i, j];
            if (currentPiece.Style == PieceStyle.Blank)
            {
                return null;
            }

            int inARow = 1;
            int iNext = i;
            int jNext = j;

            var winningMoves = new List<string>();

            while (inARow < _rowsAndCols)
            {
                switch (dir)
                {
                    case EvaluationDirection.Up:
                        jNext -= 1;
                        break;
                    case EvaluationDirection.UpRight:
                        iNext += 1;
                        jNext -= 1;
                        break;
                    case EvaluationDirection.Right:
                        iNext += 1;
                        break;
                    case EvaluationDirection.DownRight:
                        iNext += 1;
                        jNext += 1;
                        break;
                }

                if (iNext < 0 || iNext >= _rowsAndCols || jNext < 0 || jNext >= _rowsAndCols)
                {
                    break;
                }

                if (Board[iNext, jNext].Style == currentPiece.Style)
                {
                    winningMoves.Add($"{iNext},{jNext}");
                    inARow++;
                }
                else
                {
                    return null;
                }
            }

            if (inARow >= _rowsAndCols)
            {
                winningMoves.Add($"{i},{j}");

                return new WinningPlay()
                {
                    WinningMoves = winningMoves,
                    WinningStyle = currentPiece.Style,
                    WinningDirection = dir,
                };
            }

            return null;
        }

        public string GetGameCompleteMessage()
        {
            var winningPlay = GetWinner();
            return winningPlay != null ? $"{winningPlay.WinningStyle} Wins!" : "Draw!";
        }

        public bool IsGamePieceAWinningPiece(int i, int j)
        {
            var winningPlay = GetWinner();
            return winningPlay?.WinningMoves?.Contains($"{i},{j}") ?? false;
        }
    }
}
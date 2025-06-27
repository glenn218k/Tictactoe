using BlazorApp1.Enums;

namespace BlazorApp1.Models
{
    public abstract class GameBoard
    {
        protected readonly int _rowsAndCols;

        public Player PlayerOne { get; } = new Player() { PieceStyle = PieceStyle.X, Name = "Player One", Icon = "X", Color = "Red" };

        public GamePiece[,] Board { get; private set; }

        public Player CurrentTurn { get; set; }

        public bool IsGameComplete => GetWinner() != null || IsADraw();

        public abstract string GetIcon(PieceStyle style);

        public abstract string GetColor(PieceStyle style);

        protected GameBoard(int rowsAndCols)
        {
            Board = new GamePiece[_rowsAndCols, _rowsAndCols];
            _rowsAndCols = rowsAndCols;
            Reset();
        }

        public void SetPlayerOneColor(string color)
        {
            PlayerOne.Color = color;
        }

        public void SetPlayerOneIcon(string icon)
        {
            PlayerOne.Icon = icon;
        }

        public void Reset()
        {
            CurrentTurn = PlayerOne;
            Board = new GamePiece[_rowsAndCols, _rowsAndCols];
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
        public abstract void PieceClicked(int x, int y);

        public bool IsADraw()
        {
            int pieceBlankCount = 0;

            //Count all the blank spaces. If the count is 0, this is a draw.
            for (int i = 0; i < _rowsAndCols; i++)
            {
                for (int j = 0; j < _rowsAndCols; j++)
                {
                    if (Board[i, j].Style == PieceStyle.Blank)
                    {
                        pieceBlankCount++;
                    }
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
                var other = GetOtherPlayer(CurrentTurn);

                return new WinningPlay()
                {
                    WinningMoves = winningMoves,
                    WinningStyle = other.PieceStyle,
                    WinningDirection = dir,
                    WinningName = other.Name,
                    WinningIcon = other.Icon
                };
            }

            return null;
        }

        protected abstract Player GetOtherPlayer(Player currentPlayer);

        public string GetGameCompleteMessage()
        {
            var winningPlay = GetWinner();
            return winningPlay != null ? $"{winningPlay.WinningName} ({winningPlay.WinningIcon}) Wins!" : "Cat's Game!";
        }

        public bool IsGamePieceAWinningPiece(int i, int j)
        {
            var winningPlay = GetWinner();
            return winningPlay?.WinningMoves?.Contains($"{i},{j}") ?? false;
        }
    }
}
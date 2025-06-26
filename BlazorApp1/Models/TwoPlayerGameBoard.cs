using BlazorApp1.Enums;

namespace BlazorApp1.Models
{
    public class TwoPlayerGameBoard : GameBoard
    {
        public static Player PlayerTwo { get; } = new Player() { PieceStyle = PieceStyle.O, Name = "Player Two", Icon = "O" };

        public TwoPlayerGameBoard()
            : base(3) { }

        public override string GetIcon(PieceStyle style)
        {
            return style switch
            {
                PieceStyle.X => PlayerOne.Icon ?? PieceStyle.X.ToString(),
                PieceStyle.O => PlayerTwo.Icon ?? PieceStyle.O.ToString(),
                _ => string.Empty
            };
        }

        protected override Player GetOtherPlayer(Player currentPlayer)
        {
            if (currentPlayer == PlayerOne)
            {
                return PlayerTwo;
            }
            else
            {
                return PlayerOne;
            }
        }

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
                clickedSpace.Style = CurrentTurn.PieceStyle;

                //This is equivalent to: if currently X's turn, 
                // make it O's turn, and vice-versa
                CurrentTurn = CurrentTurn == PlayerOne ? PlayerTwo : PlayerOne;
            }
        }
    }
}
namespace Simple_CSharp_Games.Models
{
    public class TicTacToeBoard
    {
        // How will I represent the board? Tuple with named values
        public (string one, string two, string three, string four, string five, string six, string seven, string eight, string nine) board = ("", "", "", "", "", "", "", "", "");
        public string boardState { get; set; } = "open"; // open, full, won 

        public void ResetBoard()
        {
            board = ("", "", "", "", "", "", "", "", "");
            boardState = "open";
        }

        public void UpdateBoard(string squareNumber, string playerMark)
        {
            // Update the square value with players mark 
            switch (squareNumber)
            {
                case "1":
                    board.one = playerMark;
                    break;
                case "2":
                    board.two = playerMark;
                    break;
                case "3":
                    board.three = playerMark;
                    break;
                case "4":
                    board.four = playerMark;
                    break;
                case "5":
                    board.five = playerMark;
                    break;
                case "6":
                    board.six = playerMark;
                    break;
                case "7":
                    board.seven = playerMark;
                    break;
                case "8":
                    board.eight = playerMark;
                    break;
                case "9":
                    board.nine = playerMark;
                    break;
            }

        }

        public bool IsSquareFree(string squareNumber)
        {
            // Check the square number and access the corresponding tuple item  
            switch (squareNumber)
            {
                case "1":
                    return board.one == "";
                case "2":
                    return board.two == "";
                case "3":
                    return board.three == "";
                case "4":
                    return board.four == "";
                case "5":
                    return board.five == "";
                case "6":
                    return board.six == "";
                case "7":
                    return board.seven == "";
                case "8":
                    return board.eight == "";
                case "9":
                    return board.nine == "";
                default:
                    return false;
            }
        }

        public void CheckBoard()
        {

            // destructor the tuple to get the values
            var (one, two, three, four, five, six, seven, eight, nine) = board;

            // check for a winner
            if (one != "" && one == two && one == three)
            {
                DisplayWinner(one);
                boardState = "won";
                return;
            }
            if (four != "" && four == five && four == six)
            {
                DisplayWinner(four);
                boardState = "won";
                return;
            }
            if (seven != "" && seven == eight && seven == nine)
            {
                DisplayWinner(seven);
                boardState = "won";
                return;
            }
            if (one != "" && one == four && one == seven)
            {
                DisplayWinner(one);
                boardState = "won";
                return;
            }
            if (two != "" && two == five && two == eight)
            {
                DisplayWinner(two);
                boardState = "won";
                return;
            }
            if (three != "" && three == six && three == nine)
            {
                DisplayWinner(three);
                boardState = "won";
                return;
            }
            if (one != "" && one == five && one == nine)
            {
                DisplayWinner(one);
                boardState = "won";
                return;
            }
            if (three != "" && three == five && three == seven)
            {
                DisplayWinner(three);
                boardState = "won";
                return;
            }

            // check for a draw
            if (one != "" && two != "" && three != "" && four != "" && five != "" && six != "" && seven != "" && eight != "" && nine != "")
            {
                Console.WriteLine("The board is full, Game is a draw!");
                boardState = "draw";
                return;
            }

            // keep playing
            Console.WriteLine("No winner yet, keep playing!");
            boardState = "open";
            return;

        }

        internal void DisplayWinner(string winner)
        {
            Console.WriteLine($"{winner} is the winner!");
        }
    }
}

using System.Drawing;
using System.Numerics;

namespace Simple_CSharp_Games.Models
{
    public class FountainGameState
    {
        public bool isPlayerAlive { get; set; } = true;
        public bool IsFountainOn { get; set; } = false;
        public int CurrentRow { get; set; }
        public int CurrentColumn { get; set; }
        public bool IsGameOver => !isPlayerAlive || (IsFountainOn && CurrentRow == 0 && CurrentColumn == 0);
        public Board Board { get; set; }
        public FountainGameState(string boardSize) 
        {
            Board = new Board(boardSize);
            Board.LoadBoard();

            // Set player starting position
            CurrentRow = 0;
            CurrentColumn = 0;
        }

        // Player Actions for the game
        // move player to a new position
        // sense what is happening in the current room
        // enable to fountain in the right room
        // check if the game is over
    }

    public class Game
    {
        public void Start()
        {

            DisplayGameExplanation();

            var board = CreateBoard();
            var player = new Player();



            while (player.IsAlive)
            {
                board.DisplayBoard(player);

                board.MovePlayer(player);

                var didPlayerWin = (player.CurrentRow == 0 && player.CurrentColumn == 0) && board.IsFountainOn == true;

                if (didPlayerWin == true)
                {
                    board.PlayerWon(player);
                    break;
                }
            }

        }

        public void DisplayGameExplanation()
        {
            Console.ForegroundColor = ConsoleColor.Magenta; // narrative items in magenta

            Console.WriteLine(@"
            Welcome to the Fountain of Objects!
            You are in a dark cavern system with many rooms.
            Your goal is to find the Fountain of Objects and return to the entrance.
            Be careful, there are dangers lurking in the darkness!
            Good luck!
            ");

            Console.ResetColor();
        }

        public Board CreateBoard()
        {
            string? size = null;

            Console.ForegroundColor = ConsoleColor.White; // descriptive text in white

            Console.Write("Please select a board size: small, medium, or large: ");

            Console.ForegroundColor = ConsoleColor.Cyan;

            size = Console.ReadLine()?.ToLower();

            Console.ResetColor();

            if (String.IsNullOrWhiteSpace(size) || (size != "small" && size != "medium" && size != "large"))
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Must enter a valid board size");

                return CreateBoard();
            }

            Board board = new Board(size);

            board.LoadBoard();

            return board;
        }
    }

    public class Board
    {
    public string Size { get; set; }

    public int Rows { get; set; }

    public int Columns { get; set; }

    public IRoom[,] Rooms { get; set; }

    public bool IsFountainOn { get; set; } = false;

    public bool QuestComplete { get; set; } = false;

    public Board(string size)
    {
        if (size == "small")
        {
            Rooms = new IRoom[4, 4];
            Size = "Small";
            Rows = 4;
            Columns = 4;
        }
        else if (size == "medium")
        {
            Rooms = new IRoom[6, 6];
            Size = "Medium";
            Rows = 6;
            Columns = 6;
        }
        else
        {
            Rooms = new IRoom[8, 8];
            Size = "Large";
            Rows = 8;
            Columns = 8;
        }
    }

    public void LoadBoard()
    {
        Rooms[0, 0] = new Entrance(0, 0, "Entrance");

        Rooms[0, 2] = new Fountain(0, 2, "Fountain");

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (Rooms[i, j] == null)
                {
                    Rooms[i, j] = new Empty(i, j, "Empty");
                }
            }
        }

    }

    public void DisplayBoard(Player player)
    {
        Console.WriteLine($"\n{Size} Game Board\n");

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                var room = Rooms[i, j];
                var isPlayerLocation = i == player.CurrentRow && j == player.CurrentColumn;

                if (room != null && room == Rooms[0, 0])
                {
                    Console.Write($"{room.Type.ToString()[0]} ");
                }
                else if (isPlayerLocation)
                {
                    Console.Write("P ");
                }
                else
                {
                    Console.Write(". ");
                }
            }
            Console.WriteLine();
            Console.WriteLine(String.Concat(Enumerable.Repeat("--", Rows)));
        }
    }

    public bool IsAValidMove(int row, int column)
    {
        if (row < 0 || row >= Rows || column < 0 || column >= Columns)
        {
            return false;
        }

        return true;
    }

    public void InvalidMove(string direction)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"You can not move in that direction: {direction}");
    }

    public void MovePlayer(Player player)
    {

        string? input = player.GetInput();

        if (input == "move north")
        {
            if (IsAValidMove(player.CurrentRow - 1, player.CurrentColumn))
            {
                player.CurrentRow -= 1; // update player position
                SenseRoom(player); // sense the room
                return;
            }

            InvalidMove(input);
            MovePlayer(player);
            return;
        }
        else if (input == "move south")
        {
            if (IsAValidMove(player.CurrentRow + 1, player.CurrentColumn))
            {
                player.CurrentRow += 1;
                SenseRoom(player);
                return;
            }

            InvalidMove(input);
            MovePlayer(player);
        }
        else if (input == "move east")
        {
            if (IsAValidMove(player.CurrentRow, player.CurrentColumn + 1))
            {
                player.CurrentColumn += 1;
                SenseRoom(player);
                return;
            }

            InvalidMove(input);
            MovePlayer(player);
            return;
        }
        else if (input == "move west")
        {
            if (IsAValidMove(player.CurrentRow, player.CurrentColumn - 1))
            {
                player.CurrentColumn -= 1;
                SenseRoom(player);
                return;
            }

            InvalidMove(input);
            MovePlayer(player);
            return;
        }
        else if (input == "enable fountain")
        {
            EnableFountain(player);
            return;
        }
        else if (input == "disable fountain")
        {
            DisableFountain(player);
            return;
        }
        else if (input == "quit")
        {
            PlayerQuit(player);
        }
    }

    public void SenseRoom(Player player)
    {
        // Sense the room
        Rooms[player.CurrentRow, player.CurrentColumn].Sense(IsFountainOn);
    }

    public void EnableFountain(Player player)
    {

        var currentRoom = Rooms[player.CurrentRow, player.CurrentColumn];


        if (currentRoom is IActivatableRoom activatableRoom)
        {
            IsFountainOn = true;
            activatableRoom.Enable();
            return;
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("You can not interact with the fountain because it is not in this room.");
        Console.ResetColor();
        return;
    }

    public void DisableFountain(Player player)
    {
        var currentRoom = Rooms[player.CurrentRow, player.CurrentColumn];

        if (currentRoom is IActivatableRoom activatableRoom)
        {
            IsFountainOn = false;
            activatableRoom.Disable();
            return;
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("You can not interact with the fountain because it is not in this room.");
        Console.ResetColor();
        return;
    }

    public void PlayerWon(Player player)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nYou Win! Thanks For Playing!");
        Console.ResetColor();
        player.IsAlive = false; // end the game
        return;

    }

    public void PlayerLost(Player player)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nYou Lost! Game Over!");
        Console.ResetColor();
        player.IsAlive = false;
    }

    void PlayerQuit(Player player)
    {
        Console.ForegroundColor = ConsoleColor.Red; // text in red
        Console.WriteLine("\nThanks for playing, Quitter!");
        Console.ResetColor();
        player.IsAlive = false;
        return;
    }

}

    public class Empty : IRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }

        public Empty(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }
        public void Sense(bool isFountainOn = false)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("You sense nothing but the unnatural darkness, this room is empty!");
            Console.ResetColor();
            return;
        }
    }

    public class Entrance : IRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }
        public Entrance(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }

        public void Sense(bool isFountainOn)
        {
            if (isFountainOn)
            {
                Console.ForegroundColor = ConsoleColor.Yellow; // entrance text in yellow
                Console.WriteLine("The Fountain of Objects has been reactivated, and you have escaped with your life!");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("You see light coming from the cavern entrance.");
            Console.ResetColor();
            return;
        }

    }

    public class Fountain : IActivatableRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }
        public Fountain(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }

        public void Sense(bool isFountainOn)
        {
            if (isFountainOn)
            {
                Console.ForegroundColor = ConsoleColor.Blue; // fountain text in blue
                Console.WriteLine("You here rushing waters from the Fountain of Objects, it has been reactivated!");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("You hear water dripping in this room. The fountain of Objects is here!");
            Console.ResetColor();
            return;
        }

        public void Enable()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("The Fountain of Objects has been reactivated!");
            Console.ResetColor();
            return;
        }

        public void Disable()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("The Fountain of Objects has been DeActivated!");
            Console.ResetColor();
            return;
        }
    }

    public class Player
    {
        public bool IsAlive { get; set; } = true;
        public int CurrentRow { get; set; }
        public int CurrentColumn { get; set; }

        public string Location => $"(Row = {CurrentRow}, Column = {CurrentColumn})";

        public string GetInput()
        {
            Console.ForegroundColor = ConsoleColor.White;
            string prompt = "\nWhat do you want to do? ";

            Console.Write(prompt);

            Console.ForegroundColor = ConsoleColor.Cyan;
            string? userInput = Console.ReadLine()?.ToLower();
            Console.ResetColor();

            if (string.IsNullOrWhiteSpace(userInput) ||
                userInput != "move north" &&
                userInput != "move south" &&
                userInput != "move east" &&
                userInput != "move west" &&
                userInput != "help" &&
                userInput != "quit" &&
                userInput != "enable fountain" &&
                userInput != "disable fountain")
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Must enter a valid input");
                Console.ResetColor();
                return GetInput();
            }


            if (userInput == "help")
            {
                ShowHelp();
                return GetInput();
            }

            return userInput;
        }

        public void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("These are the actions that you can take.");
            Console.WriteLine("You can move through rooms: move north, move south, move east, move west.");
            Console.WriteLine("You can interact with the Fountain Of Objects: enable fountain, disable fountain.");
            Console.WriteLine("You can ask for help: help");
            Console.WriteLine("You can quit the game: quit");
            Console.ResetColor();
            return;
        }
    }

}

public interface IRoom
{
    int Row { get; set; }
    int Column { get; set; }
    string Location { get; }
    string Type { get; set; }
    void Sense(bool isFountainOn = false);
}

public interface IActivatableRoom : IRoom
{
    void Enable();
    void Disable();
}
using System.Drawing;
using System.Numerics;

namespace Simple_CSharp_Games.Models
{
    public class FountainGameState
    {
        public bool IsPlayerAlive { get; set; } = true;
        public bool IsFountainOn { get; set; } = false;
        public int CurrentRow { get; set; }
        public int CurrentColumn { get; set; }
        public bool IsGameOver => !IsPlayerAlive || (IsFountainOn && CurrentRow == 0 && CurrentColumn == 0);
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
        public string MovePlayer(string userInput)
        {
            string result = string.Empty;

            (CurrentRow, CurrentColumn, result, IsFountainOn, IsPlayerAlive) = Board.MovePlayer(userInput, CurrentRow, CurrentColumn, IsFountainOn, IsPlayerAlive);

            return result;
        }

        // check if the game is over
    }

    public class Board
    {
        public string Size { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public IRoom[,] Rooms { get; set; }

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

            LoadBaseBoard(); // Load the base board with Entrance and Empty rooms
            PlaceFountain(); // Randomly place the fountain in an empty room
            PlacePits(); // Place pits based on the board size
            MarkAdjacentRooms(); // Update rooms surrounding Pits to Adjacent type

        }

        public void LoadBaseBoard() // Entrance + Empty
        {
            Rooms[0, 0] = new Entrance(0, 0, "Entrance");

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
        public void PlaceFountain() // Random open tile
        {
            Random random = new Random();
            int row, column;
            do
            {
                row = random.Next(0, Rows);
                column = random.Next(0, Columns);
            } while (Rooms[row, column].Type != "Empty");
            Rooms[row, column] = new Fountain(row, column, "Fountain");

        }       
        public void PlacePits()  // One or more, safely placed
        {
            int numPits = Size switch
            {
                "Small" => 1,
                "Medium" => 2,
                "Large" => 3,
                _ => 1
            };

            Random random = new Random();
            int row, column;
            for (int i = 0; i < numPits; i++)
            {
                do
                {
                    row = random.Next(0, Rows);
                    column = random.Next(0, Columns);
                } while (Rooms[row, column].Type != "Empty" || (row == 0 && column == 0)); // Ensure not placing in entrance or already occupied
                Rooms[row, column] = new Pit(row, column, "Pit");
            }

        }
        public void MarkAdjacentRooms() // scan the entire board and laod adjacent rooms next ti Pit rooms
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (Rooms[row, col].Type == "Pit")
                    {
                        // Loop through all 8 surrounding tiles
                        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
                        {
                            for (int colOffset = -1; colOffset <= 1; colOffset++)
                            {
                                if (rowOffset == 0 && colOffset == 0) continue; // Skip the Pit itself

                                int adjacentRow = row + rowOffset;
                                int adjacentCol = col + colOffset;

                                if (IsOnBoard(adjacentRow, adjacentCol) &&
                                    Rooms[adjacentRow, adjacentCol].Type == "Empty")
                                {
                                    Rooms[adjacentRow, adjacentCol] = new Adjacent(adjacentRow, adjacentCol, "Adjacent");
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsOnBoard(int row, int col)  // ensures room is on board
        {
            return row >= 0 && row < Rows && col >= 0 && col < Columns;
        }

        public bool IsAValidMove(int row, int column)
        {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
            {
                return false;
            }

            return true;
        }

        public string InvalidMove(string direction)
        {
            return $"You can not move in that direction: {direction}";
        }

        public (int newRow, int newCol, string message, bool isFountainOn, bool isPlayerAlive) MovePlayer(string input, int CurrentRow, int CurrentColumn, bool IsFountainOn, bool IsPlayerAlive)
        {

            string result = string.Empty;

            if (input == "move north")
            {
                if (IsAValidMove(CurrentRow - 1, CurrentColumn))
                {
                    CurrentRow -= 1; // update player position
                    IRoom currentRoom = Rooms[CurrentRow, CurrentColumn];

                    if (currentRoom.InstantDeath)
                    {
                        IsPlayerAlive = false;
                        result = currentRoom.Sense();
                    }
                    else
                    {
                        result = SenseRoom(CurrentRow, CurrentColumn, IsFountainOn); // sense the room
                    }

                }
                else
                {

                    result = InvalidMove(input);
                }
            }
            else if (input == "move south")
            {
                if (IsAValidMove(CurrentRow + 1, CurrentColumn))
                {
                    CurrentRow += 1;
                    IRoom currentRoom = Rooms[CurrentRow, CurrentColumn];

                    if (currentRoom.InstantDeath)
                    {
                        IsPlayerAlive = false;
                        result = currentRoom.Sense();
                    }
                    else
                    {
                        result = SenseRoom(CurrentRow, CurrentColumn, IsFountainOn); // sense the room
                    }
                }
                else
                {

                    result = InvalidMove(input);
                }
            }
            else if (input == "move east")
            {
                if (IsAValidMove(CurrentRow, CurrentColumn + 1))
                {
                    CurrentColumn += 1;
                    IRoom currentRoom = Rooms[CurrentRow, CurrentColumn];

                    if (currentRoom.InstantDeath)
                    {
                        IsPlayerAlive = false;
                        result = currentRoom.Sense();
                    }
                    else
                    {
                        result = SenseRoom(CurrentRow, CurrentColumn, IsFountainOn); // sense the room
                    }
                }
                else
                {

                    result = InvalidMove(input);
                }

            }
            else if (input == "move west")
            {
                if (IsAValidMove(CurrentRow, CurrentColumn - 1))
                {
                    CurrentColumn -= 1;
                    IRoom currentRoom = Rooms[CurrentRow, CurrentColumn];

                    if (currentRoom.InstantDeath)
                    {
                        IsPlayerAlive = false;
                        result = currentRoom.Sense();
                    }
                    else
                    {
                        result = SenseRoom(CurrentRow, CurrentColumn, IsFountainOn); // sense the room
                    }
                }
                else
                {

                    result = InvalidMove(input);
                }

            }
            else if (input == "enable fountain")
            {
                (result, IsFountainOn) = EnableFountain(CurrentRow, CurrentColumn, IsFountainOn);

            }
            else if (input == "disable fountain")
            {
                (result, IsFountainOn) = DisableFountain(CurrentRow, CurrentColumn, IsFountainOn);
            }
            else if (input == "quit")
            {
                (result, IsPlayerAlive) = PlayerQuit(IsPlayerAlive);
            }
            return (CurrentRow, CurrentColumn, result, IsFountainOn, IsPlayerAlive);
        }

        // update method to take in IsPlayerAlive for Pit rooms
        public string SenseRoom(int CurrentRow, int CurrentColumn, bool IsFountainOn)
        {
            return Rooms[CurrentRow, CurrentColumn].Sense(IsFountainOn);
        }

        public (string message, bool isFountainOn) EnableFountain(int CurrentRow, int CurrentColumn, bool IsFountainOn)
        {

            string message = string.Empty;
            var currentRoom = Rooms[CurrentRow, CurrentColumn];


            if (currentRoom is IActivatableRoom activatableRoom)
            {
                IsFountainOn = true;
                message = activatableRoom.Enable();
            }
            else
            {
                message = "You can not interact with the fountain because it is not in this room.";
            }

            return (message, IsFountainOn);
        }

        public (string message, bool isFountainOn) DisableFountain(int CurrentRow, int CurrentColumn, bool IsFountainOn)
        {

            string message = string.Empty;
            var currentRoom = Rooms[CurrentRow, CurrentColumn];

            if (currentRoom is IActivatableRoom activatableRoom)
            {
                IsFountainOn = false;
                message = activatableRoom.Disable();
            }
            else
            {
                message = "You can not interact with the fountain because it is not in this room.";
            }
            return (message, IsFountainOn);
        }

        public string PlayerWon()
        {
            return "You Win! Thanks For Playing!";
        }

        public (string message, bool isPlayerAlive) PlayerLost(bool IsPlayerAlive)
        {
            string message = string.Empty;
            IsPlayerAlive = false;
            message = "You Lost! Game Over!";
            return (message, IsPlayerAlive);
        }

        public (string message, bool isPlayerAlive) PlayerQuit(bool IsPlayerAlive)
        {
            string message = string.Empty;
            IsPlayerAlive = false;
            message = "Thanks for playing, Quitter!";
            return (message, IsPlayerAlive);
        }

    }

    public class Empty : IRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }

        public bool InstantDeath => false;
        public Empty(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }
        public string Sense(bool isFountainOn = false)
        {
            return "You sense nothing but the unnatural darkness, this room is empty!";
        }
    }

    public class Entrance : IRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }

        public bool InstantDeath => false;
        public Entrance(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }

        public string Sense(   bool isFountainOn)
        {
            if (isFountainOn)
            {
                return "The Fountain of Objects has been reactivated, and you have escaped with your life!";
            }

            return "You see light coming from the cavern entrance.";
        }

    }

    public class Fountain : IActivatableRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }
        public bool InstantDeath => false;
        public Fountain(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }

        public string Sense(bool isFountainOn)
        {
            if (isFountainOn)
            {
                return "You here rushing waters from the Fountain of Objects, it has been reactivated!";
            }

            return "You hear water dripping in this room. The fountain of Objects is here!";
        }

        public string Enable()
        {
            return "The Fountain of Objects has been reactivated!";
        }

        public string Disable()
        {
            return "The Fountain of Objects has been DeActivated!";
        }
    }

    // create class for new room: Pit
    public class Pit : IRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }
        public bool InstantDeath => true;
        public Pit(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }
        public string Sense(bool isFountainOn = false)
        {
            return "Without warning, you plummet into darkness. You are dead.";
        }
    }

    public class  Adjacent : IRoom
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Location => $"(Row = {Row}, Column = {Column})";
        public string Type { get; set; }

        public bool InstantDeath => false;
        public Adjacent(int row, int column, string type)
        {
            Row = row;
            Column = column;
            Type = type;
        }
        public string Sense(bool isFountainOn)
        {
            return "You feel a draft. There is a PIT in a nearby room.";
        }

    }

}
public interface IRoom
{
    int Row { get; set; }
    int Column { get; set; }
    string Location { get; }
    string Type { get; set; }
    bool InstantDeath { get; }
    string Sense(bool isFountainOn = false);
}

public interface IActivatableRoom : IRoom
{
    string Enable();
    string Disable();
}
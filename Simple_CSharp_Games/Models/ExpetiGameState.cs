using System.Numerics;

namespace Simple_CSharp_Games.Models
{
    public class ExpetiGameState
    {
        public int TargetNumber { get; set; }
        public List<int> Guesses { get; set; } = new List<int>();
        public bool StillGuessing { get; set; } = true;

        public ExpetiGameState() { }

        public void Start()
        {
            PickRandomNumber();     // start game by picking new target number 
            Guesses.Clear();        // clear any guesses from previous games
            StillGuessing = true;   // reset game state to true 
        }

        public void PickRandomNumber()
        {
            Random random = new Random();
            TargetNumber = random.Next(10);

        }

        public bool IsValidGuess(int guess)
        {
            if (Guesses.Contains(guess)) return false;

            return true;
        }

        public void TrackPlayerGuess(int guess)
        {
            Guesses.Add(guess);
        }

        public void GameOver()
        {
            StillGuessing = false;
        }

    }
}

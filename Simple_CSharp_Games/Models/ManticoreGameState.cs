
namespace Simple_CSharp_Games.Models
{
    public class ManticoreGameState
    {
        public int Round { get; set; } = 1; 

        public List<(int Guess, string Result)> Shots { get; set; } = new List<(int Guess, string Result)>();

        public Manticore Manticore { get; set; } = new Manticore();

        public City City { get; set; } = new City();

        public bool IsGameOver => Manticore.Health <= 0 || City.Health <= 0;

        public string? Winner { get; set; }

        public ManticoreGameState() { }

        public void TakeTurn(int guess)
        {
            CheckShotResult(guess);
            AttackCity();
            CheckGameOver();
        }

        public void CheckGameOver()
        {
            if (City.Health <= 0 && Manticore.Health <= 0)
            {
                Winner = "Draw";
            }
            else if (Manticore.Health <= 0)
            {
                Winner = "City";
            }
            else if (City.Health <= 0)
            {
                Winner = "Manticore";
            }
            else
            {
                IncreaseRound();
            }
        }

        public void CheckShotResult(int guess)
        {
            if (guess < Manticore.Range)
            {
                Shots.Add((guess, "Short"));
            }
            else if (guess > Manticore.Range)
            {
                Shots.Add((guess, "Long"));
            }
            else
            {
                Shots.Add((guess, "Hit"));
                City.FireMagicCannon(Round, Manticore);
            }
        }

        public void AttackCity()
        {
            if (Manticore.Health > 0)
            {
                Manticore.AttackCity(City);
            }
        }

        public void IncreaseRound()
        {
            Round++;
        }

        public void ResetGame()
        {
            Round = 1;
            Shots.Clear();
            Manticore = new Manticore();
            City = new City();
            Winner = null;
        }

    }

    public class Manticore 
    {
        public int Health { get; set; } = 5;
        public int Range { get; set; }

        private static readonly Random _random = new Random();

        public Manticore() 
        { 
            //Range = _random.Next(0, 101);
            Range = 0;
        }


        public void AttackCity(City city)
        {
            if (city.Health > 0)
            {
                city.Health -= 1;
            }
        }
    }

    public class  City
    {
        public int Health { get; set; } = 5;

        public MagicCannon cannon = new MagicCannon();

        public City() { }

        public void FireMagicCannon(int round, Manticore manticore)
        {           
            int damage = cannon.CalculateDamage(round);
            manticore.Health -= damage;

        }
    }

    public class MagicCannon
    {
        public int Damage { get; set; }
        public MagicCannon() { }

        public int CalculateDamage(int round)
        {
            // calculate cannon damage
            if (round % 15 == 0)
            {
                Damage = 10;
            }
            else if (round % 3 == 0 || round % 5 == 0)
            {
                Damage = 3;
            }
            else
            {
                Damage = 1;
            }
            return Damage;

        }
    }
}

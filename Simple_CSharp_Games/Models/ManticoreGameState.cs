
namespace Simple_CSharp_Games.Models
{
    public class ManticoreGameState
    {
        public int Round { get; set; } = 1; 
        public ManticoreGameState() { }

    }

    public class Manticore 
    {
        public int Health { get; set; } = 15;
        public int Range { get; set; } = 0;

        private static readonly Random _random = new Random();

        public Manticore() { }

        public void SetManticoreRange()
        {
            Range = _random.Next(0, 101);
        }
    }

    public class  City
    {
        public int Health { get; set; } = 10;

        public City() { }
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

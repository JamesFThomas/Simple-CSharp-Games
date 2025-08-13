using System;

namespace Simple_CSharp_Games.Models.FinalBattle
{
    public class DoNothing : IBehavior
    {
        public string Name { get; set; } = "do nothing";

        public DoNothing() { }

        public string Execute(ICharacter character, ICharacter? target, int? damage)
        {
            return $"{character.Name} will {Name}.";
        }

    }

    public class StandardAttack : IAttack
    {
        public string Name { get; set; }
        public int Damage { get; set; }

        protected static readonly Random random = new Random();
        protected virtual double HitChance => 1.0;

        public StandardAttack(string name, int damage)
        {
            Name = name;
            Damage = damage;
        }
        public virtual string Execute(ICharacter attacker, ICharacter? target, int? damage)
        {
            string? result = null;

            if (target != null)
            {
                // calculate probability of success before adding damage
                bool hit = random.NextDouble() < HitChance;

                int actualDamage = damage ?? Damage;

                if (hit)
                {
                    // landed attack
                    result = $"{attacker.Name} used {Name} on {target.Name}, & dealt {actualDamage} damage ";

                    target.CurrentHP -= actualDamage;

                    if (target.CurrentHP <= 0)
                    {
                        target.CurrentHP = 0;
                    }
                }
                else
                {
                    // missed attack
                    result = $"{attacker.Name} MISSED {target.Name} with {Name} attack!";
                }

            }
            else
            {
                result = "No target to attack.";
            }

            // because health will be displayed in UI I don't think I need this anymore.
            //$"\n{target?.Name} health: {target?.CurrentHP}/{target?.MaxHP}."

            return result;
        }

    }

    public class BoneCrunch : StandardAttack
    {
        private const string AttackName = "bone crunch";

        public BoneCrunch() : base(AttackName, 0)
        {
        }

        public override string Execute(ICharacter attacker, ICharacter? target, int? damage)
        {
            int randomDamage = random.Next(2);

            return base.Execute(attacker, target, randomDamage);

        }
    }

    public class Punch : StandardAttack
    {
        public Punch() : base("punch", 1) { }
    }

    public class QuickShot : StandardAttack
    {
        public QuickShot() : base("quick shot", 3)
        {
        }
        protected override double HitChance => 0.5;

    }

    public class Unravel : StandardAttack
    {
        private const string AttackName = "unravel";

        public Unravel() : base(AttackName, 0) { }

        public override string Execute(ICharacter attacker, ICharacter? target, int? damage)
        {

            int randomDamage = random.Next(3);

            return base.Execute(attacker, target, randomDamage);
        }

    }

}

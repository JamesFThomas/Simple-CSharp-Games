using System;

namespace Simple_CSharp_Games.Models.FinalBattle
{
    public class TrueProgrammer : ICharacter
    {
        public string Name { get; set; }

        public int MaxHP { get; set; } = 2;

        public int CurrentHP { get; set; } = 2;

        public CharacterTypes Type { get; set; } = CharacterTypes.Hero;

        public Dictionary<string, IBehavior> Behaviors { get; set; } = new Dictionary<string, IBehavior>();
        public TrueProgrammer(string name)
        {
            Name = name;
            AddBehavior("donothing", new DoNothing());
            AddBehavior("punch", new Punch());
        }

        public void AddBehavior(string behaviorName, IBehavior action)
        { 
            Behaviors.Add(behaviorName, action);
        }
        public string PerformBehavior(string behaviorName, ICharacter? target)
        {
            string? result = null;
            
            if (Behaviors.TryGetValue(behaviorName, out IBehavior? behavior))
            {
                result = behavior.Execute(this, target, null);
            }

            else
            {
               result = $"{Name} has no actions named: {behaviorName}. Try another!";
            }

            return result;
        }

    }

    public class VinFletcher : ICharacter
    {

        public string Name { get; set; } = "Vin Fletcher";

        public int MaxHP { get; set; } = 2;

        public int CurrentHP { get; set; } = 2;

        public CharacterTypes Type { get; set; } = CharacterTypes.Fletcher;

        public Dictionary<string, IBehavior> Behaviors { get; set; } = new Dictionary<string, IBehavior>();

        public VinFletcher()
        {
            AddBehavior("donothing", new DoNothing());
            AddBehavior("quickshot", new QuickShot());
        }

        public void AddBehavior(string behaviorName, IBehavior action)
        {
            Behaviors.Add(behaviorName, action);
        }
        public string PerformBehavior(string behaviorName, ICharacter? target)
        {
            string? result = null;

            if (Behaviors.TryGetValue(behaviorName, out IBehavior? behavior))
            {
                result = behavior.Execute(this, target, null);
            }

            else
            {
                result = $"{Name} has no actions named: {behaviorName}. Try another";
            }

            return result;
        }

    }

    public class Skeleton : ICharacter
    {
        
        public string Name { get; set; }

        public int MaxHP { get; set; } = 10;

        public int CurrentHP { get; set; } = 10;

        public CharacterTypes Type { get; set; } = CharacterTypes.Skeleton;


        public Dictionary<string, IBehavior> Behaviors { get; set; } = new Dictionary<string, IBehavior>();

        public Skeleton(string name)
        {
            Name = name;
            AddBehavior("bonecrunch", new BoneCrunch());
        }

        public void AddBehavior(string behaviorName, IBehavior action)
        {
            Behaviors.Add(behaviorName, action);
        }
        public string PerformBehavior(string behaviorName, ICharacter? target)
        {
            string? result = null;

            if (Behaviors.TryGetValue(behaviorName, out IBehavior? behavior))
            {

                result = behavior.Execute(this, target, null);
            }
            else
            {
                result = $"{Name} has no actions named: {behaviorName}. Try another";
            }

            return result;
        }


    }

    public class UncodedOne : ICharacter
    {
        public string Name { get; set; }
        public int MaxHP { get; set; } = 2;
        public int CurrentHP { get; set; } = 2;
        public CharacterTypes Type { get; set; } = CharacterTypes.Uncoded;
        public Dictionary<string, IBehavior> Behaviors { get; set; } = new Dictionary<string, IBehavior>();

        public UncodedOne(string name)
        {
            Name = name;
            AddBehavior("unravel", new Unravel());
        }

        public void AddBehavior(string behaviorName, IBehavior action)
        {
            Behaviors.Add(behaviorName, action);
        }
        public string PerformBehavior(string behaviorName, ICharacter? target)
        {
            string? result = null;

            if (Behaviors.TryGetValue(behaviorName, out IBehavior? behavior))
            {

                result = behavior.Execute(this, target, null);
            }

            else
            {
                result = $"{Name} has no actions named: {behaviorName}. Try another";
            }

            return result;
        }
    }

}

public enum CharacterTypes
{
    Hero,
    Fletcher,
    Skeleton,
    Uncoded,
}
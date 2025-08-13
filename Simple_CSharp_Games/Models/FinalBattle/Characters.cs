using System;

namespace Simple_CSharp_Games.Models.FinalBattle
{
    public class TrueProgrammer : ICharacter
    {
        public string Name { get; set; }

        public int MaxHP { get; set; } = 25;

        public int CurrentHP { get; set; } = 25;
        public Dictionary<string, IBehavior> Behaviors { get; set; } = new Dictionary<string, IBehavior>();
        public TrueProgrammer(string name)
        {
            Name = name;
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

        public int MaxHP { get; set; } = 15;

        public int CurrentHP { get; set; } = 15;
        public Dictionary<string, IBehavior> Behaviors { get; set; } = new Dictionary<string, IBehavior>();

        public VinFletcher()
        {
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

            // this should be a return string or at least a boolean for UI state update
            // change later
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

        public int MaxHP { get; set; } = 5;

        public int CurrentHP { get; set; } = 5;
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
        public int MaxHP { get; set; } = 15;
        public int CurrentHP { get; set; } = 15;
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

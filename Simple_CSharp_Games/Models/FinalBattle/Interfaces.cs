namespace Simple_CSharp_Games.Models.FinalBattle
{
    public interface IPlayer
    {
        string Type { get; set; }

        public string PickBehavior(ICharacter character, ICharacter? target);
    }

    public interface ICharacter
    {
        string Name { get; set; }

        int MaxHP { get; set; }

        int CurrentHP { get; set; }

        public Dictionary<string, IBehavior> Behaviors { get; set; }

        public void AddBehavior(string behaviorName, IBehavior action);
        public string PerformBehavior(string actionName, ICharacter? target);

    }

    public interface IBehavior
    {
        string Name { get; set; }

        public string Execute(ICharacter character, ICharacter? target, int? damage);
    }

    public interface IAttack : IBehavior
        {
            int Damage { get; set; }

        }

}

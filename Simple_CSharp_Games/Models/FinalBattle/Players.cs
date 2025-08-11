using System.Linq;

namespace Simple_CSharp_Games.Models.FinalBattle
{
    public class Human : IPlayer
    {
        public string Type { get; set; }

        public Human(string type)
        {
            Type = type;
        }

        public void PickBehavior(ICharacter character, ICharacter? target)
        {

            string? userInput;
            int convertedInput = 0;
            string menuPrompt = $"\n{character.Name}'s behaviors:";
            string choicesPrompt = $"\nWhat behavior would you like {character.Name} to perform? ";
            string invalidInputPrompt = $"\nYour behavior choice was invalid for {character.Name}. Try again!";

            int choice = 0;

            Console.WriteLine(menuPrompt);

            foreach (KeyValuePair<string, IBehavior> keyValuePair in character.Behaviors)
            {
                Console.WriteLine($"{choice} - {keyValuePair.Key}");
                choice++;
            }

            Console.Write(choicesPrompt);
            userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput) || !Int32.TryParse(userInput, out convertedInput) || convertedInput >= character.Behaviors.Count || convertedInput < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(invalidInputPrompt);
                Console.ResetColor();
                PickBehavior(character, target);
                return;
            }

            var behaviorKey = character.Behaviors.Keys.ElementAt(convertedInput);

            character.PerformBehavior(behaviorKey, target);
        }
    }

    public class Computer : IPlayer
    {
        public string Type { get; set; }

        public Computer(string type)
        {
            Type = type;
        }

        public void PickBehavior(ICharacter character, ICharacter? target)
        {
            foreach (var pair in character.Behaviors)
            {
                if (pair.Value is StandardAttack)
                {
                    character.PerformBehavior(pair.Key, target);
                    return; // choose attack immediately
                }
            }

            // Fallback if no StandardAttack found
            var firstKey = character.Behaviors.Keys.FirstOrDefault();
            
            if (firstKey is not null)
            {
                character.PerformBehavior(firstKey, target);
            }

        }
    }
}

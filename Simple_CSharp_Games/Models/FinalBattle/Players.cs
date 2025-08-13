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

        public string PickBehavior(ICharacter character, ICharacter? target)
        {
            string result; 
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
                result = PickBehavior(character, target);
                //return;
            }

            var behaviorKey = character.Behaviors.Keys.ElementAt(convertedInput);

            result = character.PerformBehavior(behaviorKey, target);

            return result;
        }
    }

    public class Computer : IPlayer
    {
        public string Type { get; set; }

        public Computer(string type)
        {
            Type = type;
        }

        public string PickBehavior(ICharacter character, ICharacter? target)
        {
            if (character?.Behaviors == null || character.Behaviors.Count == 0)
            {
                return $"{character?.Name ?? "Unknown"} has no behaviors to perform.";
            }

            string result = string.Empty;
            
            foreach (var pair in character.Behaviors)
            {
                if (pair.Value is StandardAttack)
                {
                    result = character.PerformBehavior(pair.Key, target);
                    break;
                }
            }

            // Fallback if no StandardAttack found
            if (string.IsNullOrEmpty(result))
            {
                var firstKey = character.Behaviors.Keys.FirstOrDefault();

                if (firstKey is not null)
                {
                    result = character.PerformBehavior(firstKey, null);
                }
            }

            return result;
        }
    }
}

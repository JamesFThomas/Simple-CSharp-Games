using System.Linq;

namespace Simple_CSharp_Games.Models.FinalBattle
{

    public class Game
    {
        public List<ICharacter> Heroes { get; set; } = new List<ICharacter>();
        public List<List<ICharacter>> Monsters { get; set; } = new List<List<ICharacter>>();

        public Game()
        {
            Monsters = new List<List<ICharacter>>()
            {
                new List<ICharacter>(),
                new List<ICharacter>(),
                new List<ICharacter>()
        };
        }


        public void Start()
        {
            GameExplanation();

            var (player1, player2) = ChooseGameMode();

            CreateHeroAndMonsterParties();

            Battle(player1, player2);

        }

        private (IPlayer, IPlayer) ChooseGameMode()
        {
            IPlayer player1 = null!;
            IPlayer player2 = null!;
            string? gameMode;
            int convertedInput;

            string invalidEntryPrompt = "\nYour entry was NOT a valid game mode";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nChoose what game play mode you want.");
            Console.WriteLine("1 => Single player");
            Console.WriteLine("2 => Double player versus");
            Console.WriteLine("3 => I just want to watch!");
            Console.ResetColor();

            gameMode = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(gameMode) || !int.TryParse(gameMode, out convertedInput) || (convertedInput < 1 || convertedInput > 3))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(invalidEntryPrompt);
                Console.ResetColor();
                return ChooseGameMode();
            }

            switch (convertedInput)
            {
                case 1:
                    player1 = new Human("Player 1");
                    player2 = new Computer("Computer");
                    break;
                case 2:
                    player1 = new Human("Player 1");
                    player2 = new Human("Player 2");
                    break;
                case 3:
                    player1 = new Computer("Player 1");
                    player2 = new Computer("Computer");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("You like to watch it. It's cool man no judgement!");
                    Console.ResetColor();
                    break;
            }

            return (player1, player2);
        }

        public void GameExplanation()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("The final battle has arrived.");
            Console.WriteLine("On a volcanic island, enshrouded in a cloud of ash, you have reached the lair of the Uncoded One.");
            Console.WriteLine("You have prepared for this fight and you will return Programming to the lands.");
            Console.WriteLine("Your allies have gathered to engage the Uncoded One's minions on the volcanic slopes while you and your party strike into the heart of the Uncoded One's lair to battle and destroy it.");
            Console.WriteLine("Only a True Programmer will be able to survive the encounter, defeat the Uncoded One, and escape!");
            Console.ResetColor();
        }

        public void CreateHeroAndMonsterParties()
        {
            string heroName = CollectHeroName();

            TrueProgrammer hero = new TrueProgrammer(heroName);

            VinFletcher vin = new VinFletcher();

            AddToHeroesParty(hero);

            AddToHeroesParty(vin);

            AddToMonstersParty();

        }

        public string CollectHeroName()
        {
            string? input;
            string namePrompt = "\nWhat shall be our hero's title? ";
            string invalidInputPrompt = "Please enter a valid name for the hero";

            Console.Write(namePrompt);
            input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(invalidInputPrompt);
                Console.ResetColor();
                return CollectHeroName();
            }

            return input;

        }

        public void AddToHeroesParty(ICharacter character)
        {
            Heroes.Add(character);
        }

        public void AddToMonstersParty()
        {
            ICharacter character = new Skeleton("Skelly");
            ICharacter character1 = new Skeleton("Skeletor");
            ICharacter character2 = new Skeleton("Skeletia");
            ICharacter finalBoss = new UncodedOne("Boss Hog");

            Monsters[0].Add(character);     // Battle 1
            Monsters[1].Add(character1);    // Battle 2
            Monsters[1].Add(character2);    // Battle 2
            Monsters[2].Add(finalBoss);     // Battle 3
        }

        private void CheckCharacterHealth(ICharacter character, List<ICharacter> party)
        {
            if (character.CurrentHP == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"{character.Name} has been defeated!");
                Console.ResetColor();
                party.Remove(character);
            }
        }

        public void WhosTurn(ICharacter character)
        {
            string prompt = $"\nIt's {character.Name}'s turn";
            Console.WriteLine(prompt);
        }

        public void HuzzahTheHeroesWon()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nHeroes have won! The Uncoded One has been defeated.");
            Console.ResetColor();
        }

        public void BooTheMonstersWon()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\nHeroes have lost! The Uncoded One's forces have prevailed.");
            Console.ResetColor();
        }

        private void CheckBattleOutcome()
        {

            bool heroesAlive = Heroes.Any(character => character.CurrentHP > 0);
            bool monstersAlive = Monsters.Any(party => party.Any(character => character.CurrentHP > 0));

            if (!heroesAlive)
            {
                BooTheMonstersWon();
                return;
            }
            else if (!monstersAlive)
            {
                HuzzahTheHeroesWon();
                return;
            }
        }

        public void HeroesTurns(IPlayer player, List<ICharacter> targets)
        {
            if (targets.Count == 0) return;

            var currentTarget = targets[0];

            foreach (var hero in Heroes)
            {
                DisplayBattleStatus(hero, Heroes, Monsters);
                WhosTurn(hero);
                player.PickBehavior(hero, currentTarget);
                CheckCharacterHealth(currentTarget, targets);
                Thread.Sleep(500);
            }
        }


        public void MonstersTurns(IPlayer player, int index)
        {
            var targets = Heroes;
            
            if (targets.Count == 0) return;
            
            var currentTarget = targets[0];

            foreach (var monster in Monsters[index])
            {
                DisplayBattleStatus(monster, targets, Monsters);
                WhosTurn(monster);
                player.PickBehavior(monster, currentTarget);
                CheckCharacterHealth(currentTarget, Heroes);
                Thread.Sleep(500);
            }
        }

        public void DisplayBattleStatus(ICharacter currentCharacter, List<ICharacter> heroes, List<List<ICharacter>> monsters)
        {
            var allMonsters = monsters.SelectMany(monsterParty => monsterParty);

            Console.WriteLine("\n======================================================== BATTLE ========================================================\n");

            foreach (var hero in heroes)
            {
                if (hero == currentCharacter)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }

                Console.WriteLine($"{hero.Name} _______________ {hero.CurrentHP}/{hero.MaxHP}");

                Console.ResetColor();
            }

            Console.WriteLine("\n----------------------------------------------------------- VS --------------------------------------------------------\n");




            foreach (var monster in allMonsters)
            {
                if (monster == currentCharacter)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }

                Console.WriteLine($"                                                                                  {monster.Name} _____________ {monster.CurrentHP}/{monster.MaxHP}");

                Console.ResetColor();
            }

            Console.WriteLine("=========================================================================================================================");

        }


        public void Battle(IPlayer player1, IPlayer player2)
        {

            int currentIndex = 0;

            while (Heroes.Any() && currentIndex < Monsters.Count)
            {
                var currentMonsterParty = Monsters[currentIndex];

                HeroesTurns(player1, currentMonsterParty);
                if (!currentMonsterParty.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Monster party {currentIndex + 1} has been defeated!");
                    Console.ResetColor();
                    CheckBattleOutcome();
                    currentIndex++;
                    continue;
                }

                MonstersTurns(player2, currentIndex);
                if (!Heroes.Any())
                {
                    CheckBattleOutcome();
                    break;
                }

            }
        }
    }
}

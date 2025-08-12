using System.Linq;

namespace Simple_CSharp_Games.Models.FinalBattle
{

    public class Game
    {
        public List<ICharacter> Heroes { get; set; } = new List<ICharacter>();
        public List<List<ICharacter>> Monsters { get; set; } = new List<List<ICharacter>>();

        public IPlayer Player1 { get; set; } = new Human("Player 1");

        public IPlayer Player2 { get; set; } = new Computer("Computer");

        public bool isOver { get; set; } = false;

        public IPlayer? Winner { get; set; } = null;

        public Game()
        {
            Monsters = new List<List<ICharacter>>()
            {
                new List<ICharacter>(), // battle 1
                new List<ICharacter>(), // battle 2
                new List<ICharacter>()  // battle 3
            };
        }

        public List<string> InitializeGame(string newHero)  // working
        {
            List<string> initMessages = new List<string>();
            isOver = false;
            Winner = null;

            ClearListBeforeAddingHeroes();
            ClearListsBeforeAddingMonsters();

            ICharacter hero = new TrueProgrammer(newHero);
            ICharacter vin = new VinFletcher();

            initMessages.Add($"Players created: player 1 => {Player1.GetType().Name}, player 2 => {Player2.GetType().Name}");

            AddToHeroesParty(hero);
            initMessages.Add($"{hero.Name} was added to the hero's party");

            AddToHeroesParty(vin);
            initMessages.Add($"{vin.Name} was added to the hero's party");

            AddToMonstersParty();
            initMessages.Add($"{Monsters.Count} waves of opponents were added to the monster's party");

            return initMessages;

        }

        public List<string> CommenceBattle() // not tested yet
        {
            var battleMessages = Battle(Player1, Player2);

            return battleMessages;
        }

        public void ClearListBeforeAddingHeroes()
        {
            Heroes.Clear();
        }

        public void ClearListsBeforeAddingMonsters()
        {
            foreach (List<ICharacter> innerList in Monsters)
            {
                innerList.Clear();
            }
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

        public string WhosTurn(ICharacter character)
        {
            return $"It's {character.Name}'s turn";
             
        }
        
        private string? CheckCharacterHealth(ICharacter character, List<ICharacter> party)
        {
            string? healthMessage = null;

            if (character.CurrentHP == 0)
            {
                healthMessage = $"{character.Name} has been defeated!";
                party.Remove(character);
            }

            return healthMessage;
        }

        public string HuzzahTheHeroesWon()
        {
            return "Heroes have won! The Uncoded One has been defeated.";
        }

        public string BooTheMonstersWon()
        {
            return "Heroes have lost! The Uncoded One's forces have prevailed.";
        }

        private string? CheckBattleOutcome()
        {
            string? outCome = null;
            bool heroesAlive = Heroes.Any(character => character.CurrentHP > 0);
            bool monstersAlive = Monsters.Any(party => party.Any(character => character.CurrentHP > 0));

            if (!heroesAlive)
            {
                isOver = true;
                Winner = Player2;
                outCome = BooTheMonstersWon();
                
            }
            else if (!monstersAlive)
            {
                isOver = true;
                Winner = Player1;
                outCome = HuzzahTheHeroesWon();
            }

            return outCome;
        }

        public List<string> HeroesTurns(IPlayer player, List<ICharacter> targets)
        {
            List<string> turnMessages = new List<string>();

            if (targets.Count == 0) return turnMessages;

            var currentTarget = targets[0];

            foreach (var hero in Heroes)
            {
                if (hero.CurrentHP <= 0) continue;

                var message = WhosTurn(hero);
                
                turnMessages.Add(message);
                
                player.PickBehavior(hero, currentTarget);
                
                var healthMessage = CheckCharacterHealth(currentTarget, targets);

                if (healthMessage != null)
                {
                    turnMessages.Add(healthMessage);

                    // if current target was just removed, pick a new one
                    if (!targets.Contains(currentTarget))
                    {
                        if (targets.Count == 0)
                        {
                            // wave cleared mid-turn — end early
                            return turnMessages;
                        }

                        currentTarget = targets[0]; // retarget to the next alive enemy
                    }
                }
            }

            return turnMessages;
        }

        public List<string> MonstersTurns(IPlayer player, int index)
        {
            List<string> turnMessages = new List<string>();

            var targets = Heroes;
            
            if (targets.Count == 0) return turnMessages;
            
            var currentTarget = targets[0];

            foreach (var monster in Monsters[index])
            {
                if (monster.CurrentHP <= 0) continue;
                
                var message = WhosTurn(monster);
                
                turnMessages.Add(message);
                
                player.PickBehavior(monster, currentTarget);

                var healthMessage = CheckCharacterHealth(currentTarget, targets);
                if (healthMessage != null)
                {
                    turnMessages.Add(healthMessage);
                    if (!targets.Contains(currentTarget))
                    {
                        if (targets.Count == 0)
                        {
                            return turnMessages;
                        }

                        currentTarget = targets[0];
                    }
                }
            }

            return turnMessages;
        }

        public List<string> Battle(IPlayer player1, IPlayer player2)
        {
            List<string> battleMessages = new List<string>();

            int currentIndex = 0;

            while (Heroes.Any() && currentIndex < Monsters.Count)
            {
                var currentMonsterParty = Monsters[currentIndex];

                var heroMessage = HeroesTurns(player1, currentMonsterParty);

                battleMessages.AddRange(heroMessage);
                
                if (!currentMonsterParty.Any())
                {
                    battleMessages.Add($"Monster party {currentIndex + 1} has been defeated!");

                    var outcomeMessage = CheckBattleOutcome();

                    if (outcomeMessage != null) 
                    {
                        battleMessages.Add(outcomeMessage);
                        break;
                    } 

                    currentIndex++;
                    
                    continue;

                }

                var monsterMessage = MonstersTurns(player2, currentIndex);

                battleMessages.AddRange(monsterMessage);

                if (!Heroes.Any())
                {
                    var outcomeMessage = CheckBattleOutcome();

                    if (outcomeMessage != null) battleMessages.Add(outcomeMessage);

                    break;
                }

            }

            return battleMessages;
        }


    }
}

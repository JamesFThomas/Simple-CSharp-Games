using System;
using System.Reflection;
using Simple_CSharp_Games.Models.FinalBattle;

namespace Simple_CSharp_Games.Models
{
    public class BattleGameState
    {

        public int ActiveHeroIndex { get; set; }

        public int CurrentMonsterPartyIndex { get; set; }

        public List<string> BattleLog { get; set; } = new List<string>();

        public BattlePhase BattlePhase { get; set; }

        public Game _battleGame { get; set; } = new Game();

        public string CurrentTurnLabel
        {
            get
            {
                return BattlePhase switch
                {
                    BattlePhase.HeroAwaitInput or BattlePhase.HeroResolving
                        => _battleGame.Heroes.Count > ActiveHeroIndex
                            ? $"{_battleGame.Heroes[ActiveHeroIndex].Name}'s turn"
                            : string.Empty,
                    BattlePhase.MonsterResolving
                        => "Monsters' turn",
                    _ => string.Empty
                };
            }
        }
        public BattleGameState() { }

        public void SetUp(string newHeroName)
        {
            CurrentMonsterPartyIndex = 0;
            BattleLog.Clear();
            BattlePhase = BattlePhase.Setup;

            var initMessages = _battleGame.InitializeGame(newHeroName);

            BattlePhase = BattlePhase.HeroAwaitInput;

            BattleLog.AddRange(initMessages);

        }


        public List<KeyValuePair<string, IBehavior>> GetAvailableActions()
        {
            var actions = _battleGame.Heroes[ActiveHeroIndex].Behaviors.ToList();

            return actions;
        }

        public List<ICharacter> GetAvailableTargets()
        {
            var targets = _battleGame.Monsters[CurrentMonsterPartyIndex].Where(target => target.CurrentHP > 0 ).ToList();

            return targets;
        }

        public void BeginHeroInput()
        {
            if ( _battleGame.Heroes.FirstOrDefault(hero => hero.CurrentHP > 0 ) == null )
            {
                BattlePhase = BattlePhase.Outcome;
            }

            for (int i = 0; i < _battleGame?.Heroes.Count; i++)
            {
                if (_battleGame.Heroes[i].CurrentHP > 0)
                {
                    ActiveHeroIndex = i;
                    break;
                }
            }

            BattlePhase = BattlePhase.HeroAwaitInput;
        }

        public void ResolveHeroAction(string actionId, string targetId)
        {
            // run one heroes turn
            BattlePhase = BattlePhase.HeroResolving;


            if(!int.TryParse(targetId, out var targetIndex)) {
                BattlePhase = BattlePhase.HeroAwaitInput;
                BattleLog.Add("Pick a valid target!");
                return;
            }

            var targets = _battleGame.Monsters[CurrentMonsterPartyIndex];

            if (targetIndex < 0 || targetIndex >= targets.Count)
            {
                BattlePhase = BattlePhase.HeroAwaitInput;
                BattleLog.Add("Pick a valid target!");
                return;
            }

            var target = targets[targetIndex];


            if ( target.CurrentHP <= 0)
            {
                BattlePhase = BattlePhase.HeroAwaitInput;
                BattleLog.Add("Select a different target!");
                return;
            }

            var message = _battleGame.Heroes[ActiveHeroIndex].PerformBehavior(actionId, target);

            if (!string.IsNullOrWhiteSpace(message))
            { 
                BattleLog.Add(message);
            }

            string? healthMessage = null;

            if (target != null)
            {
                healthMessage = _battleGame.CheckCharacterHealth(target, targets);
            }

            if (!string.IsNullOrWhiteSpace(healthMessage)) BattleLog.Add(healthMessage);

            var waveAlive = targets.Any(m => m.CurrentHP > 0);
            
            if (!waveAlive)
            {
                BattleLog.Add($"Monster party {CurrentMonsterPartyIndex + 1} defeated!");
                CurrentMonsterPartyIndex++;
                var anyWavesLeft = CurrentMonsterPartyIndex < _battleGame.Monsters.Count;
                BattlePhase = anyWavesLeft ? BattlePhase.HeroAwaitInput : BattlePhase.Outcome;

                if (!anyWavesLeft)
                    _battleGame.Winner = _battleGame.Player1;

                return;
            }

            BattlePhase = BattlePhase.MonsterResolving;

        }

        public void ResolveMonsterTurn()
        {
            BattlePhase = BattlePhase.MonsterResolving;

            // Edge case: no monster waves means heroes auto‑win
            if (_battleGame.Monsters == null || _battleGame.Monsters.Count == 0)
            {
                _battleGame.Winner = _battleGame.Player1;
                BattlePhase = BattlePhase.Outcome;
                return;
            }

            // Guard: valid wave index?
            if (CurrentMonsterPartyIndex < 0 || CurrentMonsterPartyIndex >= _battleGame.Monsters.Count)
            {
                // auto set up Heroes winning 
                _battleGame.Winner ??= _battleGame.Player1;
                BattlePhase = BattlePhase.Outcome;
                return;
            }

            // Run monsters’ turn for the current wave
            var messages = _battleGame.MonstersTurns(_battleGame.Player2, CurrentMonsterPartyIndex);

            if (messages != null && messages.Count > 0)
                BattleLog.AddRange(messages);

            // Check heroes
            var heroesAlive = _battleGame.Heroes.Any(h => h.CurrentHP > 0);

            if (!heroesAlive)
            {
                _battleGame.Winner = _battleGame.Player2; // monsters win
                BattlePhase = BattlePhase.Outcome; 
                return;
            }

            // Check current wave
            var waveAlive = _battleGame.Monsters[CurrentMonsterPartyIndex].Any(m => m.CurrentHP > 0);
            
            if (!waveAlive)
            {
                BattleLog.Add($"Monster party {CurrentMonsterPartyIndex + 1} defeated!");
            
                CurrentMonsterPartyIndex++;

                var wavesLeft = CurrentMonsterPartyIndex < _battleGame.Monsters.Count;

                if (!wavesLeft)
                {
                    _battleGame.Winner = _battleGame.Player1; // Heroes win
                    BattlePhase = BattlePhase.Outcome;
                    return;
                }
                
                // back to Heroes 
                BattlePhase = BattlePhase.HeroAwaitInput;
                
            }

            // Otherwise, back to player input
            BattlePhase = BattlePhase.HeroAwaitInput;
        }

        public void AdvanceWaveOrOutcome()
        { }

    }
}

public enum BattlePhase
{
    Setup, 
    HeroAwaitInput, 
    HeroResolving, 
    MonsterResolving, 
    WaveTransition, 
    Outcome
}
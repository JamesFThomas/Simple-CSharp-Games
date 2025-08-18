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

        public string LastLogEntry => BattleLog.Count > 0 ? BattleLog[^1] : string.Empty;

        public BattlePhase BattlePhase { get; set; }

        public Game _battleGame { get; set; } = new Game();

        private string? _currentTurnOverride;

        public string CurrentTurnLabel
        {
            get
            {
                // If an override is set, use it; otherwise compute from current state
                if (!string.IsNullOrEmpty(_currentTurnOverride))
                    return _currentTurnOverride;

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
            set
            {
                // Set a manual label, or clear to return to auto mode
                _currentTurnOverride = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        public IPlayer? GameWinner => _battleGame.Winner;

        public BattleGameState() { }

        public void SetUp(string newHeroName)
        {
            CurrentMonsterPartyIndex = 0;
            
            BattleLog.Clear();
            
            BattlePhase = BattlePhase.Setup;

            var initMessages = _battleGame.InitializeGame(newHeroName);

            BattlePhase = BattlePhase.HeroAwaitInput;

            CurrentTurnLabel = string.Empty;

            BattleLog.AddRange(initMessages);

        }


        public List<KeyValuePair<string, IBehavior>> GetAvailableActions()
        {
            if (_battleGame == null || _battleGame.Heroes == null) return new List<KeyValuePair<string, IBehavior>>();
            
            if (ActiveHeroIndex < 0 || ActiveHeroIndex >= _battleGame.Heroes.Count) return new List<KeyValuePair<string, IBehavior>>();

            var actions = _battleGame.Heroes[ActiveHeroIndex].Behaviors.ToList();

            return actions;
        }

        public List<ICharacter> GetAvailableTargets()
        {
            if (_battleGame == null || _battleGame.Monsters == null) return new List<ICharacter>();
            
            if (CurrentMonsterPartyIndex < 0 || CurrentMonsterPartyIndex >= _battleGame.Monsters.Count) return new List<ICharacter>();

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

            CurrentTurnLabel = string.Empty;
        }

        public void ResolveHeroAction(string actionId, string targetId)
        {
            if (BattlePhase != BattlePhase.HeroAwaitInput || _battleGame.Winner != null)
                return;

            BattlePhase = BattlePhase.HeroResolving;

            CurrentTurnLabel = string.Empty;

            if (!int.TryParse(targetId, out var targetIndex))
            {
                BattlePhase = BattlePhase.HeroAwaitInput;
                CurrentTurnLabel = string.Empty;
                BattleLog.Add("Pick a valid target!");
                return;
            }

            var targets = _battleGame.Monsters[CurrentMonsterPartyIndex];

            if (targetIndex < 0 || targetIndex >= targets.Count)
            {
                BattlePhase = BattlePhase.HeroAwaitInput;
                CurrentTurnLabel = string.Empty;
                BattleLog.Add("Pick a valid target!");
                return;
            }

            var target = targets[targetIndex];

            if (target.CurrentHP <= 0)
            {
                BattlePhase = BattlePhase.HeroAwaitInput;
                CurrentTurnLabel = string.Empty;
                BattleLog.Add("Select a different target!");
                return;
            }

            // Perform the action
            var message = _battleGame.Heroes[ActiveHeroIndex].PerformBehavior(actionId, target);
            if (!string.IsNullOrWhiteSpace(message))
                BattleLog.Add(message);

            // Health + defeat messaging for the target
            var healthMessage = _battleGame.CheckCharacterHealth(target, targets);
            if (!string.IsNullOrWhiteSpace(healthMessage))
                BattleLog.Add(healthMessage);

            // Check if wave is cleared
            bool waveAlive = targets.Any(m => m.CurrentHP > 0);

            if (!waveAlive)
            {
                BattleLog.Add($"Monster party {CurrentMonsterPartyIndex + 1} defeated!");
                CurrentMonsterPartyIndex++;

                bool anyWavesLeft = CurrentMonsterPartyIndex < _battleGame.Monsters.Count;
               
                if (!anyWavesLeft)
                {
                    _battleGame.Winner = _battleGame.Player1;   // heroes win
                    
                    BattleLog.Add(_battleGame.HuzzahTheHeroesWon());
                    
                    BattlePhase = BattlePhase.Outcome;
                    CurrentTurnLabel = string.Empty;
                    return;
                }

                // New wave: reset to first alive hero
                for (int i = 0; i < _battleGame.Heroes.Count; i++)
                {
                    if (_battleGame.Heroes[i].CurrentHP > 0)
                    {
                        ActiveHeroIndex = i;
                        break;
                    }
                }

                CurrentTurnLabel = string.Empty;
                BattlePhase = BattlePhase.HeroAwaitInput;
                return;
            }

            // Otherwise rotate to next alive hero, or hand off to monsters
            int nextIndex = -1;
            for (int i = ActiveHeroIndex + 1; i < _battleGame.Heroes.Count; i++)
            {
                if (_battleGame.Heroes[i].CurrentHP > 0)
                {
                    nextIndex = i;
                    break;
                }
            }

            if (nextIndex != -1)
            {
                ActiveHeroIndex = nextIndex;
                BattlePhase = BattlePhase.HeroAwaitInput;
                CurrentTurnLabel = string.Empty;
            }
            else
            {
                BattlePhase = BattlePhase.MonsterResolving;
                CurrentTurnLabel = string.Empty;
            }
        }

        public void ResolveMonsterTurn()
        {
            BattlePhase = BattlePhase.MonsterResolving;

            CurrentTurnLabel = string.Empty;

            // Edge case: no monster waves => heroes win
            if (_battleGame.Monsters == null || _battleGame.Monsters.Count == 0)
            {
                _battleGame.Winner = _battleGame.Player1;
                BattleLog.Add(_battleGame.HuzzahTheHeroesWon());
                BattlePhase = BattlePhase.Outcome;
                return;
            }

            // Guard: valid wave index
            if (CurrentMonsterPartyIndex < 0 || CurrentMonsterPartyIndex >= _battleGame.Monsters.Count)
            {
                _battleGame.Winner ??= _battleGame.Player1;
                BattleLog.Add(_battleGame.HuzzahTheHeroesWon());
                BattlePhase = BattlePhase.Outcome;
                return;
            }

            // Run monsters’ turn for the current wave
            var messages = _battleGame.MonstersTurns(_battleGame.Player2, CurrentMonsterPartyIndex);
            if (messages is { Count: > 0 })
                BattleLog.AddRange(messages);

            // Check heroes alive
            bool heroesAlive = _battleGame.Heroes.Any(h => h.CurrentHP > 0);

            if (!heroesAlive)
            {
                _battleGame.Winner = _battleGame.Player2;
                BattleLog.Add(_battleGame.BooTheMonstersWon());
                BattlePhase = BattlePhase.Outcome;
                CurrentTurnLabel = string.Empty;
                return;
            }

            // Check current wave alive
            bool waveAlive = _battleGame.Monsters[CurrentMonsterPartyIndex].Any(m => m.CurrentHP > 0);
            
            if (!waveAlive)
            {
                BattleLog.Add($"Monster party {CurrentMonsterPartyIndex + 1} defeated!");
                CurrentMonsterPartyIndex++;

                bool wavesLeft = CurrentMonsterPartyIndex < _battleGame.Monsters.Count;
                if (!wavesLeft)
                {
                    _battleGame.Winner = _battleGame.Player1;
                    BattleLog.Add(_battleGame.HuzzahTheHeroesWon());
                    BattlePhase = BattlePhase.Outcome;
                    CurrentTurnLabel = string.Empty;
                    return;
                }

                // New wave: hand control to first alive hero and update label
                for (int i = 0; i < _battleGame.Heroes.Count; i++)
                {
                    if (_battleGame.Heroes[i].CurrentHP > 0)
                    {
                        ActiveHeroIndex = i;
                        break;
                    }
                }
                
                //CurrentTurnLabel = $"{_battleGame.Heroes[ActiveHeroIndex].Name}'s turn";

                BattlePhase = BattlePhase.HeroAwaitInput;
                CurrentTurnLabel = string.Empty;
                return;
            }

            // Wave still alive: back to heroes (first alive) and update label
            for (int i = 0; i < _battleGame.Heroes.Count; i++)
            {
                if (_battleGame.Heroes[i].CurrentHP > 0)
                {
                    ActiveHeroIndex = i;
                    break;
                }
            }
            
            //CurrentTurnLabel = $"{_battleGame.Heroes[ActiveHeroIndex].Name}'s turn";

            CurrentTurnLabel = string.Empty;
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
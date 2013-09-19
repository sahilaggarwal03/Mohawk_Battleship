﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MBC.Core.Events;
using MBC.Core.Rounds;
using MBC.Core.Util;
using MBC.Shared;

namespace MBC.Core.Matches
{
    [Configuration("mbc_field_width", 10)]
    [Configuration("mbc_field_height", 10)]
    [Configuration("mbc_ship_sizes", 2, 3, 3, 4, 5)]
    [Configuration("mbc_game_mode", GameMode.Classic, null)]
    [Configuration("mbc_match_playeradd_init_only", true)]
    [Configuration("mbc_match_teams", 1)]
    [Configuration("mbc_match_rounds_mode", RoundMode.AllRounds,
        Description = "Determines the ending behaviour of a match based on a given number of rounds.",
        DisplayName = "Match Rounds Mode")]
    [Configuration("mbc_match_rounds", 100)]
    public class Match
    {
        private List<MatchEvent> events;

        private List<Player> players;

        private Dictionary<IDNumber, Player> playersByID;

        private List<Round> rounds;

        private bool started;

        public Match(Configuration conf)
        {
            Init();
            EnsureGameModeCompatibility();
            Config = conf;
        }        /// <summary>
        private Match()
        {
            Init();
        }

        /// <summary>
        /// Invoked whenever an <see cref="Event"/> has been generated.
        /// </summary>
        public event MBCEventHandler Event;

        /// Provides various behaviours as to how a Match will handle the number of rounds it is configured
        /// with.
        /// </summary>
        public enum RoundMode
        {
            /// <summary>
            /// Creates and plays through <see cref="Round"/>s until the number of <see cref="Round"/>s generated
            /// is equal to the number of rounds configured.
            /// </summary>
            /// <seealso cref="Round"/>
            AllRounds,

            /// <summary>
            /// Creates and plays through <see cref="Round"/>s until a <see cref="ControllerRegister"/> has
            /// reached the number of rounds configured.
            /// </summary>
            /// <seealso cref="Round"/>
            /// <seealso cref="ControllerRegister"/>
            FirstTo
        }
        /// <summary>
        /// Gets the <see cref="Configuration"/> used to determine game behaviour.
        /// </summary>
        public Configuration Config
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="BooleanThreader"/> that handle multi-threading and automatic progression.
        /// </summary>
        [XmlIgnore]
        public BooleanThreader Thread
        {
            get;
            private set;
        }

        public IList<Player> Players
        {
            get
            {
                return players.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Event"/>s that have been generated by this <see cref="Match"/>.
        /// </summary>
        private IList<MatchEvent> Events
        {
            get
            {
                return events.AsReadOnly();
            }
        }

        private IList<Round> Rounds
        {
            get
            {
                return rounds.AsReadOnly();
            }
        }

        public void AddPlayer(Player plr)
        {
            if (started && Config.GetValue<bool>("mbc_match_playeradd_init_only"))
            {
                throw new InvalidOperationException("Cannot add players after the match has started (mbc_match_playeradd_init_only set to true)");
            }
        }

        public void End()
        {
            Thread.Pause();
            MakeEvent(new MatchEndEvent());
        }

        public Player GetPlayerByID(IDNumber id)
        {
            return playersByID[id];
        }

        public bool PlayRound()
        {
            if (MatchConditionsMet())
            {
                return true;
            }
            Round newRound = CreateNewRound();


            return MatchConditionsMet();
        }

        public void SaveToFile(File fLocation)
        {
        }

        /// <summary>
        /// Used to send a generated <see cref="Event"/> to any listeners on <see cref="Match.Event"/>.
        /// </summary>
        /// <param name="ev">The <see cref="Event"/> generated in this <see cref="Match"/>.</param>
        /// <param name="backward">True if the event was invoked backwards.</param>
        internal void RoundEventGenerated(Event ev, bool backward)
        {
            if (Event != null)
            {
                Event(ev, backward);
            }
        }

        private void EnsureGameModeCompatibility()
        {
            foreach (var mode in Config.GetList<GameMode>("mbc_game_mode"))
            {
                if (mode == GameMode.Teams)
                {
                    throw new NotImplementedException("The " + mode.ToString() + " game mode is not supported.");
                }
            }
        }

        private void Init()
        {
            events = new List<MatchEvent>();
            rounds = new List<Round>();
            players = new List<Player>();
            playersByID = new Dictionary<IDNumber, Player>();
            Thread = new BooleanThreader(PlayRound);
            started = false;
        }

        /// <summary>
        /// Generates a <see cref="Match"/>-specific <see cref="Event"/>.
        /// </summary>
        /// <param name="ev">The <see cref="Event"/> generated.</param>
        private void MakeEvent(MatchEvent ev)
        {
            events.Add(ev);
            if (Event != null)
            {
                Event(ev, false);
            }
        }

        private Round CreateNewRound()
        {
            foreach (var mode in Config.GetList<GameMode>("mbc_game_mode"))
            {
                switch (mode)
                {
                    case GameMode.Classic:
                        return new ClassicRound(this);
                }
            }
            throw new InvalidOperationException("An unsupported game mode was configured for this match!");
        }

        private bool MatchConditionsMet()
        {
            switch (Config.GetValue<RoundMode>("mbc_match_rounds_mode"))
            {
                case RoundMode.AllRounds:
                    return rounds.Count >= Config.GetValue<int>("mbc_match_rounds");
                case RoundMode.FirstTo:
                    foreach (var player in players)
                    {
                        if (player.Score >= Config.GetValue<int>("mbc_match_rounds"))
                        {
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

    }
}
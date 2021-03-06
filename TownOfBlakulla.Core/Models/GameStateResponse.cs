﻿using System;
using System.Linq;

namespace TownOfBlakulla.Core.Models
{
    public class GameStateResponse
    {
        public GameStateResponse(
            GameState state,
            bool hasJoined,
            bool lynched,
            string[] abilityArgs,
            GameInfo game,
            int revision)
        {
            State = state;
            HasJoined = hasJoined;
            Lynched = lynched;
            AbilityArgs = abilityArgs;
            Game = game;
            Revision = revision;
        }

        public bool HasJoined { get; }
        public bool Lynched { get; }
        public string[] AbilityArgs { get; }
        public GameInfo Game { get; }
        public GameState State { get; }
        public int Revision { get; }
    }

    public class ChatMessage
    {
        public ChatMessage(string sender, string channel, string message)
        {
            Sender = sender;
            Channel = channel;
            Message = message;
            TimeSent = DateTime.UtcNow;
        }

        public DateTime TimeSent { get; }
        public string Sender { get; }
        public string Channel { get; } // or target player if privmsg
        public string Message { get; }
    }

    public class GameInfo
    {
        public static GameInfo FromUpdateInfo(GameUpdateInfo source, bool isPlaying)
        {
            var result = new GameInfo();
            if (isPlaying)
            {
                result.Players = source.Players.Select(GameInfo.Player.Map).ToArray();
                result.SubPhaseStart = source.SubPhase.EnterTime;
                result.SubPhaseDuration = source.SubPhase.Duration;
            }

            result.Phase = source.PhaseName;
            result.SubPhase = source.SubPhase.Name;
            return result;
        }

        public float SubPhaseDuration { get; set; }

        public Player[] Players { get; set; }

        public float SubPhaseStart { get; set; }

        public string SubPhase { get; set; }

        public string Phase { get; set; }

        public class Player
        {
            private Player(string identifier, string name, bool lynched)
            {
                Name = name;
                Identifier = identifier;
                Lynched = lynched;
            }

            public string Identifier { get; }

            public string Name { get; }

            public bool Lynched { get; }

            public static Player Map(PlayerInfo player)
            {
                return new Player(player.Identifier, player.Name, player.Lynched);
            }
        }
    }

}
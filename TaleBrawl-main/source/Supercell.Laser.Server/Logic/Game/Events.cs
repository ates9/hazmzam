﻿namespace Supercell.Laser.Server.Logic.Game
{
    using Google.Protobuf.WellKnownTypes;
    using Supercell.Laser.Logic.Battle.Objects;
    using Supercell.Laser.Logic.Command.Home;
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Home;
    using Supercell.Laser.Logic.Home.Items;
    using Supercell.Laser.Logic.Home.Structures;
    using Supercell.Laser.Logic.Listener;
    using Supercell.Laser.Logic.Message.Account;
    using Supercell.Laser.Logic.Message.Home;
    using Supercell.Laser.Server.Networking.Session;
    using Supercell.Laser.Titan.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public static class Events
    {
        public const int REFRESH_MINUTES = 6 * 60;

        private static Timer RefreshTimer;
        private static EventSlotConfig[] ConfigSlots;
        private static Dictionary<int, EventData> Slots;
        public static Dictionary<int, int> SlotsLocations;
        public static int[] Locations;
        private static int EventWriter;
        private static Dictionary<int, List<long>> SlotsCollected = new();
        private static Dictionary<int, List<long>> SlotsPlayed = new();

        public static void Init()
        {
            LoadSettings();
            Slots = new Dictionary<int, EventData>();
            SlotsLocations = new Dictionary<int, int>();
            EventWriter = 0;
            Locations = new int[16];
            RefreshTimer = new Timer(new TimerCallback(RefreshTimerElapsed), null, 0, REFRESH_MINUTES * 60 * 1000 - 1);
            //GenerateEvents();
        }

        public static void GenerateEvents()
        {
            int index = 0;
            for (int i = 0; i < ConfigSlots.Length + 1; i++, index++)
            {
                EventData[] eventspair = GenerateEvent(ConfigSlots[index].AllowedModes, ConfigSlots[index].Slot, ConfigSlots[index].SlotIndex);

                if (eventspair.Length > 1)
                {
                    Slots[ConfigSlots[i].Slot] = eventspair[0];
                    i++;
                    Slots[5] = eventspair[1];
                }
                else
                {
                    Slots[ConfigSlots[index].Slot] = eventspair[0];
                }

            }
        }

        private static void RefreshTimerElapsed(object s)
        {
            SlotsLocations = new Dictionary<int, int>();
            SlotsCollected = new Dictionary<int, List<long>>();
            SlotsPlayed = new Dictionary<int, List<long>>();
            EventWriter = 0;
            GenerateEventsInf();
            //GenerateEvents();
            EventData[] Updated = GetEvents();
            foreach (Session session in Sessions.ActiveSessions.Values.ToArray())
            {
                LogicDayChangedCommand newday = new()
                {
                    Home = session.Home.Home
                };
                newday.Home.Events = Updated;
                AvailableServerCommandMessage eventupdated = new()
                {
                    Command = newday,
                };
                session.Connection.Send(eventupdated);
            }
            //LogicGameListener
        }
        public static void GenerateEventsInf()
        {
            bool v1 = false;
            while (!v1)
            {
                try
                {
                    GenerateEvents();
                    v1 = true;
                }
                catch (Exception e)
                {

                }
            }

        }

        private static EventData[] GenerateEvent(string[] gameModes, int slot, int slotIndex)
        {
            int count = DataTables.Get(DataType.Location).Count;
            Random rand = new();
            while (true)
            {
                LocationData location = DataTables.Get(DataType.Location).GetDataWithId<LocationData>(rand.Next(0, count));
                if (!location.Disabled && gameModes.Contains(location.GameMode))
                {
                    int[] modifires = { 1, 2, 3, 5 };
                    Random random = new();
                    int[] sel = {

                    };
                    EventData ev = null;
                    if (slot != 9)
                    {
                        ev = new()
                        {
                            EndTime = DateTime.Now.AddMinutes(REFRESH_MINUTES),
                            LocationId = location.GetGlobalId(),
                            Slot = slot,
                            Modifiers = new int[0],
                        };

                    }
                    else
                    {
                        ev = new()
                        {
                            EndTime = DateTime.Now.AddMinutes(REFRESH_MINUTES),
                            LocationId = location.GetGlobalId(),
                            Slot = slot,
                            Modifiers = new int[0]
                        };
                    }
                    Locations[EventWriter] = location.GetGlobalId();
                    EventWriter++;
                    SlotsLocations.Add(location.GetGlobalId(), slot);
                    SlotsCollected.Add(slot, new List<long>());
                    SlotsPlayed.Add(slot, new List<long>());
                    if (slot == 2)
                    {
                        string[] mode = location.Name.Split("l");
                        string sep = "l";
                        if (mode[0].Contains("Graves"))
                        {
                            mode = location.Name.Split("s");
                            sep = "s";
                        }
                        string duomode = mode[0] + sep + "Team" + mode[1];
                        LocationData locationa = DataTables.Get(DataType.Location).GetData<LocationData>(duomode);
                        EventData evs = new()
                        {
                            EndTime = DateTime.Now.AddMinutes(REFRESH_MINUTES),
                            LocationId = locationa.GetGlobalId(),
                            Slot = 5,
                            Modifiers = sel,
                        };
                        Locations[EventWriter] = locationa.GetGlobalId();
                        EventWriter++;
                        SlotsLocations.Add(locationa.GetGlobalId(), 5);
                        SlotsCollected.Add(5, new List<long>());
                        SlotsPlayed.Add(5, new List<long>());
                        return new EventData[] { ev, evs };
                    }
                    return new EventData[] { ev };
                }
            }
        }

        private static void LoadSettings()
        {
            LogicJSONObject settings = LogicJSONParser.ParseObject(File.ReadAllText("gameplay.json"));
            LogicJSONArray slots = settings.GetJSONArray("slots");
            ConfigSlots = new EventSlotConfig[slots.Size()];

            for (int i = 0; i < slots.Size(); i++)
            {
                EventSlotConfig config = new();

                LogicJSONObject slot = slots.GetJSONObject(i);
                config.Slot = slot.GetJSONNumber("slot").GetIntValue();

                LogicJSONArray gameModes = slot.GetJSONArray("game_modes");
                config.AllowedModes = new string[gameModes.Size()];
                for (int j = 0; j < gameModes.Size(); j++)
                {
                    config.AllowedModes[j] = gameModes.GetJSONString(j).GetStringValue();
                }

                ConfigSlots[i] = config;
            }
        }
        public static bool CollectEvent(long id, int slot)
        {
            if (!HasSlot(slot)) return false;
            if (SlotsCollected[slot].Contains(id)) return false;
            SlotsCollected[slot].Add(id);
            //Slots[slot].IsNewEvent = false;
            return true;
        }

        public static bool PlaySlot(long id, int slot)
        {
            if (!HasSlot(slot)) return false;
            if (SlotsPlayed[slot].Contains(id)) return false;

            if (slot == 5)
            {
                SlotsPlayed[2].Add(id);
                SlotsPlayed[5].Add(id);
            }
            else if (slot == 2)
            {
                SlotsPlayed[2].Add(id);
                SlotsPlayed[5].Add(id);
            }
            else
            {
                SlotsPlayed[slot].Add(id);
            }
            //Slots[slot].IsNewEvent = false;
            return true;
        }

        public static bool HasEventBonus(long id, int slot)
        {
            if (!SlotsCollected[slot].Contains(id))
            {
                return false;
            }
            return true;
        }
        public static EventData GetEvent(int i)
        {
            if (HasSlot(i))
                return Slots[i];
            return null;
        }

        public static bool HasSlot(int slot)
        {
            return Slots.ContainsKey(slot);
        }

        public static bool HasLocation(int location)
        {
            return Locations.Contains(location);
        }

        public static EventData[] GetEvents()
        {
            return Slots.Values.ToArray();
        }

        public static EventData[] GetEventsById(int a, long id)
        {
            try
            {
                EventData[] events = Slots.Values.ToArray();
                int index = 0;
                foreach (EventData e in events)
                {
                    if (SlotsCollected[e.Slot].Contains(id))
                    {
                        e.IsBonusCollected = true;
                    }
                    else
                    {
                        e.IsBonusCollected = false;
                    }
                    e.PowerPlayGamesPlayed = a;
                    events[index] = e;
                    index++;
                }
                return events;
            }
            catch
            {
                return Slots.Values.ToArray();
            }
        }
        public static EventData[] GetSecondaryEvents()
        {
            EventData[] events = Slots.Values.ToArray();
            int index = 0;
            foreach (EventData e in events)
            {
                e.IsSecondary = true;
                e.EndTime.AddSeconds((int)(e.EndTime - DateTime.Now).TotalSeconds);
                index++;
            }
            return events;
        }

        private class EventSlotConfig
        {
            public int Slot { get; set; }

            public int SlotIndex { get; set; }
            public string[] AllowedModes { get; set; }
        }
    }
}

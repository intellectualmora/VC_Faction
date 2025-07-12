// Decompiled with JetBrains decompiler
// Type: RimWorld.MissionSignal
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    public class MissionSignal : RadioSignal
    {
        public MissionKindDef def;
        private bool wasAnnounced;
        private static List<string> tmpExtantNames = new List<string>();

        public override string FullTitle => this.name;

        public IThingHolder ParentHolder => (IThingHolder)this.Map;

        public string MissionName => this.def.label;

        public bool CanReceiveNow => !this.Departed;

        public bool WasAnnounced
        {
            get => this.wasAnnounced;
            set => this.wasAnnounced = value;
        }


        public MissionSignal()
        {
        }

        public MissionSignal(MissionKindDef def,Faction faction = null)
          : base(faction)
        {
            this.def = def;
            this.name = def.label;
            MissionSignal.tmpExtantNames.Clear();
            List<Map> maps = Find.Maps;
            for (int index = 0; index < maps.Count; ++index)
                MissionSignal.tmpExtantNames.AddRange(maps[index].passingShipManager.passingShips.Select<PassingShip, string>((Func<PassingShip, string>)(x => x.name)));
            this.name = NameGenerator.GenerateName(RulePackDefOf.NamerTraderGeneral, (IEnumerable<string>)MissionSignal.tmpExtantNames);
            if (faction != null)
                this.name = string.Format("{0} {1} {2}", (object)this.name, (object)"OfLower".Translate(), (object)faction.Name);
            this.loadID = Find.UniqueIDsManager.GetNextPassingShipID();
        }


        public override void RadioTick()
        {
            base.RadioTick();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.wasAnnounced, "wasAnnounced", true);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
        }

        public override void TryOpenComms(Pawn negotiator)
        {
            if (!this.CanReceiveNow)
                return;
            Find.WindowStack.Add((Window)new Dialog_Radio(negotiator,radioSignalManager, this,ContentFinder<Texture2D>.Get(def.graphicsPath), def.title,def.text,def.choice1,def.choice2,def.questScriptDef,def.point));
        }

        public override void Depart()
        {
            base.Depart();
        }

        public override string GetCallLabel() => this.name;

        protected override AcceptanceReport CanCommunicateWith(Pawn negotiator)
        {
            AcceptanceReport acceptanceReport = base.CanCommunicateWith(negotiator);
            return acceptanceReport;
        }

        public override string ToString() => this.FullTitle;
    }
}

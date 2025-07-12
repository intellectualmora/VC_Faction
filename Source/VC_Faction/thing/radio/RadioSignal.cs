// Decompiled with JetBrains decompiler
// Type: RimWorld.RadioSignal
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Linq;
using Verse;
using RimWorld;
namespace VC_Faction
{
    public class RadioSignal : IExposable, ICommunicable, ILoadReferenceable
    {
        public RadioSignalManager radioSignalManager;
        private Faction faction;
        public string name = "Nameless";
        protected int loadID = -1;
        public int ticksUntilDeparture = 40000;

        public virtual string FullTitle => "ErrorFullTitle";

        public bool Departed => this.ticksUntilDeparture <= 0;

        public Map Map => this.radioSignalManager == null ? (Map)null : this.radioSignalManager.map;

        public Faction Faction => this.faction;

        public RadioSignal()
        {
        }

        public RadioSignal(Faction faction)
        {
            this.faction = faction;
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look<string>(ref this.name, "name");
            Scribe_Values.Look<int>(ref this.loadID, "loadID");
            Scribe_Values.Look<int>(ref this.ticksUntilDeparture, "ticksUntilDeparture");
            Scribe_References.Look<Faction>(ref this.faction, "faction");
        }

        public virtual void RadioTick()
        {
            --this.ticksUntilDeparture;
            if (!this.Departed)
                return;
            this.Depart();
        }

        public virtual void Depart()
        {
            if (this.Map.listerBuildings.ColonistsHaveBuilding((Func<Thing, bool>)(b => b.def.IsCommsConsole)))
                Messages.Message((string)"来自"+this.Faction.Name+this.name+"的无线电信号消失", MessageTypeDefOf.SituationResolved);
            this.radioSignalManager.RemoveRadioSignal(this);
        }

        public virtual void TryOpenComms(Pawn negotiator) => throw new NotImplementedException();

        public virtual string GetCallLabel() => this.name;

        public string GetInfoText() => this.FullTitle;

        public Faction GetFaction() => (Faction)null;

        protected virtual AcceptanceReport CanCommunicateWith(Pawn negotiator) => AcceptanceReport.WasAccepted;

        public FloatMenuOption CommFloatMenuOption(Building_Radio console, Pawn negotiator)
        {
            string label = "接收来自"+this.GetCallLabel()+"的无线讯息";
            Action action1 = (Action)null;
            AcceptanceReport canCommunicate = this.CanCommunicateWith(negotiator);
            if (!canCommunicate.Accepted)
            {
                if (!canCommunicate.Reason.NullOrEmpty())
                    action1 = (Action)(() => Messages.Message(canCommunicate.Reason, (LookTargets)(Thing)console, MessageTypeDefOf.RejectInput, false));
            }
            else
                action1 = (Action)(() =>
                {
                    console.GiveUseCommsJob(negotiator, (ICommunicable)this);
                });
            Action action2 = action1;
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action2, MenuOptionPriority.InitiateSocial), negotiator, (LocalTargetInfo)(Thing)console);
        }

        public FloatMenuOption CommFloatMenuOption(Building_CommsConsole console, Pawn negotiator)
        {
            return null;
        }

        public string GetUniqueLoadID() => "RadioSignal_" + (object)this.loadID;
    }
}

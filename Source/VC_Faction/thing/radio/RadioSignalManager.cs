// Decompiled with JetBrains decompiler
// Type: RimWorld.RadioSignalManager
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Verse;
using RimWorld;
namespace VC_Faction
{
    public sealed class RadioSignalManager : IExposable
    {
        public Map map;
        public List<RadioSignal> radioSignals = new List<RadioSignal>();
        private static List<RadioSignal> tmpRadioSignals = new List<RadioSignal>();
        public RadioSignalManager() { }
        public RadioSignalManager(Map map) => this.map = map;

        public void ExposeData()
        {
            Scribe_Collections.Look<RadioSignal>(ref this.radioSignals, "VC_RadioSignalManager.radioSignals", LookMode.Deep);

            if (Scribe.mode != LoadSaveMode.LoadingVars)
                return;
            for (int index = 0; index < this.radioSignals.Count; ++index)
                this.radioSignals[index].radioSignalManager = this;
        }

        public void AddRadioSignal(RadioSignal vis)
        {
            this.radioSignals.Add(vis);
            vis.radioSignalManager = this;
        }

        public void RemoveRadioSignal(RadioSignal vis)
        {
            this.radioSignals.Remove(vis);
            vis.radioSignalManager = (RadioSignalManager)null;
        }

        public void RadioSignalManagerTick()
        {
            for (int index = this.radioSignals.Count - 1; index >= 0; --index)
                this.radioSignals[index].RadioTick();
        }

        public void RemoveAllRadioSignalsOfFaction(Faction faction)
        {
            for (int index = this.radioSignals.Count - 1; index >= 0; --index)
            {
                if (this.radioSignals[index].Faction == faction)
                    this.radioSignals[index].Depart();
            }
        }

        internal void DebugSendAllRadioSignalsAway()
        {
            RadioSignalManager.tmpRadioSignals.Clear();
            RadioSignalManager.tmpRadioSignals.AddRange((IEnumerable<RadioSignal>)this.radioSignals);
            for (int index = 0; index < RadioSignalManager.tmpRadioSignals.Count; ++index)
                RadioSignalManager.tmpRadioSignals[index].Depart();
            Messages.Message("All Radio Signals sent away.", MessageTypeDefOf.TaskCompletion, false);
        }
    }
}

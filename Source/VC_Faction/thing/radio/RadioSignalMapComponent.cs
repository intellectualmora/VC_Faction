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
    public class RadioSignalMapComponent : MapComponent
    {
        public RadioSignalManager radioSignalManager;

        public RadioSignalMapComponent(Map map) : base(map)
        {
            radioSignalManager = new RadioSignalManager(map);
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref radioSignalManager, "VC_RadioSignalManager");
        }
    }
}

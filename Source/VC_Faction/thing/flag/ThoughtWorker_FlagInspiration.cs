// Decompiled with JetBrains decompiler
// Type: RimWorld.ThoughtWorker_PsychicEmanatorSoothe
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Verse;
using RimWorld;

namespace VC_Faction
{
    public class ThoughtWorker_FlagInspiration : ThoughtWorker
    {
        private const float Radius = 10f;

        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.Spawned)
                return (ThoughtState)false;
            if (p.NonHumanlikeOrWildMan())
            {
                return (ThoughtState)false;
            }
            List<Thing> thingList1 = p.Map.listerThings.ThingsOfDef(ThingDefOf.VC_Gallia_Flag);
            for (int index = 0; index < thingList1.Count; ++index)
            {
                if ( p.Position.InHorDistOf(thingList1[index].Position, Radius) && p.Faction.def == FactionDefOf.VC_Gallia)
                    return (ThoughtState)true;
            }
            List<Thing> thingList2 = p.Map.listerThings.ThingsOfDef(ThingDefOf.VC_Empire_Flag);
            for (int index = 0; index < thingList2.Count; ++index)
            {
                if (p.Position.InHorDistOf(thingList2[index].Position, Radius) && p.Faction.def == FactionDefOf.VC_Empire)
                    return (ThoughtState)true;
            }
            List<Thing> thingList3 = p.Map.listerThings.ThingsOfDef(ThingDefOf.VC_Federation_Flag);
            for (int index = 0; index < thingList3.Count; ++index)
            {
                if (p.Position.InHorDistOf(thingList3[index].Position, Radius) && p.Faction.def == FactionDefOf.VC_Federation)
                    return (ThoughtState)true;
            }
            return (ThoughtState)false;
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: RimWorld.PitGate
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

using  RimWorld;
namespace VC_Faction
{
    public class Building_Stair_Down : RimWorld.MapPortal
    {
        public Building_Stair_Up connectStair;
        private Map underground;
        public override IntVec3 GetDestinationLocation()
        {
            return this.connectStair.Position;
        }

        public override Map GetOtherMap()
        {
            List<Thing> listDown = this.Map.listerThings.ThingsOfDef(ThingDefOf.VC_Stair_Down);
                if (underground == null)
                {
                    GenerateUnderground(listDown);
                }
                return this.underground;
        }

        public void GenerateUnderground(List<Thing> listDown)
        {
            this.underground = PocketMapUtility.GeneratePocketMap(new IntVec3(GenStep_Relic_Underground0.x, 1, GenStep_Relic_Underground0.z),
                    MapGeneratorDefOf.VC_Relic_Underground0, (IEnumerable<GenStepWithParams>)null, this.Map);
            List<Thing> listup = this.underground.listerThings.ThingsOfDef(ThingDefOf.VC_Stair_Up);
            for (int i = 0; i < listup.Count; i++)
            {
                Building_Stair_Up stair = (Building_Stair_Up)listup[i];
                stair.floor = this.Map;
                stair.connectStair = (Building_Stair_Down)listDown[i];
                stair.connectStair.connectStair = stair;
                stair.connectStair.underground = this.underground;
            }
        }
    }

}

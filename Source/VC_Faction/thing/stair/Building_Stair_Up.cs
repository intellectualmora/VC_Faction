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

using RimWorld;
namespace VC_Faction
{
    public class Building_Stair_Up : RimWorld.MapPortal
    {
        public Building_Stair_Down connectStair;
        public Map floor;
        public override IntVec3 GetDestinationLocation()
        {
            return this.connectStair.Position;
        }

        public override Map GetOtherMap()
        {
            return this.floor;
        }

    }

}
// Decompiled with JetBrains decompiler
// Type: RimWorld.ICommunicable
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld;
using Verse;

namespace VC_Faction
{
    public interface ICommunicable:RimWorld.ICommunicable
    {
        FloatMenuOption CommFloatMenuOption(Building_Radio console, Pawn negotiator);
    }
}
using System.Diagnostics;
using RimWorld;
using VC_Faction;
using Verse;

[StaticConstructorOnStartup]
public static class VC_MyMod_DefInject
{
    static VC_MyMod_DefInject()
    {
        DefDatabase<VC_Faction.MissionKindDef>.AddAllInMods();
        DefDatabase<VC_Faction.RoleScriptDef>.AddAllInMods();
    }
}
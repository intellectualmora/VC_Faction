using Verse.AI.Group;
using RimWorld;
namespace VC_Faction
{
    public class LordJob_ManTurrets : LordJob
    {
        public override StateGraph CreateGraph() => new StateGraph()
        {
            StartingToil = (LordToil)new LordToil_ManClosestTurrets()
        };
    }
}
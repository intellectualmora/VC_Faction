using Verse;

namespace VC_Faction
{
    public class JobGiver_ManTurretsNearSelf : JobGiver_ManTurrets
    {
        protected override IntVec3 GetRoot(Pawn pawn) => pawn.Position;
    }
}

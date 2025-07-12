using RimWorld;
using Verse;

namespace VC_Faction
{
    [DefOf]
    public static class RecipeDefOf
    {
        public static RecipeDef FillRagniteReactor;
        public static RecipeDef FillRagniteReactorBulk;
        static RecipeDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(RecipeDefOf));
    }
}
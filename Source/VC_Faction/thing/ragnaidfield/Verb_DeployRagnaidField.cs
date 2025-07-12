// Decompiled with JetBrains decompiler
// Type: RimWorld.Verb_DeployRagnaidField
// Assembly: Assembly-CSharp, Version=1.5.8909.13066, Culture=neutral, PublicKeyToken=null
// MVID: F0AC5EB9-B52E-4CC3-96C7-0FC9A4EE15E5
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using RimWorld;
using Verse;
using Verse.Sound;

namespace VC_Faction
{
    public class Verb_DeployRagnaidField : Verb
    {
        protected override bool TryCastShot() => Verb_DeployRagnaidField.Deploy(this.ReloadableCompSource);

        public static bool Deploy(CompApparelReloadable comp)
        {
            if (!ModLister.CheckRoyalty("Projectile interceptors") || comp == null || !comp.CanBeUsed(out string _))
                return false;
            Pawn wearer = comp.Wearer;
            Map map = wearer.Map;
            int num = GenRadial.NumCellsInRadius(4f);
            for (int index = 0; index < num; ++index)
            {
                IntVec3 intVec3 = wearer.Position + GenRadial.RadialPattern[index];
                if (intVec3.IsValid && intVec3.InBounds(map))
                {
                    Verb_DeployRagnaidField.SpawnEffect(GenSpawn.Spawn(ThingDefOf.VC_RagnaidProjector, intVec3, map));
                    comp.UsedOnce();
                    return true;
                }
            }
            Messages.Message((string)"AbilityNotEnoughFreeSpace".Translate(), (LookTargets)(Thing)wearer, MessageTypeDefOf.RejectInput, false);
            return false;
        }

        private static void SpawnEffect(Thing projector)
        {
            FleckMaker.Static(projector.TrueCenter(), projector.Map, FleckDefOf.VC_RagnaidFieldActivation);
            SoundDefOf.Broadshield_Startup.PlayOneShot((SoundInfo)new TargetInfo(projector.Position, projector.Map));
        }
    }
}
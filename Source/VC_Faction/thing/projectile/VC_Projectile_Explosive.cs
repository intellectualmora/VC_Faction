// Decompiled with JetBrains decompiler
// Type: Verse.Projectile_Explosive
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System.Reflection;
using Verse;

namespace VC_Faction
{
    public class VC_Projectile_Explosive : Projectile_Explosive
    {
        private int ticksToDetonation;
        private int exposeCount = 5;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation");
        }

        protected override void Tick()
        {
            base.Tick();
            if (this.exposeCount <= 0)
                return;
            --this.ticksToDetonation;
            if (this.ticksToDetonation > 0)
                return;
            if (this.landed == true)
            {
                ticksToDetonation = 30;
                exposeCount--;
                this.Explode();
            }
        }


        protected override void Explode()
        {
            Map map = base.Map;
            if (exposeCount == 0)
            {
                Destroy();
            }
            if (def.projectile.explosionEffect != null)
            {
                Effecter effecter = def.projectile.explosionEffect.Spawn();
                if (def.projectile.explosionEffectLifetimeTicks != 0)
                {
                    map.effecterMaintainer.AddEffecterToMaintain(effecter, base.Position.ToVector3().ToIntVec3(), def.projectile.explosionEffectLifetimeTicks);
                }
                else
                {
                    effecter.Trigger(new TargetInfo(base.Position, map), new TargetInfo(base.Position, map));
                    effecter.Cleanup();
                }
            }

            IntVec3 position = base.Position;
            float explosionRadius = def.projectile.explosionRadius;
            DamageDef damageDef = def.projectile.damageDef;
            Thing instigator = launcher;
            int damageAmount = DamageAmount;
            float armorPenetration = ArmorPenetration;
            SoundDef soundExplode = def.projectile.soundExplode;
            ThingDef weapon = equipmentDef;
            ThingDef projectile = def;
            Thing thing = intendedTarget.Thing;
            ThingDef postExplosionSpawnThingDef = def.projectile.postExplosionSpawnThingDef ?? def.projectile.filth;
            ThingDef postExplosionSpawnThingDefWater = def.projectile.postExplosionSpawnThingDefWater;
            float postExplosionSpawnChance = def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = def.projectile.postExplosionSpawnThingCount;
            GasType? postExplosionGasType = def.projectile.postExplosionGasType;
            ThingDef preExplosionSpawnThingDef = def.projectile.preExplosionSpawnThingDef;
            float preExplosionSpawnChance = def.projectile.preExplosionSpawnChance;
            int preExplosionSpawnThingCount = def.projectile.preExplosionSpawnThingCount;
            explosionRadius += 2*(5 - exposeCount)/5;
            GenExplosion.DoExplosion(applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef: preExplosionSpawnThingDef, preExplosionSpawnChance: preExplosionSpawnChance, preExplosionSpawnThingCount: preExplosionSpawnThingCount, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination), ignoredThings: null, affectedAngle: null, propagationSpeed: def.projectile.damageDef.expolosionPropagationSpeed, screenShakeFactor: def.projectile.screenShakeFactor, center: position, map: map, radius: explosionRadius, damType: damageDef, instigator: instigator, damAmount: damageAmount, armorPenetration: armorPenetration, explosionSound: soundExplode, weapon: weapon, projectile: projectile, intendedTarget: thing, postExplosionSpawnThingDef: postExplosionSpawnThingDef, postExplosionSpawnChance: postExplosionSpawnChance, postExplosionSpawnThingCount: postExplosionSpawnThingCount, postExplosionGasType: postExplosionGasType, doVisualEffects: def.projectile.doExplosionVFX, excludeRadius: 0f, doSoundEffects: true, postExplosionSpawnThingDefWater: postExplosionSpawnThingDefWater);

        }
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (blockedByShield || this.def.projectile.explosionDelay == 0)
            {
                this.exposeCount = 0;
                this.Explode();

            }
            else
            {
                this.landed = true;
                this.ticksToDetonation = this.def.projectile.explosionDelay;
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive((Thing)this, this.def.projectile.damageDef, this.launcher.Faction, this.launcher);
            }
        }
    }
}

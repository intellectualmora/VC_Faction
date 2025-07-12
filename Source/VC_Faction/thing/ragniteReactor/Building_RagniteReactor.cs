// Decompiled with JetBrains decompiler
// Type: RimWorld.Building_FermentingBarrel
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace VC_Faction
{
    [StaticConstructorOnStartup]
    public class Building_RagniteReactor : Building_WorkTable
    {
        private int ragniteCount;
        private float progressInt;
        private Material barFilledCachedMat;
        public const int MaxCapacity = 160;
        public int minRagniteCount = 8;
        private const int BaseRefineDuration = 180000;
        private const float increaseRate = 1f;
        private static readonly Vector2 BarSize = new Vector2(0.55f, 0.1f);
        private static readonly Color BarZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);
        private static readonly Color BarFermentedColor = new Color(0.9f, 0.85f, 0.2f);
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

        public float Progress
        {
            get => this.progressInt;
            set
            {
                if ((double)value == (double)this.progressInt)
                    return;
                this.progressInt = value;
                this.barFilledCachedMat = (Material)null;
            }
        }

        private Material BarFilledMat
        {
            get
            {
                if ((UnityEngine.Object)this.barFilledCachedMat == (UnityEngine.Object)null)
                    this.barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(Building_RagniteReactor.BarZeroProgressColor, Building_RagniteReactor.BarFermentedColor, this.Progress));
                return this.barFilledCachedMat;
            }
        }

        public int SpaceLeftForRagnite => !this.Refined ? MaxCapacity - this.ragniteCount : 0;

        private bool Empty => this.ragniteCount <= 0;

        public bool Refined => !this.Empty && (double)this.Progress >= 1.0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ragniteCount, "ragniteCount");
            Scribe_Values.Look<float>(ref this.progressInt, "progress");
        }

        public override void Tick()
        {
            base.Tick();
            if (this.Empty)
                return;
            CompPowerTrader powerComp = this.GetComp<CompPowerTrader>();
            if (powerComp != null && powerComp.PowerOn)
            {
                this.Progress = Mathf.Min(this.Progress + (increaseRate / BaseRefineDuration), 1f);
            }

            float radius = 0.6f * this.Progress * this.ragniteCount;
            if (this.HitPoints < this.MaxHitPoints * 0.2f && radius > 4f)
            {
                GenExplosion.DoExplosion(
                    this.Position,
                    this.Map,
                    radius, // 自定义半径
                    DamageDefOf.Bomb,
                    this
                );
            }
        }

        public void AddRagnite(int count)
        {
            if (this.Refined)
            {
                Log.Warning("请先取出拉格奈特精炼液");
            }
            else
            {
                int weightA = Mathf.Min(count, MaxCapacity - this.ragniteCount);
                if (weightA <= 0)
                    return;
                this.Progress = GenMath.WeightedAverage(0.0f, (float)weightA, this.Progress, (float)this.ragniteCount);
                this.ragniteCount += weightA;
            }
        }

        protected override void ReceiveCompSignal(string signal)
        {
            if (!(signal == "断电被毁"))
                return;
            this.Reset();
        }

        private void Reset()
        {
            this.ragniteCount = 0;
            this.Progress = 0.0f;
        }

        public void AddRagnite(Thing ragnite)
        {
            int count = Mathf.Min(ragnite.stackCount, MaxCapacity - this.ragniteCount);
            if (count <= 0)
                return;
            count = (count / 8) * 8;
            this.AddRagnite(count);
            ragnite.SplitOff(count).Destroy();
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length != 0)
                stringBuilder.AppendLine();
            if (!this.Empty)
            {
                if (this.Refined)
                    stringBuilder.AppendLine((string)"拉格奈特浓缩液");
                else
                    stringBuilder.AppendLine((string)"拉格奈特矿石");
            }
            if (!this.Empty)
            {
                if (this.Refined)
                {
                    stringBuilder.AppendLine((string)"拉格奈特精炼完成".Translate());
                }
                else
                {
                    stringBuilder.AppendLine((string)"拉格奈特精炼进度："+this.Progress.ToStringPercent());
                }
            }
            return stringBuilder.ToString().TrimEndNewlines();
        }

        public Thing TakeOutRagnite()
        {
            if (!this.Refined)
            {
                Log.Warning("没有精炼完毕无法获取精炼液。");
                return (Thing)null;
            }
            Thing liquidRagnite = ThingMaker.MakeThing(ThingDefOf.VC_LiquidRagnite);
            liquidRagnite.stackCount = this.ragniteCount/8;
            this.Reset();
            return liquidRagnite;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (this.Empty)
                return;
            Vector3 vector3 = drawLoc;
            vector3.y += 0.03846154f;
            vector3.z -= 0.25f;
            GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest()
            {
                center = vector3,
                size = Building_RagniteReactor.BarSize,
                fillPercent = (float)this.Progress,
                filledMat = this.BarFilledMat,
                unfilledMat = Building_RagniteReactor.BarUnfilledMat,
                margin = 0.1f,
                rotation = Rot4.North
            });
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            Building_RagniteReactor ragniteReactor = this;
            foreach (var gizmo in base.GetGizmos()) yield return gizmo;
            if (DebugSettings.ShowDevGizmos)
            {
                if (!ragniteReactor.Empty)
                {
                    Command_Action gizmo = new Command_Action();
                    gizmo.defaultLabel = "DEV: Set progress to 1";
                    gizmo.action = new Action(() =>
                    {
                        this.Progress = 1f;
                    });
                    yield return (Gizmo)gizmo;
                }
                if (ragniteReactor.SpaceLeftForRagnite > 0)
                {
                    Command_Action gizmo = new Command_Action();
                    gizmo.defaultLabel = "DEV: Fill";
                    gizmo.action = new Action(() =>
                    {
                        this.ragniteCount = MaxCapacity;
                    });
                    yield return (Gizmo)gizmo;
                }
            }
        }
    }
}

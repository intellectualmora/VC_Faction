// Decompiled with JetBrains decompiler
// Type: RimWorld.Building_WorkTable
// Assembly: Assembly-CSharp, Version=1.5.9214.33606, Culture=neutral, PublicKeyToken=null
// MVID: 630E2863-BC9A-4A34-93F2-EFF01E3A9556
// Assembly location: E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Verse;
using RimWorld;
using System.Linq;
using System.Text;
using System;
using UnityEngine;
namespace VC_Faction
{
    [StaticConstructorOnStartup]
    public class Building_WorkTable_Storage : Building_WorkTable, ISlotGroupParent,
    IStoreSettingsParent,
    IHaulDestination,
    IStorageGroupMember,
    IHaulEnroute,
    ILoadReferenceable
    {

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<BillStack>(ref this.billStack, "billStack", (object)this);
            Scribe_Deep.Look<StorageSettings>(ref this.settings, "settings", (object)this);
            Scribe_References.Look<StorageGroup>(ref this.storageGroup, "storageGroup");
            Scribe_Values.Look<string>(ref this.label, "label");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            this.cachedOccupiedCells = (List<IntVec3>)null;
            base.SpawnSetup(map, respawningAfterLoad);

            if (this.storageGroup == null || map == this.storageGroup.Map)
                return;
            StorageSettings storeSettings = this.storageGroup.GetStoreSettings();
            this.storageGroup.RemoveMember((IStorageGroupMember)this);
            this.storageGroup = (StorageGroup)null;
            this.settings.CopyFrom(storeSettings);
       
        }

        public StorageSettings settings;
        public StorageGroup storageGroup;
        public string label;
        public SlotGroup slotGroup;
        private List<IntVec3> cachedOccupiedCells;
        private static StringBuilder sb = new StringBuilder();

        public Building_WorkTable_Storage() {
            this.slotGroup = new SlotGroup((ISlotGroupParent)this);
            this.billStack = new BillStack((IBillGiver)this);
        }

        StorageGroup IStorageGroupMember.Group
        {
            get => this.storageGroup;
            set => this.storageGroup = value;
        }

        bool IStorageGroupMember.DrawConnectionOverlay => this.Spawned;

        Map IStorageGroupMember.Map => this.MapHeld;

        string IStorageGroupMember.StorageGroupTag => this.def.building.storageGroupTag;

        StorageSettings IStorageGroupMember.StoreSettings => this.GetStoreSettings();

        StorageSettings IStorageGroupMember.ParentStoreSettings => this.GetParentStoreSettings();

        StorageSettings IStorageGroupMember.ThingStoreSettings => this.settings;

        bool IStorageGroupMember.DrawStorageTab => true;

        bool IStorageGroupMember.ShowRenameButton => this.Faction == Faction.OfPlayer;

        public bool StorageTabVisible => true;

        public bool IgnoreStoredThingsBeauty => this.def.building.ignoreStoredThingsBeauty;

        public SlotGroup GetSlotGroup() => this.slotGroup;

        public virtual void Notify_ReceivedThing(Thing newItem)
        {
            if (this.Faction != Faction.OfPlayer || newItem.def.storedConceptLearnOpportunity == null)
                return;
            LessonAutoActivator.TeachOpportunity(newItem.def.storedConceptLearnOpportunity, OpportunityType.GoodToKnow);
        }

        public virtual void Notify_LostThing(Thing newItem)
        {
        }

        public virtual IEnumerable<IntVec3> AllSlotCells()
        {
            Building_WorkTable_Storage t = this;
            if (t.Spawned)
            {
                foreach (IntVec3 intVec3 in GenAdj.CellsOccupiedBy((Thing)t))
                    yield return intVec3;
            }
        }

        public List<IntVec3> AllSlotCellsList() => this.cachedOccupiedCells ?? (this.cachedOccupiedCells = this.AllSlotCells().ToList<IntVec3>());

        public StorageSettings GetStoreSettings() => this.storageGroup != null ? this.storageGroup.GetStoreSettings() : this.settings;

        public StorageSettings GetParentStoreSettings() => this.def.building.fixedStorageSettings ?? StorageSettings.EverStorableFixedSettings();

        public void Notify_SettingsChanged()
        {
            if (!this.Spawned || this.slotGroup == null)
                return;
            this.Map.listerHaulables.Notify_SlotGroupChanged(this.slotGroup);
        }

        public string SlotYielderLabel() => this.LabelCap;

        public string GroupingLabel => this.def.building.groupingLabel;

        public int GroupingOrder => this.def.building.groupingOrder;

        public bool HaulDestinationEnabled => true;

        public bool Accepts(Thing t) => this.GetStoreSettings().AllowedToAccept(t);

        public int SpaceRemainingFor(ThingDef _) => this.slotGroup.HeldThingsCount - this.def.building.maxItemsInCell * this.def.Size.Area;

        public override void PostMake()
        {
            base.PostMake();
            this.settings = new StorageSettings((IStoreSettingsParent)this);
            if (this.def.building.defaultStorageSettings == null)
                return;
            this.settings.CopyFrom(this.def.building.defaultStorageSettings);
        }

        

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
            this.cachedOccupiedCells = (List<IntVec3>)null;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            if (this.storageGroup != null)
            {
                this.storageGroup?.RemoveMember((IStorageGroupMember)this);
                this.storageGroup = (StorageGroup)null;
            }
            BillUtility.Notify_ISlotGroupRemoved((ISlotGroup)this.slotGroup);
        }


        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            StorageGroupUtility.DrawSelectionOverlaysFor((IStorageGroupMember)this);
        }

        public override string GetInspectString()
        {
            Building_WorkTable_Storage.sb.Clear();
            Building_WorkTable_Storage.sb.Append(base.GetInspectString());
            if (this.Spawned)
            {
                if (this.storageGroup != null)
                {
                    Building_WorkTable_Storage.sb.AppendLineIfNotEmpty();
                    Building_WorkTable_Storage.sb.Append(string.Format("{0}: {1} ", (object)"StorageGroupLabel".Translate(), (object)this.storageGroup.RenamableLabel.CapitalizeFirst()));
                    if (this.storageGroup.MemberCount > 1)
                        Building_WorkTable_Storage.sb.Append(string.Format("({0})", (object)"NumBuildings".Translate((NamedArgument)this.storageGroup.MemberCount)));
                    else
                        Building_WorkTable_Storage.sb.Append(string.Format("({0})", (object)"OneBuilding".Translate()));
                }
                if (this.slotGroup.HeldThings.Any<Thing>())
                {
                    Building_WorkTable_Storage.sb.AppendLineIfNotEmpty();
                    Building_WorkTable_Storage.sb.Append((string)"StoresThings".Translate());
                    Building_WorkTable_Storage.sb.Append(": ");
                    Building_WorkTable_Storage.sb.Append(this.slotGroup.HeldThings.Select<Thing, string>((Func<Thing, string>)(x => x.LabelShortCap)).Distinct<string>().ToCommaList());
                    Building_WorkTable_Storage.sb.Append(".");
                }
            }
            return Building_WorkTable_Storage.sb.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            foreach (Gizmo item in StorageSettingsClipboard.CopyPasteGizmosFor(GetStoreSettings()))
            {
                yield return item;
            }

            if (!StorageTabVisible || base.MapHeld == null)
            {
                yield break;
            }

            foreach (Gizmo item2 in StorageGroupUtility.StorageGroupMemberGizmos(this))
            {
                yield return item2;
            }

            if (Find.Selector.NumSelected != 1)
            {
                yield break;
            }

            foreach (Thing heldThing in slotGroup.HeldThings)
            {
                yield return ContainingSelectionUtility.CreateSelectStorageGizmo("CommandSelectStoredThing".Translate(heldThing), ("CommandSelectStoredThingDesc".Translate() + "\n\n" + heldThing.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + heldThing.GetInspectString()).Resolve(), heldThing, heldThing, groupable: false);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.ButterLib.HotKeys;
using TaleWorlds.InputSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;
using TaleWorlds.CampaignSystem.Party;

namespace TroopSpawner
{
    internal class TroopSpawnerKeyBase : HotKeyBase
    {
        private List<CharacterObject> shownTroops = new List<CharacterObject>();
        protected override string DisplayName { get; }
        protected override string Description { get; }
        protected override InputKey DefaultKey { get; }
        protected override string Category { get; }

        public TroopSpawnerKeyBase() : base(nameof(TroopSpawnerKeyBase))
        {
            DisplayName = "JustMatt's Troop Spawner";
            Description = "";
            DefaultKey = (InputKey)34;
            Category = HotKeyManager.Categories[(HotKeyCategory)1];
            Predicate = new Func<bool>(CanKeyBePressed);
        }

        protected bool CanKeyBePressed()
        {
            if (Campaign.Current != null && CharacterObject.PlayerCharacter != null && TaleWorlds.ScreenSystem.ScreenManager.TopScreen is SandBox.View.Map.MapScreen)
            {
                return true;
            }
            return false;
        }

        protected override void OnReleased()
        {
            List<InquiryElement> kingdomInquiries = new List<InquiryElement>();
            foreach (Kingdom kingdom in Kingdom.All.Where(k => !k.IsMinorFaction && k.IsKingdomFaction).ToList())
            {
                kingdomInquiries.Add(new InquiryElement(kingdom, kingdom.Name.ToString(), new ImageIdentifier(kingdom.Banner)));
            }
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Faction", "", kingdomInquiries, true, 1, "Next", "Cancel", (kingdom =>
            {
                GetKingdomTroops((Kingdom)kingdom.First().Identifier);
                DisplayTroops();
            }), null, ""), true);
        }

        private void GetKingdomTroops(Kingdom kingdom)
        {
            shownTroops.Clear();
            foreach (CharacterObject troop in CharacterObject.All.Where(troop => troop.IsSoldier && !troop.IsNotTransferableInPartyScreen && !troop.IsHero && troop.Culture.Equals(kingdom.Culture)))
            {
                shownTroops.Add(troop);
            }
        }

        private void GetTroopNextUpgrade(CharacterObject troop)
        {
            CharacterObject[] upgradeTargets = troop.UpgradeTargets;
            List<CharacterObject> HigherTroops = new List<CharacterObject>();
            if (upgradeTargets != null)
                HigherTroops = upgradeTargets.ToList();

            if (HigherTroops.Count > 0)
                return;

            shownTroops.AddRange(HigherTroops);
            foreach (CharacterObject soldier in HigherTroops)
                GetTroopNextUpgrade(soldier);
        }

        private void DisplayTroops()
        {
            List<InquiryElement> troopInquiries = new List<InquiryElement>();
            foreach (CharacterObject troop in shownTroops)
            {
                CharacterCode troopCode = CharacterCode.CreateFrom(troop);
                troopInquiries.Add(new InquiryElement(troop, troop.Name.ToString(), new ImageIdentifier(troopCode)));
            }
            MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData("Select Troop", "", troopInquiries, true, 1, "Next", "Cancel", (troop =>
            {
                AddTroopsToParty((CharacterObject)troop.First().Identifier);
            }), null, ""), true);
        }

        private void AddTroopsToParty(CharacterObject troop)
        {
            InformationManager.ShowTextInquiry(new TextInquiryData("Troop Count", "How many troops to add?", true, true, "Spawn", "Cancel", (response =>
            {
                int result;
                if (!int.TryParse(response, out result))
                    return;
                MobileParty.MainParty.AddElementToMemberRoster(troop, result, true);
            }), null, false, null, "", ""), true);
        }
    }
}

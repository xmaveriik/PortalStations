using System.Runtime.CompilerServices;
using HarmonyLib;
using JetBrains.Annotations;
using PortalStations.UI;
using UnityEngine;
using static PortalStations.PortalStationsPlugin;

namespace PortalStations.Stations;

public static class PersonalTeleportationDevice
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UseItem))]
    static class UsePersonalPortalDevice
    {
        [UsedImplicitly]
        private static bool Prefix(Humanoid __instance, ItemDrop.ItemData item)
        {
            if (!item.IsPortablePortal()) return true;
            UseItem(__instance, item);
            return false;
        }
    }
    
    public static bool IsPortablePortal(this ItemDrop.ItemData item) => item.m_shared.m_name == "$item_personal_teleportation_device";
    
    private static readonly ConditionalWeakTable<ItemDrop.ItemData, ExtraItemData> extraData = new();
    public static ExtraItemData GetExtraData(this ItemDrop.ItemData item) => extraData.GetOrCreateValue(item);

    private static void UseItem(Humanoid user, ItemDrop.ItemData item)
    {
        if (user is not Player player) return;
        if (item.m_durability < item.m_shared.m_durabilityDrain) return;
        PortalStationUI.instance?.Show(player, item);
        ExtraItemData data = item.GetExtraData();
        data.lastPosition = user.transform.position;
        data.hasLastPosition = true;
    }
    
    [UsedImplicitly]
    public class ExtraItemData
    {
        public Vector3 lastPosition;
        public bool hasLastPosition;
    }
}
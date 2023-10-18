using System;

namespace CauldronOnlineCommon.Data.Items
{
    [Serializable]
    public class RestrictedShopItemData : ShopItemData
    {
        public string TriggerEvent;
    }
}
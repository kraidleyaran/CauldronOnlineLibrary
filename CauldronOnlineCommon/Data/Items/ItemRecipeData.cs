using System;

namespace CauldronOnlineCommon.Data.Items
{
    [Serializable]
    public class ItemRecipeData
    {
        public string Item;
        public int Stack;
        public WorldItemStackData[] Recipe;
        public int Gold;
    }
}
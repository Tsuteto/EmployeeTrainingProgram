using System.Linq;
using BepInEx;
using EmployeeTraining.api;
using MyBox;
using SMS_PalletsDisplay;

namespace EmployeeTraining.PalletsDisplayPatch
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            DisplayModApi.RegisterHandler(new PalletsDisplayHandler());
        }
    }

    

    public class PalletsDisplayHandler : IModdedDisplayHandler
    {
        public bool IsTargetDisplay(DisplaySlot displaySlot)
        {
                Display componentInParent = displaySlot.GetComponentInParent<Display>();
                DisplayType? displayType = (componentInParent != null) ? new DisplayType?(componentInParent.DisplayType) : null;
                DisplayType furnitureID = (DisplayType)Helper.FurnitureID;
                return displayType.GetValueOrDefault() == furnitureID & displayType != null;
        }

        public int GetProductCountOfGridLayout(DisplaySlot displaySlot) {
            PalletProduct palletProduct = Singleton<PalletProductManager>.Instance.PalletProducts
                    .First((PalletProduct pp) => pp.ProductSO.ID == displaySlot.Data.FirstItemID);
            return palletProduct.GridLayoutOnPallet.productCount;
        }
    }
}

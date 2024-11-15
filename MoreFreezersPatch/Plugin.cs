using System.Reflection;
using BepInEx;
using EmployeeTraining.api;
using MyBox;

#pragma warning disable IDE0130
namespace EmployeeTraining.MoreFreezersPatch
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            DisplayModApi.RegisterHandler(new MoreFreezersHandler());
        }
    }

    public class MoreFreezersHandler : IModdedDisplayHandler
    {
        private readonly FieldInfo fldID = typeof(Display).GetField("m_ID", BindingFlags.Instance | BindingFlags.NonPublic);

        public bool IsTargetDisplay(DisplaySlot displaySlot)
        {
            var id = (int)fldID.GetValue(displaySlot.Display);
            return id == 589897 || id == 589891;
        }

        public int GetProductCountOfGridLayout(DisplaySlot displaySlot) {
            ProductSO productSO = Singleton<IDManager>.Instance.ProductSO(displaySlot.Data.FirstItemID);
            return productSO.GridLayoutInStorage.productCount * 2;
        }
    }

}

using TaleWorlds.MountAndBlade;
using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace TroopSpawner
{
    public class SubModule : MBSubModuleBase
    {
        private bool moduleLaunch;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {

            if (!this.moduleLaunch)
            {
                this.moduleLaunch = true;
                HotKeyManager hotKeyManager = HotKeyManager.Create("TroopSpawner");
                if (hotKeyManager != null)
                {
                    hotKeyManager.Add<TroopSpawnerKeyBase>();
                    hotKeyManager.Build();
                }
            }
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }
    }
}
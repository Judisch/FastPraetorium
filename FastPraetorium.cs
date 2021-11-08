using System;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Configuration;

namespace FastPraetorium
{
    public partial class FastPraetorium : IDalamudPlugin
    {
        public string Name => "FastPraetorium";

        private readonly DalamudPluginInterface _pluginInterface;
        [PluginService] [RequiredVersion("1.0")] public static TargetManager TargetManager { get; private set; } = null!;

        public FastPraetorium(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
            _pluginInterface.UiBuilder.Draw += CheckTargetChanged;
        }

        public void Dispose()
        {
            _pluginInterface.UiBuilder.Draw -= CheckTargetChanged;
            GC.SuppressFinalize(this);
        }

        private uint previousTargetID = 0;

        private void CheckTargetChanged()
        {
            var currentTarget = TargetManager.Target?.DataId ?? 0;

            if (currentTarget != previousTargetID)
            {
                OnTargetChanged(currentTarget);
                previousTargetID = currentTarget;
            }
        }

        private const uint NERO_TOL_SCAEVA = 1437;
        private const uint GAIUS_VAN_BAELSAR = 1441;
        private const uint THE_ULTIMA_WEAPON = 1974;
        private const uint THE_ULTIMA_WEAPON_AGAIN = 1975;
        private const uint LAHABREA = 1976;

        private static void OnTargetChanged(uint id)
        {
            switch (id)
            {
                case NERO_TOL_SCAEVA:
                    ChangeCutsceneAudioLanguage(LANGUAGE_JAPANESE);
                    break;

                case GAIUS_VAN_BAELSAR:
                    ChangeCutsceneAudioLanguage(LANGUAGE_GERMAN);
                    break;

                case THE_ULTIMA_WEAPON:
                    ChangeCutsceneAudioLanguage(LANGUAGE_FRENCH);
                    break;

                case THE_ULTIMA_WEAPON_AGAIN:
                    ChangeCutsceneAudioLanguage(LANGUAGE_GERMAN);
                    break;

                case LAHABREA:
                    ChangeCutsceneAudioLanguage(LANGUAGE_ADJUST_TO_CLIENT);
                    break;
            }
        }

        private const uint LANGUAGE_ADJUST_TO_CLIENT = 4294967295;
        private const uint LANGUAGE_JAPANESE = 0;
        private const uint LANGUAGE_ENGLISH = 1;
        private const uint LANGUAGE_GERMAN = 2;
        private const uint LANGUAGE_FRENCH = 3;

        private const uint CONFIG_INDEX_CUTSCENEAUDIOVOICE = 792;

        private const int CONFIG_ENTRY_SIZE = 0x38;

        private static unsafe void ChangeCutsceneAudioLanguage(uint language)
        {
            var systemConfig = (ConfigBase*)(FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->SystemConfig);
            var configArray = systemConfig->ConfigEntry;
            var length = systemConfig->ConfigCount;

            for (int i = 0; i < length; i++)
            {
                IntPtr p = new IntPtr((byte*)configArray + (i * CONFIG_ENTRY_SIZE));
                if (((ConfigEntry*)p)->Index == CONFIG_INDEX_CUTSCENEAUDIOVOICE)
                {
                    ((ConfigEntry*)p)->Value.UInt = language;
                }
            }
        }
    }
}

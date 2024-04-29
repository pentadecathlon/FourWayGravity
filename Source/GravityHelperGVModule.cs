﻿using System;

namespace Celeste.Mod.GravityHelperGV;

public class GravityHelperGVModule : EverestModule {
    public static GravityHelperGVModule Instance { get; private set; }

    public override Type SettingsType => typeof(GravityHelperGVModuleSettings);
    public static GravityHelperGVModuleSettings Settings => (GravityHelperGVModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(GravityHelperGVModuleSession);
    public static GravityHelperGVModuleSession Session => (GravityHelperGVModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(GravityHelperGVModuleSaveData);
    public static GravityHelperGVModuleSaveData SaveData => (GravityHelperGVModuleSaveData) Instance._SaveData;

    public GravityHelperGVModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(GravityHelperGVModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(GravityHelperGVModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        GravityComponent.SetHooks();
        ControlHooks.Load();
        PlayerHooks.Load();
        SolidHooks.Load();
        PlayerHairHooks.Load();
    }
    public override void Unload() {
        PlayerHooks.Unload();
        SolidHooks.Unload();
        ControlHooks.Unload();
        GravityComponent.RemoveHooks();
        PlayerHairHooks.Unload();
    }
}

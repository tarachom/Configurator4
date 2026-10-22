/*
Copyright (C) 2019-2026 TARAKHOMYN YURIY IVANOVYCH
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

/*
Автор:    Тарахомин Юрій Іванович
Адреса:   Україна, м. Львів
Сайт:     accounting.org.ua
*/

using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtkLib;

namespace Configurator;

/// <summary>
/// Налаштування
/// </summary>
[Subclass<Box>("PageSettings")]
[Template<AssemblyResource>("PageSettings.ui")]
public partial class PageSettings
{
    Configuration Conf { get; } = Program.Kernel.Conf;
    GlobalConfigurationParam? GlobalConfigurationParam { get; } = Program.BasicForm?.GlobalConfigurationParam;
    ConfigurationParam? OpenConfigurationParam { get; } = Program.BasicForm?.OpenConfigurationParam;

    #region Fields

    [Connect("entry_ai_model")] Entry entryAiModel;
    [Connect("entry_ai_key")] Entry entryAiKey;
    [Connect("switch_autostart")] Switch switchAutostart;
    [Connect("button_save")] Button buttonSave;

    #endregion

    public static PageSettings New() => NewWithProperties([]);

    partial void Initialize()
    {
        buttonSave.OnClicked += (_, _) => SaveSettings();
    }

    public void SetValue()
    {
        if (GlobalConfigurationParam != null)
        {
            entryAiModel.SetText(GlobalConfigurationParam.AIModel);
            entryAiKey.SetText(GlobalConfigurationParam.AIKey);
            switchAutostart.Active = GlobalConfigurationParam.AIStartOnRun;
        }
    }

    void SaveSettings()
    {
        if (GlobalConfigurationParam != null)
        {
            GlobalConfigurationParam.AIModel = entryAiModel.GetText();
            GlobalConfigurationParam.AIKey = entryAiKey.GetText();
            GlobalConfigurationParam.AIStartOnRun = switchAutostart.Active;

            ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);
        }
    }
}
using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtkLib;

namespace Configurator;

/// <summary>
/// 
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

    public static PageSettings New()
    {
        PageSettings w = NewWithProperties([]);
        return w;
    }

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
using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

namespace Configurator;

/// <summary>
/// 
/// </summary>
[Subclass<Box>("PageConfigurationInfo")]
[Template<AssemblyResource>("PageConfigurationInfo.ui")]
public partial class PageConfigurationInfo
{
    Configuration Conf { get; } = Program.Kernel.Conf;

    #region Fields

    [Connect("entry_name")] Entry entryName;
    [Connect("entry_subtitle")] Entry entrySubtitle;
    [Connect("entry_namespace_generated")] Entry entryNamespaceGenerated;
    [Connect("entry_namespace_program")] Entry entryNamespaceProgram;
    [Connect("entry_author")] Entry entryAuthor;
    [Connect("textview_desc")] TextView textviewDesc;
    [Connect("dropdown_gtk_version")] DropDownControl dropdownGtkVersion;
    [Connect("button_save")] Button buttonSave;

    #endregion

    public static PageConfigurationInfo New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();
        return NewWithProperties([]);
    }

    partial void Initialize()
    {
        dropdownGtkVersion.AllowEmpty = false;

        foreach (var name in Enum.GetNames<Configuration.GtkVersion>())
            dropdownGtkVersion.Append(name);

        buttonSave.OnClicked += (_, _) => SaveSettings();
    }

    public void SetValue()
    {
        entryName.SetText(Conf.Name);
        entrySubtitle.SetText(Conf.Subtitle);
        entryNamespaceGenerated.SetText(Conf.NameSpaceGeneratedCode);
        entryNamespaceProgram.SetText(Conf.NameSpace);
        entryAuthor.SetText(Conf.Author);
        textviewDesc.Buffer?.SetText(Conf.Desc, -1);
        dropdownGtkVersion.Value = Conf.GtkLibVersion.ToString();
    }

    void SaveSettings()
    {
        Conf.Name = entryName.GetText();
        Conf.Subtitle = entrySubtitle.GetText();
        Conf.NameSpaceGeneratedCode = entryNamespaceGenerated.GetText();
        Conf.NameSpace = entryNamespaceProgram.GetText();
        Conf.Author = entryAuthor.GetText();
        Conf.Desc = textviewDesc.Buffer?.Text ?? "";
        Conf.GtkLibVersion = Enum.Parse<Configuration.GtkVersion>(dropdownGtkVersion.Value);
    }
}
using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;

[Subclass<Box>("DirectorySubordination")]
[Template<AssemblyResource>("DirectorySubordination.xml")]
public partial class DirectorySubordination
{
    [Connect("dropdown_directory_owner")] DropDown dropdownDirectoryOwner;
    [Connect("dropdown_pointer_field_owner")] DropDown dropdownPointerFieldOwner;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    string DirectoryOwner, PointerFieldOwner = "";

    public static DirectorySubordination New()
    {
        DirectorySubordination widget = NewWithProperties([]);
        return widget;
    }

    partial void Initialize()
    {
        dropdownDirectoryOwner.OnNotify += (_, e) =>
        {
            if (e.Pspec.GetName() == "selected")
                FillDirectoryField();
        };
    }

    void FillDirectoryField()
    {
        if (dropdownDirectoryOwner.Selected > 0)
        {
            var pointer = (dropdownDirectoryOwner.SelectedItem as StringObject)?.String ?? "";

            string[] items = [.. ConfDirectory.Fields.Values
                .Where(x => x.Type == "pointer" && x.Pointer == pointer)
                .Select(x => x.Name)];

            dropdownPointerFieldOwner.Model = StringList.New(items);
        }
    }

    void FillDirectory()
    {
        string[] items =
        [
            "-- Без власника --",
            .. Conf.Directories.Values.Select(x => $"Довідники.{x.Name}")
        ];

        dropdownDirectoryOwner.Model = StringList.New(items);
    }

    public static void Select(DropDown dropDown, string text)
    {
        if (dropDown.Model is StringList model)
            for (uint i = 0; i < model.GetNItems(); i++)
            {
                if (model.GetString(i) == text)
                {
                    dropDown.SetSelected(i);
                    break;
                }
            }
    }

    public void SetValue(ConfigurationDirectories confDirectory)
    {
        ConfDirectory = confDirectory;

        DirectoryOwner = ConfDirectory.DirectoryOwner_Subordination;
        PointerFieldOwner = ConfDirectory.PointerFieldOwner_Subordination;

        FillDirectory();

        if (!string.IsNullOrEmpty(DirectoryOwner))
            Select(dropdownDirectoryOwner, DirectoryOwner);
    }

    public void GetValue()
    {
        //Індекс 0 завжди для "-- Без власника --"
        uint index = dropdownDirectoryOwner.Selected;

        ConfDirectory.DirectoryOwner_Subordination = index switch
        {
            > 0 => (dropdownDirectoryOwner.SelectedItem as StringObject)?.String ?? "",
            _ => ""
        };

        ConfDirectory.PointerFieldOwner_Subordination = index switch
        {
            > 0 => (dropdownPointerFieldOwner.SelectedItem as StringObject)?.String ?? "",
            _ => ""
        };
    }
}
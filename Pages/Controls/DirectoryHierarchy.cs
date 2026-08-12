using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;

[Subclass<Box>("DirectoryHierarchy")]
[Template<AssemblyResource>("DirectoryHierarchy.xml")]
public partial class DirectoryHierarchy
{
    [Connect("dropdown_directory_type")] DropDown dropdownDirectoryType;
    [Connect("dropdown_parent_field")] DropDown dropdownParentField;
    [Connect("dropdown_allowed_content")] DropDown dropdownAllowedContent;
    [Connect("dropdown_is_folder_field")] DropDown dropdownIsFolderField;
    [Connect("dropdown_hierarchy_directory")] DropDown dropdownHierarchyDirectory;
    [Connect("button_create")] Button buttonCreate;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    ConfigurationDirectories.TypeDirectories TypeDirectory = ConfigurationDirectories.TypeDirectories.Normal;
    ConfigurationDirectories.HierarchicalContentType AllowedContent = ConfigurationDirectories.HierarchicalContentType.FoldersAndElements;
    string ParentField = "", IsFolderField = "", PointerFolders = "";

    public static DirectoryHierarchy New()
    {
        DirectoryHierarchy widget = NewWithProperties([]);
        return widget;
    }

    partial void Initialize()
    {
        dropdownDirectoryType.OnNotify += (_, e) =>
        {
            if (e.Pspec.GetName() == "selected")
            {
                TypeDirectory = (ConfigurationDirectories.TypeDirectories)(dropdownDirectoryType.Selected + 1);

                SensitiveFields();
                SensetiveIsFolderField();
            }
        };

        dropdownAllowedContent.OnNotify += (_, e) =>
        {
            if (e.Pspec.GetName() == "selected")
            {
                AllowedContent = (ConfigurationDirectories.HierarchicalContentType)dropdownAllowedContent.Selected;

                SensetiveIsFolderField();
            }
        };

        buttonCreate.OnClicked += (_, _) =>
        {

        };
    }

    void FillFields()
    {
        List<string> parent = [], isfolder = [];

        foreach (ConfigurationField field in ConfDirectory.Fields.Values)
        {
            //Поля для ієрархії
            if (field.Type == "pointer" && field.Pointer == $"Довідники.{ConfDirectory.Name}")
                parent.Add(field.Name);

            //Поля для ЦеПапка
            if (field.Type == "boolean")
                isfolder.Add(field.Name);
        }

        dropdownParentField.Model = StringList.New([.. parent]);
        dropdownIsFolderField.Model = StringList.New([.. isfolder]);
    }

    void FillPointerFolders()
    {
        string[] items = [.. Conf.Directories.Values
           .Where(x => x.TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical)
           .Select(x => $"Довідники.{x.Name}")];

        dropdownHierarchyDirectory.Model = StringList.New(items);
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

        TypeDirectory = ConfDirectory.TypeDirectory;
        AllowedContent = ConfDirectory.AllowedContent_Hierarchical;
        ParentField = ConfDirectory.ParentField_Hierarchical;
        IsFolderField = ConfDirectory.IsFolderField_Hierarchical;
        PointerFolders = ConfDirectory.PointerFolders_HierarchyInAnotherDirectory;

        dropdownDirectoryType.Selected = (uint)(TypeDirectory - 1);
        dropdownAllowedContent.Selected = (uint)AllowedContent;

        FillFields();
        FillPointerFolders();

        if (TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical)
        {
            Select(dropdownParentField, ParentField);
            Select(dropdownIsFolderField, IsFolderField);
        }

        if (TypeDirectory == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory)
            Select(dropdownHierarchyDirectory, PointerFolders);

        SensitiveFields();
        SensetiveIsFolderField();
    }

    public void GetValue()
    {
        ConfDirectory.TypeDirectory = (ConfigurationDirectories.TypeDirectories)(dropdownDirectoryType.Selected + 1);
        ConfDirectory.AllowedContent_Hierarchical = (ConfigurationDirectories.HierarchicalContentType)dropdownAllowedContent.Selected;
        ConfDirectory.ParentField_Hierarchical = (dropdownParentField.SelectedItem as StringObject)?.String ?? "";
        ConfDirectory.IsFolderField_Hierarchical = (dropdownIsFolderField.SelectedItem as StringObject)?.String ?? "";
        ConfDirectory.PointerFolders_HierarchyInAnotherDirectory = (dropdownHierarchyDirectory.SelectedItem as StringObject)?.String ?? "";
    }

    /// <summary>
    /// Метод для керування доступністю полів (Sensitive) залежно від вибору типу довідника
    /// </summary>
    void SensitiveFields()
    {
        //Вибір поля тільки для Hierarchical
        dropdownParentField.Sensitive = dropdownAllowedContent.Sensitive =
            TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical;

        //Вибір папки тільки якщо HierarchyInAnotherDirectory
        dropdownHierarchyDirectory.Sensitive = TypeDirectory == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory;

        //Кнопка працює тільки Hierarchical або HierarchyInAnotherDirectory
        buttonCreate.Sensitive =
            TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical ||
            TypeDirectory == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory;
    }

    void SensetiveIsFolderField()
    {
        dropdownIsFolderField.Sensitive =
            TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical &&
            AllowedContent == ConfigurationDirectories.HierarchicalContentType.FoldersAndElements;
    }
}
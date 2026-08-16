using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

[Subclass<Box>("DirectoryHierarchy")]
[Template<AssemblyResource>("DirectoryHierarchy.xml")]
public partial class DirectoryHierarchy
{
    [Connect("dropdown_directory_type")] DropDownControl dropdownDirectoryType;
    [Connect("dropdown_parent_field")] DropDownControl dropdownParentField;
    [Connect("dropdown_allowed_content")] DropDownControl dropdownAllowedContent;
    [Connect("dropdown_is_folder_field")] DropDownControl dropdownIsFolderField;
    [Connect("dropdown_hierarchy_directory")] DropDownControl dropdownHierarchyDirectory;
    [Connect("button_create")] Button buttonCreate;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    ConfigurationDirectories.TypeDirectories TypeDirectory = ConfigurationDirectories.TypeDirectories.Normal;
    ConfigurationDirectories.HierarchicalContentType AllowedContent = ConfigurationDirectories.HierarchicalContentType.FoldersAndElements;

    public static DirectoryHierarchy New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();

        DirectoryHierarchy widget = NewWithProperties([]);
        return widget;
    }

    partial void Initialize()
    {
        dropdownDirectoryType.AllowEmpty = false;
        dropdownDirectoryType.Fill(ConfigurationDirectories.TypeDirectories_Dict());
        dropdownDirectoryType.OnСhanged = () =>
        {
            TypeDirectory = Enum.Parse<ConfigurationDirectories.TypeDirectories>(dropdownDirectoryType.Value);
            SensitiveFields();
            SensetiveIsFolderField();
        };

        dropdownAllowedContent.AllowEmpty = false;
        dropdownAllowedContent.Fill(ConfigurationDirectories.HierarchicalContentType_Dict());
        dropdownAllowedContent.OnСhanged = () =>
        {
            AllowedContent = Enum.Parse<ConfigurationDirectories.HierarchicalContentType>(dropdownAllowedContent.Value);
            SensetiveIsFolderField();
        };

        buttonCreate.OnClicked += async (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDirectory.Name) || !Conf.Directories.ContainsKey(ConfDirectory.Name))
            {
                Message.Error(Program.BasicForm, "Назва довідника не задана або довідник не збережений! Довідник потрібно спочатку зберегти!");
                return;
            }

            if (TypeDirectory == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory)
            {
                string newConfDirectoryName = ConfDirectory.Name + "_Папки",
                    newConfDirectoryFullName = ConfDirectory.Name + " Папки",
                    newConfDirectoryType = "Довідники." + newConfDirectoryName,
                    folderFieldName = Function.FindNewFieldName(ConfDirectory.Fields, "Папка", (x) => !(x.Type == "pointer" && x.Pointer == newConfDirectoryType));

                //Перевірити і створити новий довідник для ієрархії
                if (!Conf.Directories.ContainsKey(newConfDirectoryName))
                {
                    ConfigurationDirectories newConfDirectory = new()
                    {
                        Name = newConfDirectoryName,
                        FullName = newConfDirectoryFullName,
                        Desc = newConfDirectoryFullName,
                        TypeDirectory = ConfigurationDirectories.TypeDirectories.Hierarchical,
                        AllowedContent_Hierarchical = ConfigurationDirectories.HierarchicalContentType.Folders,
                        ParentField_Hierarchical = "Папка"
                    };

                    List<ConfigurationField> otherFields = [
                        //Поле Папка яке має тип довідника який ми додаємо
                        new ConfigurationField("Папка", "Папка", "", "pointer", newConfDirectoryType, "Папка", false, true)
                    ];

                    _ = await Function.FillNewDirectory(newConfDirectory, otherFields);

                    Conf.AppendDirectory(newConfDirectory);
                    FillPointerFolders();

                    //Встановити створений довідник як вибраний у списку
                    dropdownHierarchyDirectory.Value = newConfDirectoryType;
                }
                else
                    dropdownHierarchyDirectory.Value = newConfDirectoryType;

                //Додати нове поле Папка в основний довідник
                if (!ConfDirectory.Fields.ContainsKey(folderFieldName))
                {
                    string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                    ConfDirectory.AppendField(new ConfigurationField(folderFieldName, "Папка", nameInTable, "pointer", newConfDirectoryType, "Папка", false, true));
                }
            }
            else if (TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical)
            {
                string confDirectoryType = "Довідники." + ConfDirectory.Name;
                string folderFieldName = Function.FindNewFieldName(ConfDirectory.Fields, "Папка", (x) => !(x.Type == "pointer" && x.Pointer == confDirectoryType));
                string isFolderFieldName = Function.FindNewFieldName(ConfDirectory.Fields, "ЦеПапка", (x) => x.Type != "boolean");

                if (!ConfDirectory.Fields.ContainsKey(folderFieldName))
                {
                    // Папка
                    string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                    ConfDirectory.AppendField(new ConfigurationField(folderFieldName, "Папка", nameInTable, "pointer", confDirectoryType, "Папка", false, true));
                }

                if (AllowedContent == ConfigurationDirectories.HierarchicalContentType.FoldersAndElements && !ConfDirectory.Fields.ContainsKey(isFolderFieldName))
                {
                    // ЦеПапка
                    string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                    ConfDirectory.AppendField(new ConfigurationField(isFolderFieldName, "Це папка", nameInTable, "boolean", "", "Це папка", false, true));
                }

                FillFields();

                dropdownParentField.Value = folderFieldName;
                dropdownIsFolderField.Value = isFolderFieldName;
            }
        };
    }

    /// <summary>
    /// Підбір імені для поля
    /// Пошук поля із заданою назвою. 
    /// Якщо інший тип даних тоді до назви поля додається цифра від 1 до 10
    /// </summary>
    /*string FindNewFieldName(string fieldName, Func<ConfigurationField, bool> func)
    {
        string newFieldName = fieldName;
        for (int i = 1; i <= 10; i++)
            if (ConfDirectory.Fields.TryGetValue(newFieldName, out var field))
            {
                if (func.Invoke(field))
                    newFieldName = fieldName + i.ToString();
                else
                    break;
            }
            else
                break;

        return newFieldName;
    }*/

    void FillFields()
    {
        dropdownParentField.RemoveAll();
        dropdownIsFolderField.RemoveAll();

        foreach (ConfigurationField field in ConfDirectory.Fields.Values)
        {
            //Поля для ієрархії
            if (field.Type == "pointer" && field.Pointer == $"Довідники.{ConfDirectory.Name}")
                dropdownParentField.Append(field.Name);

            //Поля для ЦеПапка
            if (field.Type == "boolean")
                dropdownIsFolderField.Append(field.Name);
        }
    }

    void FillPointerFolders()
    {
        dropdownHierarchyDirectory.RemoveAll();

        foreach (var item in Conf.Directories.Values)
            if (item.TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical)
                dropdownHierarchyDirectory.Append($"Довідники.{item.Name}", item.Name);
    }

    public void SetValue(ConfigurationDirectories confDirectory)
    {
        ConfDirectory = confDirectory;

        FillFields();
        FillPointerFolders();

        dropdownDirectoryType.Value = (TypeDirectory = ConfDirectory.TypeDirectory).ToString();
        dropdownAllowedContent.Value = (AllowedContent = ConfDirectory.AllowedContent_Hierarchical).ToString();
        dropdownParentField.Value = ConfDirectory.ParentField_Hierarchical;
        dropdownIsFolderField.Value = ConfDirectory.IsFolderField_Hierarchical;
        dropdownHierarchyDirectory.Value = ConfDirectory.PointerFolders_HierarchyInAnotherDirectory;

        SensitiveFields();
        SensetiveIsFolderField();
    }

    public void GetValue()
    {
        ConfDirectory.TypeDirectory = Enum.Parse<ConfigurationDirectories.TypeDirectories>(dropdownDirectoryType.Value);
        ConfDirectory.AllowedContent_Hierarchical = Enum.Parse<ConfigurationDirectories.HierarchicalContentType>(dropdownAllowedContent.Value);
        ConfDirectory.ParentField_Hierarchical = dropdownParentField.Value;
        ConfDirectory.IsFolderField_Hierarchical = dropdownIsFolderField.Value;
        ConfDirectory.PointerFolders_HierarchyInAnotherDirectory = dropdownHierarchyDirectory.Value;
    }

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
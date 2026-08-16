using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;
using InterfaceGtk4;

[Subclass<Box>("DirectorySubordination")]
[Template<AssemblyResource>("DirectorySubordination.xml")]
public partial class DirectorySubordination
{
    [Connect("dropdown_directory_owner")] DropDownControl dropdownDirectoryOwner;
    [Connect("dropdown_pointer_field_owner")] DropDownControl dropdownPointerFieldOwner;
    [Connect("button_create")] Button buttonCreate;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    public static DirectorySubordination New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();

        DirectorySubordination widget = NewWithProperties([]);
        return widget;
    }

    partial void Initialize()
    {
        dropdownDirectoryOwner.OnСhanged = () => FillDirectoryField();

        buttonCreate.OnClicked += async (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDirectory.Name) || !Conf.Directories.ContainsKey(ConfDirectory.Name))
            {
                Message.Error(Program.BasicForm, "Назва довідника не задана або довідник не збережений! Довідник потрібно спочатку зберегти!");
                return;
            }

            if (!string.IsNullOrEmpty(dropdownDirectoryOwner.Value))
            {
                var (Result, _, PointerType) = Configuration.PointerParse(dropdownDirectoryOwner.Value, out var _);
                if (Result)
                {
                    string fieldName = Function.FindNewFieldName(ConfDirectory.Fields, PointerType, (x) => !(x.Type == "pointer" && x.Pointer == dropdownDirectoryOwner.Value));

                    if (!ConfDirectory.Fields.ContainsKey(fieldName))
                    {
                        string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                        ConfDirectory.AppendField(new ConfigurationField(fieldName, "", nameInTable, "pointer", dropdownDirectoryOwner.Value, "", false, true));

                        FillDirectoryField();
                    }

                    dropdownPointerFieldOwner.Value = fieldName;
                }
            }
        };
    }

    void FillDirectoryField()
    {
        dropdownPointerFieldOwner.RemoveAll();

        var pointer = dropdownDirectoryOwner.Value;
        bool selectFirstField = true;

        foreach (var item in ConfDirectory.Fields.Values)
            if (item.Type == "pointer" && item.Pointer == pointer)
            {
                dropdownPointerFieldOwner.Append(item.Name);

                //Вибрати зразу перше значення
                if (selectFirstField)
                {
                    dropdownPointerFieldOwner.Value = item.Name;
                    selectFirstField = false;
                }
            }
    }

    void FillDirectory()
    {
        foreach (var item in Conf.Directories.Values)
            dropdownDirectoryOwner.Append($"Довідники.{item.Name}", item.Name);
    }

    public void SetValue(ConfigurationDirectories confDirectory)
    {
        ConfDirectory = confDirectory;

        FillDirectory();

        dropdownDirectoryOwner.Value = ConfDirectory.DirectoryOwner_Subordination;
        dropdownPointerFieldOwner.Value = ConfDirectory.PointerFieldOwner_Subordination;
    }

    public void GetValue()
    {
        ConfDirectory.DirectoryOwner_Subordination = dropdownDirectoryOwner.Value;
        ConfDirectory.PointerFieldOwner_Subordination = dropdownPointerFieldOwner.Value;
    }
}
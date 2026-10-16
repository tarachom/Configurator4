/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageField))]
partial class PageField : FormPageConfigurator
{
    public ConfigurationField ConfField { get; set; } = new();
    public Dictionary<string, ConfigurationField> Fields = [];
    Configuration Conf { get; } = Program.Kernel.Conf;
    string ParentTable { get; set; } = "";
    Dictionary<string, ConfigurationField> AllFields = [];

    BasicFields basicFields = BasicFields.New();
    Field field = Field.New();

    partial void Initialize()
    {
        PageName = "Поле:";
        basicFields.TableOrColumnLabel = "В таблиці:";
        basicFields.RenameTableOrColumn.RenameFunc = () => basicFields.RenameTableOrColumn.RenameFieldDef(ParentTable);
    }

    public static PageField New()
    {
        PageField w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);
    }

    protected override void CreateEnd(Box vBox)
    {
        //Інші
        vBox.Append(field);
    }

    public override async Task AssignValue()
    {
        ParentTable = Owner?.Group switch
        {
            ConfiguratorItemOwnerType.Directory => Owner.Obj is ConfigurationDirectories x ? x.Table : "",
            ConfiguratorItemOwnerType.Document => Owner.Obj is ConfigurationDocuments x ? x.Table : "",
            ConfiguratorItemOwnerType.TablePart => Owner.Obj is ConfigurationTablePart x ? x.Table : "",
            ConfiguratorItemOwnerType.RegisterInformation => Owner.Obj is ConfigurationRegistersInformation x ? x.Table : "",
            ConfiguratorItemOwnerType.RegisterAccumulation => Owner.Obj is ConfigurationRegistersAccumulation x ? x.Table : "",
            _ => ""
        };

        AllFields = Owner?.Group switch
        {
            ConfiguratorItemOwnerType.RegisterInformation => Owner.Obj is ConfigurationRegistersInformation x ? Configuration.CombineAllFieldForRegister(x) : throw new Exception(),
            ConfiguratorItemOwnerType.RegisterAccumulation => Owner.Obj is ConfigurationRegistersAccumulation x ? Configuration.CombineAllFieldForRegister(x) : throw new Exception(),
            _ => Fields
        };

        if (IsNew)
            _ = await Function.FillNewField(ConfField, ParentTable, AllFields);

        basicFields.ItemName = ConfField.Name;
        basicFields.FullName = ConfField.FullName;
        basicFields.TableOrColumn = ConfField.NameInTable;
        basicFields.Desc = ConfField.Desc;

        field.SetValue(ConfField);
    }

    protected override async Task GetValue()
    {
        ConfField.Name = basicFields.ItemName;
        ConfField.FullName = basicFields.FullName;
        ConfField.NameInTable = basicFields.TableOrColumn;
        ConfField.Desc = basicFields.Desc;

        field.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfField.Name, [.. AllFields.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Fields.Remove(ConfField.Name);
        }
        else
            return false;

        await GetValue();

        if ((ConfField.Type == "pointer" || ConfField.Type == "enum") && string.IsNullOrEmpty(ConfField.Pointer))
        {
            Message.Error(Program.BasicForm, "Не заповненні поля", "Потрібна деталізація для типів [ pointer ] або [ enum ]\nВиберіть із списків вказівник або перелічення!");
            return false;
        }

        Fields.Add(ConfField.Name, ConfField);

        Caption = ConfField.Name;
        IsNew = false;

        return true;
    }
}

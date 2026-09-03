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
    public override Configuration Conf { get; } = Program.Kernel.Conf;
    public string ParentTable { get; set; } = "";
    public ConfigurationField ConfField { get; set; } = new();
    public Dictionary<string, ConfigurationField> Fields = [];

    BasicFields basicFields = BasicFields.New();
    Field field = Field.New();

    partial void Initialize()
    {
        basicFields.TableOrColumnLabel = "В таблиці:";
    }

    public static PageField New()
    {
        PageField view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Інші
        vBox.Append(field);
    }

    protected override void CreateEnd(Box vBox)
    {

    }

    public override async Task AssignValue()
    {
        if (IsNew)
            _ = await Function.FillNewField(ConfField, ParentTable, Fields);

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
        (bool result, string name) = IsValid(basicFields.ItemName, ConfField.Name, [.. Fields.Keys]);
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

        Caption = $"Поле: {ConfField.Name}";
        IsNew = false;

        return true;
    }
}

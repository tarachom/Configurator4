/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageTablePart))]
partial class PageTablePart : FormPageConfigurator
{
    public ConfigurationTablePart ConfTablePart { get; set; } = new();
    public Dictionary<string, ConfigurationTablePart> TabularParts = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    TablePartDataTree dataTree = TablePartDataTree.New();

    partial void Initialize()
    {
        basicFields.TableOrColumnLabel = "Таблиця:";
    }

    public static PageTablePart New()
    {
        PageTablePart view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        if (IsNew)
            _ = await Function.FillNewTablePart(ConfTablePart);

        basicFields.ItemName = ConfTablePart.Name;
        basicFields.FullName = ConfTablePart.FullName;
        basicFields.TableOrColumn = ConfTablePart.Table;
        basicFields.Desc = ConfTablePart.Desc;

        dataTree.SetValue(ConfTablePart);
    }

    protected override async Task GetValue()
    {
        ConfTablePart.Name = basicFields.ItemName;
        ConfTablePart.FullName = basicFields.FullName;
        ConfTablePart.Table = basicFields.TableOrColumn;
        ConfTablePart.Desc = basicFields.Desc;

    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfTablePart.Name, [.. TabularParts.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                TabularParts.Remove(ConfTablePart.Name);
        }
        else
            return false;

        await GetValue();

        TabularParts.Add(ConfTablePart.Name, ConfTablePart);

        Caption = $"Таблична частина: {ConfTablePart.Name}";
        IsNew = false;

        return true;
    }
}

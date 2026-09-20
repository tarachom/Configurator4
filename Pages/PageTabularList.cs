/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageTabularList))]
partial class PageTabularList : FormPageConfigurator
{
    public ConfigurationTabularList ConfTabularList { get; set; } = new();
    public Dictionary<string, ConfigurationTabularList> TabularLists = [];
    public Dictionary<string, ConfigurationField> Fields = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    DataTabularList dataTabularList = DataTabularList.New();

    partial void Initialize()
    {
        basicFields.HideTableOrColumn();
    }

    public static PageTabularList New()
    {
        PageTabularList view = NewWithProperties([]);
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
        //Налаштування
        vBox.Append(dataTabularList);
    }

    public override async Task AssignValue()
    {
        //if (IsNew)  _ = await Function.FillNewTablePart(ConfTablePart);

        basicFields.ItemName = ConfTabularList.Name;
        basicFields.FullName = ConfTabularList.FullName;
        basicFields.Desc = ConfTabularList.Desc;

        dataTabularList.SetValue(IsNew, Fields, ConfTabularList);
    }

    protected override async Task GetValue()
    {
        ConfTabularList.Name = basicFields.ItemName;
        ConfTabularList.FullName = basicFields.FullName;
        ConfTabularList.Desc = basicFields.Desc;

        dataTabularList.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfTabularList.Name, [.. TabularLists.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                TabularLists.Remove(ConfTabularList.Name);
        }
        else
            return false;

        await GetValue();

        TabularLists.Add(ConfTabularList.Name, ConfTabularList);

        Caption = $"Табличний список: {ConfTabularList.Name}";
        IsNew = false;

        return true;
    }
}

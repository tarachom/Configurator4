/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageQueryBlock))]
partial class PageQueryBlock : FormPageConfigurator
{
    public ConfigurationQueryBlock ConfQueryBlock { get; set; } = new();
    public Dictionary<string, ConfigurationQueryBlock> QueryBlocks { get; set; } = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    QueryBlockList queryList = QueryBlockList.New();

    partial void Initialize()
    {
        PageName = "Тригер:";
        basicFields.HideTableOrColumn();
    }

    public static PageQueryBlock New()
    {
        PageQueryBlock w = NewWithProperties([]);
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
        //Список запитів
        vBox.Append(queryList);
    }

    public override async Task AssignValue()
    {
        basicFields.ItemName = ConfQueryBlock.Name;
        basicFields.FullName = ConfQueryBlock.FullName;
        basicFields.Desc = ConfQueryBlock.Desc;

        queryList.SetValue(ConfQueryBlock);
    }

    protected override async Task GetValue()
    {
        ConfQueryBlock.Name = basicFields.ItemName;
        ConfQueryBlock.FullName = basicFields.FullName;
        ConfQueryBlock.Desc = basicFields.Desc;

        queryList.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfQueryBlock.Name, [.. QueryBlocks.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                QueryBlocks.Remove(ConfQueryBlock.Name);
        }
        else
            return false;

        await GetValue();
        QueryBlocks.Add(ConfQueryBlock.Name, ConfQueryBlock);

        Caption = ConfQueryBlock.Name;
        IsNew = false;

        return true;
    }
}

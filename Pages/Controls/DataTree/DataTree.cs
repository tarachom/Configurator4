using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;
using InterfaceGtk4;

/// <summary>
/// 
/// </summary>
[Subclass<Box>()]
[Template<AssemblyResource>("DataTree.ui")]
public abstract partial class DataTree
{
    protected static async Task OpenPageField(bool isNew, Dictionary<string, ConfigurationField> fields, ConfigurationField? field = null, ConfiguratorItemOwner? owner = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageField(isNew, fields, field, owner);

        /*
        PageField page = PageField.New();

        page.IsNew = isNew;
        page.ParentTable = parentTable;
        page.Fields = fields;
        if (field != null) page.ConfField = field;
        page.Caption = $"Поле: {(isNew ? "*" : field?.Name)}";

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);

        await page.SetValue();*/
    }

    protected static async Task OpenPageTablePart(bool isNew, Dictionary<string, ConfigurationTablePart> tabularParts, ConfigurationTablePart? tablePart = null, ConfiguratorItemOwner? owner = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageTablePart(isNew, tabularParts, tablePart, owner);

        /*
        PageTablePart page = PageTablePart.New();

        page.IsNew = isNew;
        page.TabularParts = tabularParts;
        if (tablePart != null) page.ConfTablePart = tablePart;
        page.Caption = $"Таблична частина: {(isNew ? "*" : tablePart?.Name)}";

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);

        await page.SetValue();*/
    }

    protected static async Task OpenPageTabularList(bool isNew, Dictionary<string, ConfigurationTabularList> tabularLists, Dictionary<string, ConfigurationField> fields, ConfigurationTabularList? tabularList = null)
    {
        ///!!!
        PageTabularList page = PageTabularList.New();

        page.IsNew = isNew;
        page.TabularLists = tabularLists;
        page.Fields = fields;
        if (tabularList != null) page.ConfTabularList = tabularList;
        page.Caption = isNew ? "*" : tabularList?.Name ?? "";

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);

        await page.SetValue();
    }
}
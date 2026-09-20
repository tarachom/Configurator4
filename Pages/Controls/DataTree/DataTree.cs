using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;

/// <summary>
/// 
/// </summary>
[Subclass<Box>()]
[Template<AssemblyResource>("DataTree.ui")]
public abstract partial class DataTree
{
    protected static async Task OpenPageField(bool isNew, string parentTable, Dictionary<string, ConfigurationField> fields, ConfigurationField? field = null)
    {
        PageField page = PageField.New();

        page.IsNew = isNew;
        page.ParentTable = parentTable;
        page.Fields = fields;
        if (field != null) page.ConfField = field;
        page.Caption = $"Поле: {(isNew ? "*" : field?.Name)}";

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);

        await page.SetValue();
    }

    protected static async Task OpenPageTablePart(bool isNew, Dictionary<string, ConfigurationTablePart> tabularParts, ConfigurationTablePart? tablePart = null)
    {
        PageTablePart page = PageTablePart.New();

        page.IsNew = isNew;
        page.TabularParts = tabularParts;
        if (tablePart != null) page.ConfTablePart = tablePart;
        page.Caption = $"Таблична частина: {(isNew ? "*" : tablePart?.Name)}";

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);

        await page.SetValue();
    }

    protected static async Task OpenPageTabularList(bool isNew, Dictionary<string, ConfigurationTabularList> tabularLists, Dictionary<string, ConfigurationField> fields, ConfigurationTabularList? tabularList = null)
    {
        PageTabularList page = PageTabularList.New();

        page.IsNew = isNew;
        page.TabularLists = tabularLists;
        page.Fields = fields;
        if (tabularList != null) page.ConfTabularList = tabularList;
        page.Caption = $"Табличний список: {(isNew ? "*" : tabularList?.Name)}";

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);

        await page.SetValue();
    }
}
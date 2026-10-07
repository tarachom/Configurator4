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
    }

    protected static async Task OpenPageTablePart(bool isNew, Dictionary<string, ConfigurationTablePart> tabularParts, ConfigurationTablePart? tablePart = null, ConfiguratorItemOwner? owner = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageTablePart(isNew, tabularParts, tablePart, owner);
    }

    protected static async Task OpenPageTabularList(bool isNew, Dictionary<string, ConfigurationTabularList> tabularLists, Dictionary<string, ConfigurationField> fields, ConfigurationTabularList? tabularList = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageTabularList(isNew, tabularLists, fields, tabularList);
    }

    protected static async Task OpenPageForm(bool isNew, Dictionary<string, ConfigurationForms> forms, ConfigurationForms? form = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageForm(isNew, forms, form);
    }
}
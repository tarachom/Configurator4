using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;

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


}
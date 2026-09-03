using Gtk;
using GObject;
using AccountingSoftware;

namespace Configurator;

[Subclass<Box>("BasicFields")]
[Template<AssemblyResource>("BasicFields.ui")]
public partial class BasicFields
{
    [Connect("entry_item_name")] Entry entryItemName;
    [Connect("entry_full_name")] Entry entryFullName;
    [Connect("label_table_or_column")] Label labelTableOrColumn;
    [Connect("entry_table_or_column")] Entry entryTableOrColumn;
    [Connect("textview_desc")] TextView textviewDesc;

    partial void Initialize()
    {
        //Втрата фокусу полем entryItemName
        {
            EventControllerFocus controller = EventControllerFocus.New();
            controller.OnLeave += (_, _) =>
            {
                if (string.IsNullOrEmpty(FullName))
                {
                    FullName = Configuration.CreateFullName(ItemName);
                    if (string.IsNullOrEmpty(Desc)) Desc = FullName;
                }
            };
            entryItemName.AddController(controller);
        }
    }

    public static BasicFields New()
    {
        BasicFields w = NewWithProperties([]);
        return w;
    }

    public string ItemName
    {
        get => entryItemName.GetText();
        set => entryItemName.SetText(value);
    }

    public string FullName
    {
        get => entryFullName.GetText();
        set => entryFullName.SetText(value);
    }

    public string TableOrColumnLabel
    {
        get => labelTableOrColumn.GetText();
        set => labelTableOrColumn.SetText(value);
    }

    public string TableOrColumn
    {
        get => entryTableOrColumn.GetText();
        set => entryTableOrColumn.SetText(value);
    }

    public string Desc
    {
        get => textviewDesc.Buffer?.Text ?? string.Empty;
        set => textviewDesc.Buffer?.Text = value;
    }
}
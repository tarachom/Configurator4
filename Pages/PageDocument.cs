/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageDocument))]
partial class PageDocument : FormPageConfigurator
{
    public override Configuration Conf { get; } = Program.Kernel.Conf;
    public ConfigurationDocuments ConfDocument { get; set; } = new();

    BasicFields basicFields = BasicFields.New();
    Triggers triggers = Triggers.New();
    DocumentAutomaticNumbering autoNum = DocumentAutomaticNumbering.New();
    DocumentDataTree dataTree = DocumentDataTree.New();

    partial void Initialize()
    {
        basicFields.TableOrColumnLabel = "Таблиця:";
    }

    public static PageDocument New()
    {
        PageDocument view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Автоматична нумерація
        vBox.Append(autoNum);

        //Тригери
        vBox.Append(triggers);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        if (IsNew)
            _ = await Function.FillNewDocument(ConfDocument);

        basicFields.ItemName = ConfDocument.Name;
        basicFields.FullName = ConfDocument.FullName;
        basicFields.TableOrColumn = ConfDocument.Table;
        basicFields.Desc = ConfDocument.Desc;

        triggers.SetValue(ConfDocument.TriggerFunctions);
        dataTree.SetValue(ConfDocument);
        autoNum.SetValue(ConfDocument);
    }

    protected override async Task GetValue()
    {
        ConfDocument.Name = basicFields.ItemName;
        ConfDocument.FullName = basicFields.FullName;
        ConfDocument.Table = basicFields.TableOrColumn;
        ConfDocument.Desc = basicFields.Desc;

        ConfDocument.TriggerFunctions = triggers.GetValue();
        autoNum.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfDocument.Name, [.. Conf.Documents.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.Documents.Remove(ConfDocument.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendDocument(ConfDocument);
        IsNew = false;

        return true;
    }
}

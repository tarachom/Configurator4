/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageDirectory))]
partial class PageDirectory : FormPageConfigurator
{
    public override Configuration Conf { get; } = Program.Kernel.Conf;
    public ConfigurationDirectories ConfDirectory { get; set; } = new();

    BasicFields basicFields = BasicFields.New();
    Triggers triggers = Triggers.New();
    DirectoryHierarchy hierarchy = DirectoryHierarchy.New();
    DirectorySubordination subordination = DirectorySubordination.New();
    DirectoryAutomaticNumbering autoNum = DirectoryAutomaticNumbering.New();
    DirectoryDataTree dataTree = DirectoryDataTree.New();

    partial void Initialize()
    {
        basicFields.TableOrColumnLabel = "Таблиця:";
    }

    public static PageDirectory New()
    {
        PageDirectory view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Ієрархія
        vBox.Append(hierarchy);

        //Підпорядкування
        vBox.Append(subordination);

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
            _ = await Function.FillNewDirectory(ConfDirectory);

        basicFields.ItemName = ConfDirectory.Name;
        basicFields.FullName = ConfDirectory.FullName;
        basicFields.TableOrColumn = ConfDirectory.Table;
        basicFields.Desc = ConfDirectory.Desc;

        triggers.SetValue(ConfDirectory.TriggerFunctions);
        hierarchy.SetValue(ConfDirectory);
        subordination.SetValue(ConfDirectory);
        dataTree.SetValue(ConfDirectory);
        autoNum.SetValue(ConfDirectory);
    }

    protected override async Task GetValue()
    {
        ConfDirectory.Name = basicFields.ItemName;
        ConfDirectory.FullName = basicFields.FullName;
        ConfDirectory.Table = basicFields.TableOrColumn;
        ConfDirectory.Desc = basicFields.Desc;

        ConfDirectory.TriggerFunctions = triggers.GetValue();
        hierarchy.GetValue();
        subordination.GetValue();
        autoNum.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfDirectory.Name, [.. Conf.Directories.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.Directories.Remove(ConfDirectory.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendDirectory(ConfDirectory);

        Caption = $"Довідник: {ConfDirectory.Name}";
        IsNew = false;

        return true;
    }
}

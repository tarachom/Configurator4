/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageRegisterAccumulation))]
partial class PageRegisterAccumulation : FormPageConfigurator
{
    public ConfigurationRegistersAccumulation ConfRegAccum { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    RegisterAccumulationDataTree dataTree = RegisterAccumulationDataTree.New();

    partial void Initialize()
    {
        PageName = "Регістр накопичення:";
        basicFields.TableOrColumnLabel = "Таблиця:";
        basicFields.RenameTableOrColumn.RenameFunc = basicFields.RenameTableOrColumn.RenameTableDef;
    }

    public static PageRegisterAccumulation New()
    {
        PageRegisterAccumulation w = NewWithProperties([]);
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
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        if (IsNew)
            _ = await Function.FillNewRegAccum(ConfRegAccum);

        basicFields.ItemName = ConfRegAccum.Name;
        basicFields.FullName = ConfRegAccum.FullName;
        basicFields.TableOrColumn = ConfRegAccum.Table;
        basicFields.Desc = ConfRegAccum.Desc;

        dataTree.SetValue(ConfRegAccum);
    }

    protected override async Task GetValue()
    {
        ConfRegAccum.Name = basicFields.ItemName;
        ConfRegAccum.FullName = basicFields.FullName;
        ConfRegAccum.Table = basicFields.TableOrColumn;
        ConfRegAccum.Desc = basicFields.Desc;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfRegAccum.Name, [.. Conf.Directories.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.RegistersAccumulation.Remove(ConfRegAccum.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendRegistersAccumulation(ConfRegAccum);

        Caption = ConfRegAccum.Name;
        IsNew = false;

        return true;
    }
}

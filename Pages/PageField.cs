/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageField))]
partial class PageField : FormPageConfigurator
{
    public override Configuration Conf { get; } = Program.Kernel.Conf;

    Field field = Field.New();

    partial void Initialize()
    {

    }

    public static PageField New()
    {
        PageField view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    protected override void CreateStart(Box vBox)
    {
        vBox.Append(field);
    }

    protected override void CreateEnd(Box vBox)
    {

    }

    public override async Task AssignValue()
    {
        if (IsNew)
        {

        }

    }

    protected override async Task GetValue()
    {

    }

    protected override async Task<bool> Save()
    {


        return true;
    }
}

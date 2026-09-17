/*

Стартова сторінка

*/

using InterfaceGtk4;
using AccountingSoftware;
using InterfaceGtkLib;

namespace Configurator;

[GObject.Subclass<Form>]
partial class PageSettings : Form
{
    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationParam? OpenConfigurationParam { get; } = Program.BasicForm?.OpenConfigurationParam;

    partial void Initialize()
    {

    }

    public static PageSettings New()
    {
        PageSettings view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    public void SetValue()
    {

    }

    public void GtValue()
    {

    }
}

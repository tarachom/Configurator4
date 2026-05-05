/*

Стартова сторінка

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<Form>]
partial class PageSaveConfiguration : Form
{
    Configuration Conf = Program.Kernel.Conf;

    partial void Initialize()
    {
        
    }

    public static PageSaveConfiguration New()
    {
        PageSaveConfiguration view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }
}

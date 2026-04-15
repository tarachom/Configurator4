/*

Стартова сторінка

*/

using Gtk;
using InterfaceGtk4;

namespace Configurator;

[GObject.Subclass<Form>]
partial class PageHome : Form
{
    partial void Initialize()
    {
        {
            ActiveUsersView view = ActiveUsersView.New(Program.Kernel, 800, 300);

            Box hBox = New(Orientation.Horizontal, 0);
            hBox.MarginBottom = 10;
            hBox.Append(view);
            Append(hBox);
        }
    }

    public static PageHome New() 
    {
        PageHome page = NewWithProperties([]);
        page.NotebookFunc = Program.BasicForm?.NotebookFunc;
        return page;
    }

    public async ValueTask SetValue()
    {

    }
}

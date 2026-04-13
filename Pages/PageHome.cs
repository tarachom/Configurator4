/*

Стартова сторінка

*/

using Gtk;
using InterfaceGtk4;

namespace Configurator;

class PageHome : Form
{
    public PageHome() : base(Program.BasicForm?.NotebookFunc)
    {
        {
            ActiveUsersView view = ActiveUsersView.New(Program.Kernel, 800, 300);

            Box hBox = New(Orientation.Horizontal, 0);
            hBox.MarginBottom = 10;
            hBox.Append(view);
            Append(hBox);
        }
    }

    public async ValueTask SetValue()
    {

    }
}

/*

Стартова сторінка

*/

using Gtk;
using InterfaceGtk4;

namespace Configurator;

class PageHome : Form
{
    public ActiveUsersView activeUsersView = new(Program.Kernel, 800, 300);

    public PageHome() : base(Program.BasicForm?.NotebookFunc)
    {
        {
            Box hBox = New(Orientation.Horizontal, 0);
            hBox.MarginBottom = 10;
            hBox.Append(activeUsersView);
            Append(hBox);
        }
    }

    public async ValueTask SetValue()
    {

    }
}

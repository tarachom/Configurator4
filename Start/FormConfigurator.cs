
using Gtk;
using AccountingSoftware;

namespace Configurator;

public class FormConfigurator : InterfaceGtk4.FormConfigurator
{
    public FormConfigurator() : base(Program.BasicApp, Program.Kernel) { }

    public FormConfigurator(Application app, Kernel kernel) : base(app, kernel)
    {
        Program.BasicForm = this;
        Program.Kernel = kernel;
    }

    public async ValueTask OpenFirstPages()
    {
        PageHome page = new();
        NotebookFunc?.CreatePage("Стартова", () => page, false, null, null, true);
        await page.SetValue();
    }

    protected override void Settings(LinkButton linkButton)
    {

    }

    protected override void Service(LinkButton linkButton)
    {

    }
}
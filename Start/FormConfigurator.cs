
using Gtk;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<InterfaceGtk4.FormConfigurator>]
public partial class FormConfigurator : InterfaceGtk4.FormConfigurator
{
    /// <summary>
    /// Викликається із конфігуратора при запуску
    /// </summary>
    public static FormConfigurator NewConfiguratorStart()
    {
        FormConfigurator window = NewWithProperties([]);
        window.Application = Program.BasicApp;
        window.Init(Program.Kernel);

        Program.BasicForm = window;

        return window;
    }

    /// <summary>
    /// Викликається із зовнішньої програми при запуску конфігуратора
    /// </summary>
    public static FormConfigurator NewProgramStart(Application app, Kernel kernel)
    {
        FormConfigurator window = NewWithProperties([]);
        window.Application = app;
        window.Init(kernel);

        Program.BasicApp = app;
        Program.BasicForm = window;
        Program.Kernel = kernel;

        return window;
    }

    public async ValueTask OpenFirstPages()
    {
        PageHome page = PageHome.New();
        NotebookFunc?.CreatePage("Стартова", () => page, false, null, null, true);

        await page.SetValue();
    }

    protected override void Settings(LinkButton linkButton)
    {

    }

    protected override void Service(LinkButton linkButton)
    {

    }

    protected override async ValueTask PageDirectory(string name, bool isNew = false)
    {
        PageDirectory page = Configurator.PageDirectory.New();
        page.IsNew = isNew;
        page.ConfName = name;
        page.Caption = $"Довідник: {(isNew ? "*" : name)}";

        NotebookFunc?.CreatePage(page.Caption, page);

        await page.SetValue();
    }
}

using Gtk;
using AccountingSoftware;

namespace Configurator;

public class FormConfigurator : InterfaceGtk4.FormConfigurator
{
    public FormConfigurator() : base(Program.BasicApp, Program.Kernel) { }

    /// <summary>
    /// Викликається із конфігуратора при запуску
    /// </summary>
    public FormConfigurator(Application app) : base(app, Program.Kernel)
    {
        Program.BasicForm = this;
    }

    /// <summary>
    /// Викликається із зовнішньої програми при запуску конфігуратора
    /// </summary>
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

    protected override async ValueTask PageDirectory(string name, bool isNew = false)
    {
        PageDirectory page = new()
        {
            IsNew = isNew,
            ConfName = name,
            Caption = $"Довідник: {(isNew ? "*" : name)}"
        };

        NotebookFunc?.CreatePage(page.Caption, page);

        await page.SetValue();
    }
}
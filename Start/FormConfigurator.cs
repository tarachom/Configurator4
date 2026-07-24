
using Gtk;
using AccountingSoftware;
using InterfaceGtkLib;

namespace Configurator;

[GObject.Subclass<InterfaceGtk4.FormConfigurator>]
public partial class FormConfigurator : InterfaceGtk4.FormConfigurator
{
    protected override Kernel Kernel { get; set; } = Program.Kernel;

    /// <summary>
    /// Викликається із конфігуратора при запуску
    /// </summary>
    public static FormConfigurator NewConfiguratorStart(ConfigurationParam? openConfigurationParam)
    {
        FormConfigurator form = NewWithProperties([]);
        form.Application = Program.BasicApp;
        form.OpenConfigurationParam = openConfigurationParam;

        form.SetValue();

        return form;
    }

    /// <summary>
    /// Викликається із зовнішньої програми при запуску конфігуратора
    /// </summary>
    public static FormConfigurator NewProgramStart(Application app, Kernel kernel, ConfigurationParam? openConfigurationParam)
    {
        FormConfigurator form = NewWithProperties([]);
        form.Application = app;
        form.Kernel = kernel;
        form.OpenConfigurationParam = openConfigurationParam;

        form.SetValue();

        Program.BasicApp = app;
        Program.BasicForm = form;
        Program.Kernel = kernel;

        return form;
    }

    public async Task OpenFirstPages()
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

    protected override async Task PageDirectory(string name, bool isNew = false)
    {
        PageDirectory page = Configurator.PageDirectory.New();
        page.IsNew = isNew;
        page.ConfName = name;
        page.Caption = $"Довідник: {(isNew ? "*" : name)}";

        NotebookFunc?.CreatePage(page.Caption, page);

        await page.SetValue();
    }

    #region TopMenu

    protected override async Task PageSaveConfiguration()
    {
        PageSaveConfiguration page = Configurator.PageSaveConfiguration.New();
        NotebookFunc?.CreatePage("Зберегти конфігурацію", page);

        page.SetValue();
    }

    #endregion
}
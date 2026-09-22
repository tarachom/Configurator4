
using Gtk;
using AccountingSoftware;
using InterfaceGtkLib;
using InterfaceGtk4;

namespace Configurator;

[GObject.Subclass<InterfaceGtk4.FormConfigurator>]
public partial class FormConfigurator : InterfaceGtk4.FormConfigurator
{
    protected override Kernel Kernel { get; set; } = Program.Kernel;

    /// <summary>
    /// Викликається із конфігуратора при запуску
    /// </summary>
    public static FormConfigurator NewConfiguratorStart(GlobalConfigurationParam globalConfigurationParam, ConfigurationParam? openConfigurationParam)
    {
        FormConfigurator form = NewWithProperties([]);
        form.Application = Program.BasicApp;
        form.OpenConfigurationParam = openConfigurationParam;
        form.GlobalConfigurationParam = globalConfigurationParam;

        form.SetValue();

        return form;
    }

    /// <summary>
    /// Викликається із зовнішньої програми при запуску конфігуратора
    /// </summary>
    public static FormConfigurator NewProgramStart(Application app, Kernel kernel, GlobalConfigurationParam globalConfigurationParam, ConfigurationParam? openConfigurationParam)
    {
        FormConfigurator form = NewWithProperties([]);
        form.Application = app;
        form.Kernel = kernel;
        form.OpenConfigurationParam = openConfigurationParam;
        form.GlobalConfigurationParam = globalConfigurationParam;

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

    protected override async Task PageDirectory(ConfiguratorItemRow? item)
    {
        PageDirectory page = Configurator.PageDirectory.New();
        if (item != null && item.Obj is ConfigurationDirectories confDirectory)
        {
            page.ConfDirectory = confDirectory;
            page.Caption += page.ConfDirectory.Name;
        }
        else
        {
            page.IsNew = true;
            page.Caption += "*";
        }

        NotebookFunc?.CreatePage(page.Caption, page);
        await page.SetValue();

        /*ConfigurationDirectories? directory = null;
        if (!isNew && !Kernel.Conf.Directories.TryGetValue(name, out directory))
        {
            Message.Error(Program.BasicForm, "Помилка", $"Не знайдено довідник '{name}' в колекції");
            return;
        }

        PageDirectory page = Configurator.PageDirectory.New();
        page.IsNew = isNew;
        page.Caption = $"Довідник: {(isNew ? "*" : name)}";

        if (!isNew && directory != null)
            page.ConfDirectory = directory;

        NotebookFunc?.CreatePage(page.Caption, page);
        await page.SetValue();*/
    }

    /*
    protected override async Task PageField(string name, bool isNew = false)
    {

    }
    */

    protected override async Task PageDocument(string name, bool isNew = false)
    {
        ConfigurationDocuments? document = null;
        if (!isNew && !Kernel.Conf.Documents.TryGetValue(name, out document))
        {
            Message.Error(Program.BasicForm, "Помилка", $"Не знайдено документ '{name}' в колекції");
            return;
        }

        PageDocument page = Configurator.PageDocument.New();
        page.IsNew = isNew;
        page.Caption = $"Документ: {(isNew ? "*" : name)}";

        if (!isNew && document != null)
            page.ConfDocument = document;

        NotebookFunc?.CreatePage(page.Caption, page);
        await page.SetValue();
    }


    protected override async Task PageSettings()
    {
        PageSettings page = Configurator.PageSettings.New();
        NotebookFunc?.CreatePage("Налаштування", page);

        page.SetValue();
    }

    #region TopMenu

    protected override async Task PageSaveConfiguration()
    {
        PageSaveConfiguration page = Configurator.PageSaveConfiguration.New();
        NotebookFunc?.CreatePage("Зберегти конфігурацію", page);

        page.SetValue();
    }

    protected override async Task PageConfigurationInfo()
    {
        PageConfigurationInfo page = Configurator.PageConfigurationInfo.New();
        NotebookFunc?.CreatePage("Параметри конфігураціЇ", page);

        page.SetValue();
    }

    #endregion

    #region Func



    #endregion
}
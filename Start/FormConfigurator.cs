
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

    public override async Task PageDirectory(bool isNew, ConfigurationDirectories? directory = null)
    {
        PageDirectory page = Configurator.PageDirectory.New();
        if (isNew)
        {
            page.IsNew = true;
            page.Caption = "*";
        }
        else if (directory != null)
        {
            page.ConfDirectory = directory;
            page.Caption = directory.Name;
        }

        NotebookFunc?.CreatePage(page.Caption, page);
        await page.SetValue();
    }

    public override async Task PageDocument(bool isNew, ConfigurationDocuments? document = null)
    {
        PageDocument page = Configurator.PageDocument.New();
        if (isNew)
        {
            page.IsNew = true;
            page.Caption = "*";
        }
        else if (document != null)
        {
            page.ConfDocument = document;
            page.Caption = document.Name;
        }

        NotebookFunc?.CreatePage(page.Caption, page);
        await page.SetValue();
    }

    public override async Task PageField(bool isNew, Dictionary<string, ConfigurationField> fields, ConfigurationField? field = null, ConfiguratorItemOwner? owner = null)
    {
        PageField page = Configurator.PageField.New();
        page.Owner = owner;
        page.Fields = fields;

        if (isNew)
        {
            page.IsNew = true;
            page.Caption = "*";
        }
        else if (field != null)
        {
            page.ConfField = field;
            page.Caption = field.Name;
        }

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);
        await page.SetValue();
    }

    public override async Task PageTablePart(bool isNew, Dictionary<string, ConfigurationTablePart> tabularParts, ConfigurationTablePart? tablePart = null, ConfiguratorItemOwner? owner = null)
    {
        PageTablePart page = Configurator.PageTablePart.New();
        page.Owner = owner;
        page.IsNew = isNew;
        page.TabularParts = tabularParts;

        if (isNew)
        {
            page.IsNew = true;
            page.Caption = "*";
        }
        else if (tablePart != null)
        {
            page.ConfTablePart = tablePart;
            page.Caption = tablePart.Name;
        }

        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);
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
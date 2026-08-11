/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageDirectory))]
partial class PageDirectory : FormPageConfigurator
{
    public override Configuration Conf { get; } = Program.Kernel.Conf;
    public ConfigurationDirectories ConfDirectory { get; set; } = new();

    Entry entryName = Entry.New();
    Entry entryFullName = Entry.New();
    Entry entryTable = Entry.New();
    TextView textViewDesc = TextView.New();
    Triggers triggers = Triggers.New();
    DirectoryHierarchy hierarchy = DirectoryHierarchy.New();

    partial void Initialize()
    {
        entryName.WidthRequest = 500;
        entryFullName.WidthRequest = 500;
        entryTable.WidthRequest = 500;

        textViewDesc.WrapMode = WrapMode.Word;


    }

    public static PageDirectory New()
    {
        PageDirectory view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    protected override void CreateStart(Box vBox)
    {
        // Назва
        CreateField(vBox, "Назва", entryName);

        // Повна назва
        CreateField(vBox, "Повна назва", entryFullName);

        // Таблиця
        CreateField(vBox, "Таблиця", entryTable);

        // Опис
        CreateFieldView(vBox, "Опис", textViewDesc, 500, 100);

        //Ієрархія
        vBox.Append(hierarchy);

        //Тригери
        vBox.Append(triggers);
    }

    protected override void CreateEnd(Box vBox)
    {

    }

    public override async Task AssignValue()
    {
        if (IsNew)
        {
            ConfDirectory.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

            //Заповнення полями
            {
                //Код
                {
                    string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                    ConfDirectory.AppendField(new ConfigurationField("Код", "Код", nameInTable, "string", "", "Код", false, true, false, true));
                }

                //Назва
                {
                    string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                    ConfDirectory.AppendField(new ConfigurationField("Назва", "Назва", nameInTable, "string", "", "Назва", true, true, false, true));
                }
            }

            //Табличний список
            {
                ConfigurationTabularList list = new("Записи");
                int sortNum = 0;

                //Заповнення полями
                foreach (var item in ConfDirectory.Fields.Values)
                    list.AppendField(new(item.Name, item.Name, 0, ++sortNum, item.Name == "Назва"));

                //Заповнення списку
                ConfDirectory.AppendTableList(list);
            }

            //Тригери
            ConfDirectory.TriggerFunctions.NewAction = true;
            ConfDirectory.TriggerFunctions.CopyingAction = true;
        }

        entryName.SetText(ConfDirectory.Name);
        entryFullName.SetText(ConfDirectory.FullName);
        entryTable.SetText(ConfDirectory.Table);
        textViewDesc.Buffer?.Text = ConfDirectory.Desc;

        triggers.SetValue(ConfDirectory.TriggerFunctions);
        hierarchy.SetValue(ConfDirectory);
    }

    protected override async Task GetValue()
    {
        ConfDirectory.Name = entryName.GetText();
        ConfDirectory.FullName = entryFullName.GetText();
        ConfDirectory.Table = entryTable.GetText();
        ConfDirectory.Desc = textViewDesc.Buffer?.Text ?? "";

        ConfDirectory.TriggerFunctions = triggers.GetValue();
        hierarchy.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(entryName.GetText(), ConfDirectory.Name, [.. Conf.Directories.Keys]);
        entryName.SetText(name);

        if (result)
        {
            if (!IsNew)
                Conf.Directories.Remove(ConfDirectory.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendDirectory(ConfDirectory);
        IsNew = false;

        return true;
    }
}

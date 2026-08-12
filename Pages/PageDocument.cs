/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

[GObject.Subclass<FormPageConfigurator>(nameof(PageDocument))]
partial class PageDocument : FormPageConfigurator
{
    public override Configuration Conf { get; } = Program.Kernel.Conf;
    public ConfigurationDocuments ConfDocument { get; set; } = new();

    Entry entryName = Entry.New();
    Entry entryFullName = Entry.New();
    Entry entryTable = Entry.New();
    TextView textViewDesc = TextView.New();
    Triggers triggers = Triggers.New();
    DocumentAutomaticNumbering autoNum = DocumentAutomaticNumbering.New();
    DocumentDataTree dataTree = DocumentDataTree.New();

    partial void Initialize()
    {
        entryName.WidthRequest = 500;
        entryFullName.WidthRequest = 500;
        entryTable.WidthRequest = 500;

        textViewDesc.WrapMode = WrapMode.Word;
    }

    public static PageDocument New()
    {
        PageDocument view = NewWithProperties([]);
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

        //Автоматична нумерація
        vBox.Append(autoNum);

        //Тригери
        vBox.Append(triggers);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        if (IsNew)
        {
            ConfDocument.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

            //Заповнення полями
            {
                //Заповнення полями
                ConfDocument.AppendField(new ConfigurationField("Назва", "Назва", "docname", "string", "", "Назва", true, true));
                ConfDocument.AppendField(new ConfigurationField("НомерДок", "Номер", "docnomer", "string", "", "Номер документу", false, true));
                ConfDocument.AppendField(new ConfigurationField("ДатаДок", "Дата", "docdate", "datetime", "", "Дата документу", false, true));

                //Код
                {
                    string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.GetText(), ConfDocument.Fields);
                    ConfDocument.AppendField(new ConfigurationField("Коментар", "Коментар", nameInTable, "string", "", "Коментар"));
                }

                //Назва
                {
                    string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.GetText(), ConfDocument.Fields);
                    ConfDocument.AppendField(new ConfigurationField("Підстава", "Підстава", nameInTable, "composite_pointer", "", "Підстава"));
                }
            }

            //Табличний список
            {
                ConfigurationTabularList list = new("Записи");
                int sortNum = 0;
                string[] typesIgnor = ["composite_pointer"];

                //Заповнення полями списків (крім типів які ігноруються)
                foreach (var item in ConfDocument.Fields.Values.Where(x => typesIgnor.Contains(x.Type)))
                {
                    string caption = item.Name switch { "ДатаДок" => "Дата", "НомерДок" => "Номер", _ => item.Name };
                    list.AppendField(new ConfigurationTabularListField(item.Name, caption, 0, ++sortNum, item.Name == "ДатаДок"));
                }

                //Заповнення списку
                ConfDocument.AppendTableList(list);
            }

            //Форми
            {
                {
                    string name = "Функції";
                    ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.Function);
                    ConfDocument.AppendForms(forms);
                }

                {
                    string name = "Тригери";
                    ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.Triggers);
                    ConfDocument.AppendForms(forms);
                }

                {
                    string name = "Реквізит вибору";
                    ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.PointerControl);
                    ConfDocument.AppendForms(forms);
                }

                {
                    string name = "Реквізит вибору для таб частини";
                    ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.PointerTablePartCell);
                    ConfDocument.AppendForms(forms);
                }

                {
                    string name = "Швидкий вибір";
                    ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.ListSmallSelect);
                    ConfDocument.AppendForms(forms);
                }

                {
                    string name = "Список";
                    ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.List);
                    ConfDocument.AppendForms(forms);
                }
            }

            //Тригери
            ConfDocument.TriggerFunctions.NewAction = true;
            ConfDocument.TriggerFunctions.CopyingAction = true;
        }

        entryName.SetText(ConfDocument.Name);
        entryFullName.SetText(ConfDocument.FullName);
        entryTable.SetText(ConfDocument.Table);
        textViewDesc.Buffer?.Text = ConfDocument.Desc;

        triggers.SetValue(ConfDocument.TriggerFunctions);
        dataTree.SetValue(ConfDocument);
        autoNum.SetValue(ConfDocument);
    }

    protected override async Task GetValue()
    {
        ConfDocument.Name = entryName.GetText();
        ConfDocument.FullName = entryFullName.GetText();
        ConfDocument.Table = entryTable.GetText();
        ConfDocument.Desc = textViewDesc.Buffer?.Text ?? "";

        ConfDocument.TriggerFunctions = triggers.GetValue();
        autoNum.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(entryName.GetText(), ConfDocument.Name, [.. Conf.Documents.Keys]);
        entryName.SetText(name);

        if (result)
        {
            if (!IsNew)
                Conf.Documents.Remove(ConfDocument.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendDocument(ConfDocument);
        IsNew = false;

        return true;
    }
}

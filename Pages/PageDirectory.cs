/*

Стартова сторінка

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

class PageDirectory : FormPageConfigurator
{
    Configuration Conf = Program.Kernel.Conf;
    ConfigurationDirectories ConfDirectory = new();

    Entry entryName = new Entry() { WidthRequest = 500 };
    Entry entryFullName = new Entry() { WidthRequest = 500 };
    Entry entryTable = new Entry() { WidthRequest = 500 };

    public PageDirectory() : base(Program.BasicForm?.NotebookFunc)
    {

    }

    protected override void CreateStart(Box vBox)
    {
        // Назва
        CreateField(vBox, "Назва", entryName);

        // Повна назва
        CreateField(vBox, "Повна назва", entryFullName);

        // Таблиця
        CreateField(vBox, "Таблиця", entryTable);
    }

    protected override void CreateEnd(Box vBox)
    {

    }

    public override async ValueTask AssignValue()
    {
        if (IsNew)
        {
            entryTable.SetText(await Configuration.GetNewUnigueTableName(Program.Kernel));

            //Заповнення полями
            string nameInTable_Code = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.GetText(), ConfDirectory.Fields);
            ConfDirectory.AppendField(new ConfigurationField("Код", "Код", nameInTable_Code, "string", "", "Код", false, true, false, true));

            string nameInTable_Name = Configuration.GetNewUnigueColumnName(Program.Kernel, entryTable.GetText(), ConfDirectory.Fields);
            ConfDirectory.AppendField(new ConfigurationField("Назва", "Назва", nameInTable_Name, "string", "", "Назва", true, true, false, true));

            //Заповнення списку
            ConfDirectory.AppendTableList(new ConfigurationTabularList("Записи"));

            int sortNum = 0;

            //Заповнення полями
            foreach (var item in ConfDirectory.Fields.Values)
                ConfDirectory.TabularList["Записи"].AppendField(
                    new ConfigurationTabularListField(item.Name, item.Name, 0, ++sortNum, item.Name == "Назва"));

            //Тригери
            ConfDirectory.TriggerFunctions.NewAction = true;
            ConfDirectory.TriggerFunctions.CopyingAction = true;
        }
        else
        {
            if (Conf.Directories.TryGetValue(ConfName, out var directory))
                ConfDirectory = directory;
            else
            {
                Message.Error(BasicApp, BasicForm, "Помилка", $"Не знайдено довідник {ConfName} в колекції");
                return;
            }
        }

        entryName.SetText(ConfDirectory.Name);
        entryFullName.SetText(ConfDirectory.FullName);
        entryTable.SetText(ConfDirectory.Table);
    }

    protected override async ValueTask GetValue()
    {
        ConfDirectory.Name = entryName.GetText();
        ConfDirectory.FullName = entryFullName.GetText();
        ConfDirectory.Table = entryTable.GetText();
    }

    protected override async ValueTask<bool> Save()
    {
        Console.WriteLine("Save");
        return await ValueTask.FromResult(true); ;
    }
}

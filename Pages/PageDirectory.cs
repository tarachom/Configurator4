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
    public ConfigurationDirectories ConfDirectory { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    Triggers triggers = Triggers.New();
    DirectoryHierarchy hierarchy = DirectoryHierarchy.New();
    DirectorySubordination subordination = DirectorySubordination.New();
    DirectoryAutomaticNumbering autoNum = DirectoryAutomaticNumbering.New();
    DirectoryDataTree dataTree = DirectoryDataTree.New();

    partial void Initialize()
    {
        PageName = "Довідник:";
        basicFields.TableOrColumnLabel = "Таблиця:";
        basicFields.RenameTableOrColumn.RenameFunc = async () =>
        {
            List<string> log = [];

            string oldName = basicFields.TableOrColumn;
            string newName = basicFields.RenameTableOrColumn.NewName;

            ConfigurationInformationSchema schema = await Program.Kernel.DataBase.SelectInformationSchema();
            if (string.IsNullOrEmpty(newName))
            {
                log.Add("Не вказана нова назва таблиці!");
                return (false, log);
            }

            if (schema.Tables.ContainsKey(newName))
            {
                log.Add($"В базі вже є таблиця із назвою {newName}!");
                return (false, log);
            }

            if (schema.Tables.TryGetValue(oldName, out ConfigurationInformationSchema_Table? tableOldInfo) && tableOldInfo != null)
            {
                byte transactionID = await Program.Kernel.DataBase.BeginTransaction();

                //Видалення зовнішніх ключів поточної таблиці
                foreach (var constraint in tableOldInfo.Constraints.Keys)
                {
                    string query = $"ALTER TABLE {oldName} DROP CONSTRAINT {constraint}";

                    log.Add(query);
                    await Program.Kernel.DataBase.ExecuteSQL(query, transactionID);
                }

                string pkeyOldName = $"{oldName}_pkey";
                string pkeyNewName = $"{newName}_pkey";

                //Видалення індексів поточної таблиці
                foreach (var index in tableOldInfo.Indexes.Keys)
                {
                    string query = (index == pkeyOldName) switch
                    {
                        true => $"ALTER INDEX {pkeyOldName} RENAME TO {pkeyNewName}",
                        false => $"DROP INDEX IF EXISTS {index}"
                    };

                    log.Add(query);
                    await Program.Kernel.DataBase.ExecuteSQL(query, transactionID);
                }

                //Перейменування
                {
                    string query = $"ALTER TABLE {oldName} RENAME TO {newName}";

                    log.Add(query);
                    await Program.Kernel.DataBase.ExecuteSQL(query, transactionID);
                }

                //
                /*
                foreach (ConfigurationInformationSchema_Table tableInfo in schema.Tables.Values) //.Where(x => x.Constraints.Values.Any(y => y.ToTable == oldName))
                {
                    foreach (var constraint in tableInfo.Constraints.Values)
                    {
                        if (constraint.ToTable == oldName)
                        {
                            
                        }
                    }
                }
                */

                await Program.Kernel.DataBase.CommitTransaction(transactionID);
            }

            return (true, log);
        };
    }

    public static PageDirectory New()
    {
        PageDirectory w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Ієрархія
        vBox.Append(hierarchy);

        //Підпорядкування
        vBox.Append(subordination);

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
            _ = await Function.FillNewDirectory(ConfDirectory);
            basicFields.NewTableOrColumn();
        }

        basicFields.ItemName = ConfDirectory.Name;
        basicFields.FullName = ConfDirectory.FullName;
        basicFields.TableOrColumn = ConfDirectory.Table;
        basicFields.Desc = ConfDirectory.Desc;

        triggers.SetValue(ConfDirectory.TriggerFunctions);
        hierarchy.SetValue(ConfDirectory);
        subordination.SetValue(ConfDirectory);
        dataTree.SetValue(ConfDirectory);
        autoNum.SetValue(ConfDirectory);
    }

    protected override async Task GetValue()
    {
        ConfDirectory.Name = basicFields.ItemName;
        ConfDirectory.FullName = basicFields.FullName;
        ConfDirectory.Table = basicFields.TableOrColumn;
        ConfDirectory.Desc = basicFields.Desc;

        ConfDirectory.TriggerFunctions = triggers.GetValue();
        hierarchy.GetValue();
        subordination.GetValue();
        autoNum.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfDirectory.Name, [.. Conf.Directories.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.Directories.Remove(ConfDirectory.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendDirectory(ConfDirectory);

        Caption = ConfDirectory.Name;
        IsNew = false;

        return true;
    }
}

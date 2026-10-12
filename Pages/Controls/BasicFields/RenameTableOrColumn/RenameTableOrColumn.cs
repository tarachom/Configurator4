using Gtk;
using GObject;
using AccountingSoftware;

namespace Configurator;

[Subclass<Box>("RenameTableOrColumn")]
[Template<AssemblyResource>("RenameTableOrColumn.ui")]
public partial class RenameTableOrColumn
{
    [Connect("popover_rename")] Popover popoverRename;
    [Connect("entry_old_name")] Entry entryOldName;
    [Connect("entry_new_name")] Entry entryNewName;
    [Connect("button_rename")] Button buttonRename;
    [Connect("textview_log")] TextView textviewLog;

    partial void Initialize()
    {
        buttonRename.OnClicked += async (_, _) =>
        {
            if (RenameFunc != null)
            {
                (bool result, List<string> list) = await RenameFunc.Invoke();
                if (result)
                {
                    OldName = NewName;
                    CallBack_Update?.Invoke(NewName);
                }

                //Вивід логу
                foreach (var text in list)
                    ApendLog(text);
            }
        };

        //При відкритті popover
        popoverRename.OnShow += (_, _) => entryNewName.GrabFocus();
    }

    /// <summary>
    /// Функція перейменування
    /// </summary>
    public Func<Task<(bool, List<string>)>>? RenameFunc { get; set; } = null;

    public Action<string>? CallBack_Update { get; set; } = null;

    /// <summary>
    /// Стара назва
    /// </summary>
    public string OldName
    {
        get => entryOldName.GetText();
        set => entryOldName.SetText(value);
    }

    public string NewName
    {
        get => entryNewName.GetText();
        set => entryNewName.SetText(value);
    }

    void ApendLog(string message)
    {
        if (textviewLog != null && textviewLog.Buffer != null)
        {
            var buffer = textviewLog.Buffer;
            buffer.GetEndIter(out TextIter iterEndText);

            string text = message + "\n";
            buffer.Insert(iterEndText, text, -1);

            GLib.Functions.IdleAdd(GLib.Constants.PRIORITY_DEFAULT_IDLE, () =>
            {
                buffer.GetEndIter(out TextIter iterEndText);
                textviewLog.ScrollToIter(iterEndText, 0.0, false, 0.0, 0.0);
                return false;
            });
        }
    }

    /// <summary>
    /// Функція стандартна для обробки перейменування таблиці
    /// </summary>
    public async Task<(bool, List<string>)> RenameTableDef()
    {
        List<string> log = [];

        string oldName = OldName;
        string newName = NewName;

        if (string.IsNullOrEmpty(newName))
        {
            log.Add("Не вказана нова назва таблиці!");
            return (false, log);
        }

        if (oldName == newName)
        {
            log.Add("Стара назва і нова назва таблиці однакові!");
            return (false, log);
        }

        ConfigurationInformationSchema schema = await Program.Kernel.DataBase.SelectInformationSchema();
        if (schema.Tables.ContainsKey(newName))
            log.Add($"Увага! В базі вже є таблиця із назвою '{newName}'!");

        log.Add($"Остаточне перейменування '{oldName}' на '{newName}' відбудеться при збереженні конфігурації");
        return (true, log);
    }

    public async Task<(bool, List<string>)> RenameFieldDef(string parentTable)
    {
        List<string> log = [];

        string oldName = OldName;
        string newName = NewName;

        if (string.IsNullOrEmpty(newName))
        {
            log.Add("Не вказана нова назва поля!");
            return (false, log);
        }

        if (oldName == newName)
        {
            log.Add("Стара назва і нова назва поля однакові!");
            return (false, log);
        }

        ConfigurationInformationSchema schema = await Program.Kernel.DataBase.SelectInformationSchema();
        if (schema.Tables.TryGetValue(parentTable, out var table) && table.Columns.ContainsKey(newName))
            log.Add($"Увага! В таблиці {parentTable} вже є поле із назвою '{newName}'!");

        log.Add($"Остаточне перейменування '{oldName}' на '{newName}' відбудеться при збереженні конфігурації");
        return (true, log);
    }
}
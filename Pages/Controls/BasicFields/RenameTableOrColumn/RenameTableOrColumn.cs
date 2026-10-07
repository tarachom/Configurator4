using Gtk;
using GObject;

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
        popoverRename.OnShow += (sender, args) => entryNewName.GrabFocus();
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
}
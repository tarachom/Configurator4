using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

namespace Configurator;

[Subclass<Box>("AllowDocumentSpend")]
[Template<AssemblyResource>("AllowDocumentSpend.ui")]
public partial class AllowDocumentSpend
{
    [Connect("listbox")] ListBox listbox;

    Configuration Conf { get; } = Program.Kernel.Conf;

    partial void Initialize()
    {
        GestureClick gesture = GestureClick.New();
        gesture.OnPressed += async (_, args) =>
        {
            if (args.NPress >= 2)
            {
                ListBoxRow? selectedRow = listbox.GetSelectedRow();
                if (Program.BasicForm != null && selectedRow != null && Conf.Documents.TryGetValue(selectedRow.GetName(), out var document))
                    await Program.BasicForm.PageDocument(false, document);
            }
        };

        listbox.AddController(gesture);
    }

    public static AllowDocumentSpend New() => NewWithProperties([]);

    public void SetValue(List<string> allowDocumentSpend)
    {
        //Очистка
        FunctionForListBox.RemoveAll(listbox);

        //Заповнення (перевірити чи є такий документ в колекції документів)
        foreach (string document in allowDocumentSpend.Where(Conf.Documents.ContainsKey))
        {
            Label label = Label.New(document);
            label.Halign = Align.Start;

            ListBoxRow row = ListBoxRow.New();
            row.Name = document;
            row.Child = label;
            listbox.Append(row);
        }
    }
}
using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;
using InterfaceGtk4;

[Subclass<Box>("CheckListRegAccum")]
[Template<AssemblyResource>("CheckListRegAccum.ui")]
public partial class CheckListRegAccum
{
    [Connect("listbox")] ListBox listbox;

    Configuration Conf { get; } = Program.Kernel.Conf;

    public static CheckListRegAccum New() => NewWithProperties([]);

    public void SetValue(List<string> allowRegisterAccumulation)
    {
        //Очистка
        FunctionForListBox.RemoveAll(listbox);

        //Заповнення
        foreach (ConfigurationRegistersAccumulation regAccum in Conf.RegistersAccumulation.Values)
        {
            CheckButton cb = CheckButton.NewWithLabel(regAccum.Name);
            cb.Active = allowRegisterAccumulation.Contains(regAccum.Name);
            listbox.Append(cb);
        }
    }

    public List<string> GetValue()
    {
        List<string> list = [];
        
        var row = listbox.GetFirstChild();
        while (row != null)
        {
            if (row.GetFirstChild() is CheckButton cb && cb.Active && cb.Label != null) list.Add(cb.Label);
            row = row.GetNextSibling();
        }

        return list;
    }
}
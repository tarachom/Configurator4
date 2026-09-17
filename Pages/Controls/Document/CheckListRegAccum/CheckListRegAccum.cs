using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;

[Subclass<Box>("CheckListRegAccum")]
[Template<AssemblyResource>("CheckListRegAccum.ui")]
public partial class CheckListRegAccum
{
    [Connect("listbox")] ListBox listbox;

    Configuration Conf { get; } = Program.Kernel.Conf;

    public static CheckListRegAccum New()
    {
        CheckListRegAccum w = NewWithProperties([]);
        return w;
    }

    public void SetValue(List<string> allowRegisterAccumulation)
    {
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
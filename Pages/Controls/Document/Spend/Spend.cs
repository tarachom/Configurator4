using Gtk;
using GObject;
using AccountingSoftware;

[Subclass<Box>("Spend")]
[Template<AssemblyResource>("Spend.ui")]
public partial class Spend
{
    [Connect("entry_spend")] Entry entrySpend;
    [Connect("entry_clear")] Entry entryClear;

    [Connect("switch_spend")] Switch switchSpend;
    [Connect("switch_clear")] Switch switchClear;


    public static Spend New()
    {
        Spend w = NewWithProperties([]);
        return w;
    }

    public void SetValue(ConfigurationSpendFunctions spendFunc)
    {
        entrySpend.SetText(spendFunc.Spend);
        entryClear.SetText(spendFunc.ClearSpend);
        
        switchSpend.Active = spendFunc.SpendAction;
        switchClear.Active = spendFunc.ClearSpendAction;
    }

    public ConfigurationSpendFunctions GetValue() => new()
    {
        Spend = entrySpend.GetText(),
        ClearSpend = entryClear.GetText(),

        SpendAction = switchSpend.Active,
        ClearSpendAction = switchClear.Active
    };
}
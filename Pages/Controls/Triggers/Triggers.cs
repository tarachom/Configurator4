using Gtk;
using GObject;
using AccountingSoftware;

[Subclass<Box>("Triggers")]
[Template<AssemblyResource>("Triggers.ui")]
public partial class Triggers
{
    [Connect("entry_new")] Entry entryNew;
    [Connect("entry_copying")] Entry entryCopying;
    [Connect("entry_before_save")] Entry entryBeforeSave;
    [Connect("entry_after_save")] Entry entryAfterSave;
    [Connect("entry_set_deletion_label")] Entry entrySetDeletionLabel;
    [Connect("entry_before_delete")] Entry entryBeforeDelete;

    [Connect("switch_new")] Switch switchNew;
    [Connect("switch_copying")] Switch switchCopying;
    [Connect("switch_before_save")] Switch switchBeforeSave;
    [Connect("switch_after_save")] Switch switchAfterSave;
    [Connect("switch_set_deletion_label")] Switch switchSetDeletionLabel;
    [Connect("switch_before_delete")] Switch switchBeforeDelete;

    public static Triggers New()
    {
        Triggers tr = NewWithProperties([]);
        return tr;
    }

    public void SetValue(ConfigurationTriggerFunctions triggerFunc)
    {
        entryNew.SetText(triggerFunc.New);
        entryCopying.SetText(triggerFunc.Copying);
        entryBeforeSave.SetText(triggerFunc.BeforeSave);
        entryAfterSave.SetText(triggerFunc.AfterSave);
        entrySetDeletionLabel.SetText(triggerFunc.SetDeletionLabel);
        entryBeforeDelete.SetText(triggerFunc.BeforeDelete);

        switchNew.Active = triggerFunc.NewAction;
        switchCopying.Active = triggerFunc.CopyingAction;
        switchBeforeSave.Active = triggerFunc.BeforeSaveAction;
        switchAfterSave.Active = triggerFunc.AfterSaveAction;
        switchSetDeletionLabel.Active = triggerFunc.SetDeletionLabelAction;
        switchBeforeDelete.Active = triggerFunc.BeforeDeleteAction;
    }

    public ConfigurationTriggerFunctions GetValue() => new()
    {
        New = entryNew.GetText(),
        Copying = entryCopying.GetText(),
        BeforeSave = entryBeforeSave.GetText(),
        AfterSave = entryAfterSave.GetText(),
        SetDeletionLabel = entrySetDeletionLabel.GetText(),
        BeforeDelete = entryBeforeDelete.GetText(),

        NewAction = switchNew.Active,
        CopyingAction = switchCopying.Active,
        BeforeSaveAction = switchBeforeSave.Active,
        AfterSaveAction = switchAfterSave.Active,
        SetDeletionLabelAction = switchSetDeletionLabel.Active,
        BeforeDeleteAction = switchBeforeDelete.Active
    };
}
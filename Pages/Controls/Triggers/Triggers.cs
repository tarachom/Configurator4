/*
Copyright (C) 2019-2026 TARAKHOMYN YURIY IVANOVYCH
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

/*
Автор:    Тарахомин Юрій Іванович
Адреса:   Україна, м. Львів
Сайт:     accounting.org.ua
*/

using Gtk;
using GObject;
using AccountingSoftware;

namespace Configurator;

/// <summary>
/// Тригери для довідників та документів
/// </summary>
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

    public static Triggers New() => NewWithProperties([]);

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
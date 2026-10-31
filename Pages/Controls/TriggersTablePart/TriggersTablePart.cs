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
/// Тригери для табличних частин
/// </summary>
[Subclass<Box>("TriggersTablePart")]
[Template<AssemblyResource>("TriggersTablePart.ui")]
public partial class TriggersTablePart
{
    [Connect("entry_before_save")] Entry entryBeforeSave;
    [Connect("entry_after_save")] Entry entryAfterSave;

    [Connect("switch_before_save")] Switch switchBeforeSave;
    [Connect("switch_after_save")] Switch switchAfterSave;

    public static TriggersTablePart New() => NewWithProperties([]);

    public void SetValue(ConfigurationTriggerTablePartFunctions triggerFunc)
    {
        entryBeforeSave.SetText(triggerFunc.BeforeSave);
        entryAfterSave.SetText(triggerFunc.AfterSave);

        switchBeforeSave.Active = triggerFunc.BeforeSaveAction;
        switchAfterSave.Active = triggerFunc.AfterSaveAction;
    }

    public ConfigurationTriggerTablePartFunctions GetValue() => new()
    {
        BeforeSave = entryBeforeSave.GetText(),
        AfterSave = entryAfterSave.GetText(),

        BeforeSaveAction = switchBeforeSave.Active,
        AfterSaveAction = switchAfterSave.Active,
    };
}
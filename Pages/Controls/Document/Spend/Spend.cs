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
/// Функції проведення і очистки проведення
/// </summary>
[Subclass<Box>("Spend")]
[Template<AssemblyResource>("Spend.ui")]
public partial class Spend
{
    [Connect("entry_spend")] Entry entrySpend;
    [Connect("entry_clear")] Entry entryClear;

    [Connect("switch_spend")] Switch switchSpend;
    [Connect("switch_clear")] Switch switchClear;

    public static Spend New() => NewWithProperties([]);

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
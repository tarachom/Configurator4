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
using InterfaceGtk4;

namespace Configurator;

/// <summary>
/// Список регістрів накопичення із вибором
/// </summary>
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
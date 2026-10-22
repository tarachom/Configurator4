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
/// Вивід списку документів які використовують регістр накопичення
/// </summary>
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
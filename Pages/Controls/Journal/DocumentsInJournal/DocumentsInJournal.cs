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
/// Вивід списку документів які входять в журнал
/// </summary>
[Subclass<Box>("DocumentsInJournal")]
[Template<AssemblyResource>("DocumentsInJournal.ui")]
public partial class DocumentsInJournal
{
    [Connect("listbox")] ListBox listbox;
    [Connect("button_fill")] Button buttonFill;

    ConfigurationJournals ConfJournals { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    public static DocumentsInJournal New() => NewWithProperties([]);

    partial void Initialize()
    {
        buttonFill.OnClicked += (_, _) =>
        {
            foreach (string doc in GetValue())
                if (!ConfJournals.FieldMapping.Exists(x => x.Name == doc))
                    ConfJournals.FieldMapping.Add(new(doc));
        };
    }

    public void SetValue(ConfigurationJournals confJournals, List<string> allowDocument)
    {
        ConfJournals = confJournals;

        //Очистка
        FunctionForListBox.RemoveAll(listbox);

        //Заповнення
        foreach (ConfigurationDocuments document in Conf.Documents.Values)
        {
            CheckButton cb = CheckButton.NewWithLabel(document.Name);
            cb.Active = allowDocument.Contains(document.Name);
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
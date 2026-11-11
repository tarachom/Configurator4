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
/// Дерево даних для 
/// </summary>
[Subclass<DataTree>()]
public partial class JournalDataTree : DataTree
{
    public static JournalDataTree New() => NewWithProperties([]);

    public void SetValue(ConfigurationJournals journals)
    {
        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "Field" when row.Obj is ConfigurationJournalField field:
                    {
                        await OpenPageJournalField(false, journals.Fields, field);
                        break;
                    }
                case "FieldMapping" when row.Obj is ConfigurationJournalFieldMapping fieldMapping:
                    {
                        await OpenPageJournalFieldMapping(fieldMapping, journals.Fields);
                        break;
                    }
                default:
                    break;
            }
        }

        async void Add(Button button, ConfiguratorItemRow? row)
        {
            switch (row?.Group)
            {
                case "FieldGroup" or "Field":
                    {
                        ConfigurationJournalField newField = new();
                        await OpenPageJournalField(true, journals.Fields, newField);
                        break;
                    }
                default:
                    break;
            }
        }

        async void Copy(ConfiguratorItemRow row)
        {
            switch (row?.Group)
            {
                case "Field" when row.Obj is ConfigurationJournalField field:
                    {
                        ConfigurationJournalField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageJournalField(true, journals.Fields, newField);
                        break;
                    }
                default:
                    break;
            }
        }

        async void Delete(ConfiguratorItemRow row)
        {
            switch (row?.Group)
            {
                case "Field" when row.Obj is ConfigurationJournalField field:
                    {
                        journals.Fields.Remove(field.Name);
                        break;
                    }
                case "FieldMapping" when row.Obj is ConfigurationJournalFieldMapping fieldMapping:
                    {
                        journals.FieldMapping.Remove(fieldMapping);
                        break;
                    }
                default:
                    break;
            }
        }

        Box box = new ConfiguratorJournalsFieldsTree(journals, Activate, new()
        {
            Add = (button, row) => Add(button, row),
            Edit = (_, rows) =>
            {
                foreach (var row in rows)
                    Activate(row);
            },
            Copy = (_, rows) =>
            {
                foreach (var row in rows)
                    Copy(row);
            },
            Delete = async (_, rows) =>
            {
                if (await Message.Request(Program.BasicForm, "Питання", $"Видалити?") == Message.YesNo.Yes)
                {
                    foreach (var row in rows)
                        Delete(row);
                }
            },
        }).Fill();

        Append(box);
    }
}
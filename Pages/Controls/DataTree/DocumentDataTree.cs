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
/// Дерево даних для документу
/// </summary>
[Subclass<DataTree>()]
public partial class DocumentDataTree : DataTree
{
    public static DocumentDataTree New() => NewWithProperties([]);

    public void SetValue(ConfigurationDocuments document)
    {
        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "Field" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, document.Fields, field, new(ConfiguratorItemOwnerType.Document, document));
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageTablePart(false, document.TabularParts, tablePart);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        await OpenPageTabularList(false, document.TabularList, document.Fields, tabularList);
                        break;
                    }
                case "TablePartField" when row.Obj is ConfigurationField field && row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageField(false, tablePart.Fields, field, new(ConfiguratorItemOwnerType.TablePart, tablePart));
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        await OpenPageForm(false, document.Forms, form);
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
                        await OpenPageField(true, document.Fields, null, new(ConfiguratorItemOwnerType.Document, document));
                        break;
                    }
                case "TablePartGroup" or "TablePart":
                    {
                        await OpenPageTablePart(true, document.TabularParts);
                        break;
                    }
                case "TabularListGroup" or "TabularList":
                    {
                        await OpenPageTabularList(true, document.TabularList, document.Fields);
                        break;
                    }
                case "FormGroup" or "Form":
                    {
                        await OpenPageForm(true, document.Forms, null);
                        break;
                    }
                case "TablePartField" when row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageField(true, tablePart.Fields, null, new(ConfiguratorItemOwnerType.TablePart, tablePart));
                        break;
                    }
                default:
                    {
                        Popover popover = Popover.New();
                        popover.SetParent(button);

                        popover.Show();
                    }
                    break;
            }
        }

        async void Copy(ConfiguratorItemRow row)
        {
            switch (row?.Group)
            {
                case "Field" when row.Obj is ConfigurationField field:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageField(true, document.Fields, newField, new(ConfiguratorItemOwnerType.Document, document));
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += Configuration.GetNewRandomName();
                        await OpenPageTablePart(true, document.TabularParts, newTablePart);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        ConfigurationTabularList newTabularList = tabularList.Copy();
                        newTabularList.Name += Configuration.GetNewRandomName();
                        await OpenPageTabularList(true, document.TabularList, document.Fields, newTabularList);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        
                        break;
                    }
                case "TablePartField" when row.Obj is ConfigurationField field && row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageField(true, tablePart.Fields, newField, new(ConfiguratorItemOwnerType.TablePart, tablePart));
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
                case "Field" when row.Obj is ConfigurationField field:
                    {
                        document.Fields.Remove(field.Name);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        document.TabularParts.Remove(tablePart.Name);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        document.TabularList.Remove(tabularList.Name);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        document.Forms.Remove(form.Name);
                        break;
                    }
                case "TablePartField" when row.Obj is ConfigurationField field && row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        tablePart.Fields.Remove(field.Name);
                        break;
                    }
                default:
                    break;
            }
        }

        Box box = new ConfiguratorDocumentsFieldsTree(document, Activate, new()
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
            Delete = (_, rows) =>
            {
                Message.Request(Program.BasicForm, "Питання", $"Видалити?", x =>
                {
                    if (x == Message.YesNo.Yes)
                        foreach (var row in rows)
                            Delete(row);
                });
            },
        }).Fill();

        Append(box);
    }
}
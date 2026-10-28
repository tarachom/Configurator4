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
/// Дерево даних для табличної частини
/// </summary>
[Subclass<DataTree>()]
public partial class TablePartDataTree : DataTree
{
    public static TablePartDataTree New() => NewWithProperties([]);

    public void SetValue(ConfigurationTablePart tablePart, ConfiguratorItemOwner? owner = null)
    {
        Owner = owner;

        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "Field" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, tablePart.Fields, field, new(ConfiguratorItemOwnerType.TablePart, tablePart, Owner));
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        await OpenPageTabularList(false, tablePart.TabularList, tablePart.Fields, tabularList);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        await OpenPageForm(false, tablePart.Forms, form, new(ConfiguratorItemOwnerType.TablePart, tablePart, Owner));
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
                        await OpenPageField(true, tablePart.Fields, null, new(ConfiguratorItemOwnerType.TablePart, tablePart, Owner));
                        break;
                    }
                case "TabularListGroup" or "TabularList":
                    {
                        await OpenPageTabularList(true, tablePart.TabularList, tablePart.Fields);
                        break;
                    }
                case "FormGroup" or "Form":
                    {
                        await OpenPageForm(true, tablePart.Forms, null, new(ConfiguratorItemOwnerType.TablePart, tablePart, Owner));
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
                        await OpenPageField(true, tablePart.Fields, newField, new(ConfiguratorItemOwnerType.TablePart, tablePart, Owner));
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        ConfigurationTabularList newTabularList = tabularList.Copy();
                        newTabularList.Name += Configuration.GetNewRandomName();
                        await OpenPageTabularList(true, tablePart.TabularList, tablePart.Fields, newTabularList);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        ConfigurationForms newForm = form.Copy();
                        newForm.Name += Configuration.GetNewRandomName();
                        await OpenPageForm(true, tablePart.Forms, newForm, new(ConfiguratorItemOwnerType.TablePart, tablePart, Owner));
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
                        tablePart.Fields.Remove(field.Name);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        tablePart.TabularList.Remove(tabularList.Name);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        tablePart.Forms.Remove(form.Name);
                        break;
                    }
                default:
                    break;
            }
        }

        Box box = new ConfiguratorTablePartsFieldsTree(tablePart, Activate, new()
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
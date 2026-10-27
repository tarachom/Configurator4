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
/// Дерево даних для регістру накопичення
/// </summary>
[Subclass<DataTree>()]
public partial class RegisterAccumulationDataTree : DataTree
{
    public static RegisterAccumulationDataTree New() => NewWithProperties([]);

    public void SetValue(ConfigurationRegistersAccumulation register)
    {
        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "DimensionField" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, register.DimensionFields, field, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "ResourcesField" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, register.ResourcesFields, field, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "PropertyField" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, register.PropertyFields, field, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageTablePart(false, register.TabularParts, tablePart);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        await OpenPageTabularList(false, register.TabularList, Configuration.CombineAllFieldForRegister(register), tabularList);
                        break;
                    }
                case "TabularListPrintBalances" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        await OpenPageTabularList(false, register.TabularListPrintBalances, Configuration.CombineAllFieldForRegister(register), tabularList);
                        break;
                    }
                case "QueryBlock" when row.Obj is ConfigurationQueryBlock block:
                    {
                        await OpenPageQueryBlock(false, register.QueryBlockList, block);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        await OpenPageForm(false, register.Forms, form);
                        break;
                    }
                case "TablePartField" when row.Obj is ConfigurationField field && row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageField(false, tablePart.Fields, field, new(ConfiguratorItemOwnerType.TablePart, tablePart));
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
                case "DimensionFields" or "DimensionField":
                    {
                        await OpenPageField(true, register.DimensionFields, null, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "ResourcesFields" or "ResourcesField":
                    {
                        await OpenPageField(true, register.ResourcesFields, null, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "PropertyFields" or "PropertyField":
                    {
                        await OpenPageField(true, register.PropertyFields, null, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "TablePartGroup" or "TablePart":
                    {
                        await OpenPageTablePart(true, register.TabularParts);
                        break;
                    }
                case "TabularListGroup" or "TabularList":
                    {
                        await OpenPageTabularList(true, register.TabularList, Configuration.CombineAllFieldForRegister(register));
                        break;
                    }
                case "TabularListPrintBalancesGroup" or "TabularListPrintBalances":
                    {
                        await OpenPageTabularList(true, register.TabularListPrintBalances, Configuration.CombineAllFieldForRegister(register));
                        break;
                    }
                case "QueryGroup" or "QueryBlock":
                    {
                        await OpenPageQueryBlock(true, register.QueryBlockList);
                        break;
                    }
                case "FormGroup" or "Form":
                    {
                        await OpenPageForm(true, register.Forms);
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
                case "DimensionField" when row.Obj is ConfigurationField field:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageField(true, register.DimensionFields, newField, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "ResourcesField" when row.Obj is ConfigurationField field:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageField(true, register.ResourcesFields, newField, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "PropertyField" when row.Obj is ConfigurationField field:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageField(true, register.PropertyFields, newField, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += Configuration.GetNewRandomName();
                        await OpenPageTablePart(true, register.TabularParts, newTablePart);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        ConfigurationTabularList newTabularList = tabularList.Copy();
                        newTabularList.Name += Configuration.GetNewRandomName();
                        await OpenPageTabularList(true, register.TabularList, Configuration.CombineAllFieldForRegister(register), newTabularList);
                        break;
                    }
                case "TabularListPrintBalances" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        ConfigurationTabularList newTabularList = tabularList.Copy();
                        newTabularList.Name += Configuration.GetNewRandomName();
                        await OpenPageTabularList(true, register.TabularListPrintBalances, Configuration.CombineAllFieldForRegister(register), newTabularList);
                        break;
                    }
                case "QueryBlock" when row.Obj is ConfigurationQueryBlock block:
                    {
                        ConfigurationQueryBlock newBlock = block.Copy();
                        newBlock.Name += Configuration.GetNewRandomName();
                        await OpenPageQueryBlock(true, register.QueryBlockList, newBlock);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        ConfigurationForms newForm = form.Copy();
                        newForm.Name += Configuration.GetNewRandomName();
                        await OpenPageForm(true, register.Forms, newForm, new(ConfiguratorItemOwnerType.RegisterAccumulation, register));
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
                case "DimensionField" when row.Obj is ConfigurationField field:
                    {
                        register.DimensionFields.Remove(field.Name);
                        break;
                    }
                case "ResourcesField" when row.Obj is ConfigurationField field:
                    {
                        register.ResourcesFields.Remove(field.Name);
                        break;
                    }
                case "PropertyField" when row.Obj is ConfigurationField field:
                    {
                        register.PropertyFields.Remove(field.Name);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        register.TabularList.Remove(tabularList.Name);
                        break;
                    }
                case "TabularListPrintBalances" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        register.TabularListPrintBalances.Remove(tabularList.Name);
                        break;
                    }
                case "QueryBlock" when row.Obj is ConfigurationQueryBlock block:
                    {
                        register.QueryBlockList.Remove(block.Name);
                        break;
                    }
                case "Form" when row.Obj is ConfigurationForms form:
                    {
                        register.Forms.Remove(form.Name);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        register.TabularParts.Remove(tablePart.Name);
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

        Box box = new ConfiguratorRegistersAccumulationFieldsTree(register, Activate, new()
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
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
/// Дерево даних для блоків констант
/// </summary>
[Subclass<DataTree>()]
public partial class ConstantsBlockDataTree : DataTree
{
    public static ConstantsBlockDataTree New() => NewWithProperties([]);

    public void SetValue(ConfigurationConstantsBlock block)
    {
        //Owner = new(ConfiguratorItemOwnerType.Constant, block);

        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "Const" when row.Obj is ConfigurationConstants constant:
                    {
                        await OpenPageConstant(false, constant);
                        break;
                    }
                case "TablePart" when row.ParentObj is ConfigurationConstants constant && row.Obj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageTablePart(false, constant.TabularParts, tablePart, new(ConfiguratorItemOwnerType.Constant, constant));
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
                case "ConstGroup" or "Const":
                    {
                        await OpenPageConstant(true, new() { Block = block });
                        break;
                    }
                case "TablePartGroup" when row.Obj is ConfigurationConstants constant:
                    {
                        await OpenPageTablePart(true, constant.TabularParts, null, new(ConfiguratorItemOwnerType.Constant, constant));
                        break;
                    }
                case "TablePart" when row.ParentObj is ConfigurationConstants constant:
                    {
                        await OpenPageTablePart(true, constant.TabularParts, null, new(ConfiguratorItemOwnerType.Constant, constant));
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
                case "Const" when row.Obj is ConfigurationConstants constant:
                    {
                        ConfigurationConstants newConstant = constant.Copy();
                        newConstant.Name += Configuration.GetNewRandomName();
                        await OpenPageConstant(true, newConstant);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart && row.ParentObj is ConfigurationConstants constant:
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += Configuration.GetNewRandomName();
                        await OpenPageTablePart(true, constant.TabularParts, newTablePart, new(ConfiguratorItemOwnerType.Constant, constant));
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
                case "Const" when row.Obj is ConfigurationConstants constant:
                    {
                        block.Constants.Remove(constant.Name);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart && row.ParentObj is ConfigurationConstants constant:
                    {
                        constant.TabularParts.Remove(tablePart.Name);
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

        Box box = new ConfiguratorConstantsBlockFieldsTree(block, Activate, new()
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
                if (await Message.Request(Program.BasicForm, "Питання", "Видалити?") == Message.YesNo.Yes)
                {
                    foreach (var row in rows)
                        Delete(row);
                }
            },
        }).Fill();

        Append(box);
    }
}
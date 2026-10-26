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
/// Дерево даних для регістру інформації
/// </summary>
[Subclass<DataTree>()]
public partial class RegisterInformationDataTree : DataTree
{
    public static RegisterInformationDataTree New() => NewWithProperties([]);

    public void SetValue(ConfigurationRegistersInformation register)
    {
        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "DimensionField" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, register.DimensionFields, field, new(ConfiguratorItemOwnerType.RegisterInformation, register));
                        break;
                    }
                case "ResourcesField" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, register.ResourcesFields, field, new(ConfiguratorItemOwnerType.RegisterInformation, register));
                        break;
                    }
                case "PropertyField" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, register.PropertyFields, field, new(ConfiguratorItemOwnerType.RegisterInformation, register));
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
                        await OpenPageField(true, register.DimensionFields, null, new(ConfiguratorItemOwnerType.RegisterInformation, register));
                        break;
                    }
                case "ResourcesFields" or "ResourcesField":
                    {
                        await OpenPageField(true, register.ResourcesFields, null, new(ConfiguratorItemOwnerType.RegisterInformation, register));
                        break;
                    }
                case "PropertyFields" or "PropertyField":
                    {
                        await OpenPageField(true, register.PropertyFields, null, new(ConfiguratorItemOwnerType.RegisterInformation, register));
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
                        await OpenPageField(true, register.DimensionFields, newField, new(ConfiguratorItemOwnerType.RegisterInformation, register));
                        break;
                    }
                case "ResourcesField" when row.Obj is ConfigurationField field:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageField(true, register.ResourcesFields, newField, new(ConfiguratorItemOwnerType.RegisterInformation, register));
                        break;
                    }
                case "PropertyField" when row.Obj is ConfigurationField field:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += Configuration.GetNewRandomName();
                        await OpenPageField(true, register.PropertyFields, newField, new(ConfiguratorItemOwnerType.RegisterInformation, register));
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
                default:
                    break;
            }
        }

        Box box = new ConfiguratorRegistersInformationFieldsTree(register, Activate, new()
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
using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

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
using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

[Subclass<DataTree>()]
public partial class DirectoryDataTree : DataTree
{
    public static DirectoryDataTree New()
    {
        DirectoryDataTree w = NewWithProperties([]);
        return w;
    }

    public void SetValue(ConfigurationDirectories directory)
    {
        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "Field" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, directory.Table, directory.Fields, field);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageTablePart(false, directory.TabularParts, tablePart);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        await OpenPageTabularList(false, directory.TabularList, directory.Fields, tabularList);
                        break;
                    }
                case "TablePartField" when row.Obj is ConfigurationField field && row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageField(false, tablePart.Table, tablePart.Fields, field);
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
                        await OpenPageField(true, directory.Table, directory.Fields);
                        break;
                    }
                case "TablePartGroup" or "TablePart":
                    {
                        await OpenPageTablePart(true, directory.TabularParts);
                        break;
                    }
                case "TabularListGroup" or "TabularList":
                    {
                        await OpenPageTabularList(true, directory.TabularList, directory.Fields);
                        break;
                    }
                case "FormGroup" or "Form":
                    {
                        break;
                    }
                case "TablePartField" when row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        await OpenPageField(true, tablePart.Table, tablePart.Fields);
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
                        newField.Name += GenerateName.GetNewName();
                        await OpenPageField(true, directory.Table, directory.Fields, newField);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        await OpenPageTablePart(true, directory.TabularParts, newTablePart);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        ConfigurationTabularList newTabularList = tabularList.Copy();
                        newTabularList.Name += GenerateName.GetNewName();
                        await OpenPageTabularList(true, directory.TabularList, directory.Fields, newTabularList);
                        break;
                    }
                case "Form":
                    {
                        break;
                    }
                case "TablePartField" when row.Obj is ConfigurationField field && row.ParentObj is ConfigurationTablePart tablePart:
                    {
                        ConfigurationField newField = field.Copy();
                        newField.Name += GenerateName.GetNewName();
                        await OpenPageField(true, tablePart.Table, tablePart.Fields, newField);
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
                        directory.Fields.Remove(field.Name);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        directory.TabularParts.Remove(tablePart.Name);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        directory.TabularList.Remove(tabularList.Name);
                        break;
                    }
                case "Form":
                    {
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

        Box box = new ConfiguratorDirectoriesFieldsTree(directory, Activate, new()
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
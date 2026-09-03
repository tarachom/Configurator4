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
                case "TablePart":
                    {

                        break;
                    }
                case "TablePartField":
                    {

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
                        break;
                    }
                case "TabularListGroup" or "TabularList":
                    {
                        break;
                    }
                case "FormGroup" or "Form":
                    {
                        break;
                    }
                case "TablePartField":
                    {
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
                case "TablePart":
                    {
                        break;
                    }
                case "TabularList":
                    {
                        break;
                    }
                case "Form":
                    {
                        break;
                    }
                case "TablePartField":
                    {
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
                case "TablePart":
                    {
                        break;
                    }
                case "TabularList":
                    {
                        break;
                    }
                case "Form":
                    {
                        break;
                    }
                case "TablePartField":
                    {
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
using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

[Subclass<DataTree>()]
public partial class TablePartDataTree : DataTree
{
    public static TablePartDataTree New()
    {
        TablePartDataTree w = NewWithProperties([]);
        return w;
    }

    public void SetValue(ConfigurationTablePart tablePart)
    {
        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "Field" when row.Obj is ConfigurationField field:
                    {
                        await OpenPageField(false, tablePart.Table, tablePart.Fields, field);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        await OpenPageTabularList(false, tablePart.TabularList, tablePart.Fields, tabularList);
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
                        await OpenPageField(true, tablePart.Table, tablePart.Fields);
                        break;
                    }
                case "TabularListGroup" or "TabularList":
                    {
                        await OpenPageTabularList(true, tablePart.TabularList, tablePart.Fields);
                        break;
                    }
                case "FormGroup" or "Form":
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
                        await OpenPageField(true, tablePart.Table, tablePart.Fields, newField);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        ConfigurationTabularList newTabularList = tabularList.Copy();
                        newTabularList.Name += GenerateName.GetNewName();
                        await OpenPageTabularList(true, tablePart.TabularList, tablePart.Fields, newTabularList);
                        break;
                    }
                case "Form":
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
                        tablePart.Fields.Remove(field.Name);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        tablePart.TabularList.Remove(tabularList.Name);
                        break;
                    }
                case "Form":
                    {
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
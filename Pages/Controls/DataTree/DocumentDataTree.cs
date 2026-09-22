using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

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
                        await OpenPageField(false, document.Table, document.Fields, field);
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
                        await OpenPageField(true, document.Table, document.Fields);
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
                        await OpenPageField(true, document.Table, document.Fields, newField);
                        break;
                    }
                case "TablePart" when row.Obj is ConfigurationTablePart tablePart:
                    {
                        ConfigurationTablePart newTablePart = tablePart.Copy();
                        newTablePart.Name += GenerateName.GetNewName();
                        await OpenPageTablePart(true, document.TabularParts, newTablePart);
                        break;
                    }
                case "TabularList" when row.Obj is ConfigurationTabularList tabularList:
                    {
                        ConfigurationTabularList newTabularList = tabularList.Copy();
                        newTabularList.Name += GenerateName.GetNewName();
                        await OpenPageTabularList(true, document.TabularList, document.Fields, newTabularList);
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
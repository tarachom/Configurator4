using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

[Subclass<DataTree>()]
public partial class DocumentDataTree:DataTree
{
    public static DocumentDataTree New()
    {
        DocumentDataTree w = NewWithProperties([]);
        return w;
    }

    public void SetValue(ConfigurationDocuments documents)
    {
        async void Activate(ConfiguratorItemRow row)
        {
            switch (row.Group)
            {
                case "Documents":
                    {

                        break;
                    }
                case "Field":
                    {

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

        async void Add()
        {

        }

        Box box = new ConfiguratorDocumentsFieldsTree(documents, Activate, new()
        {
            Add = () => Add(),
            Edit = (row) => Activate(row),
            Copy = (row) =>
            {

            },
            Delete = (row) =>
            {

            }
        }).Fill();

        Append(box);
    }
}
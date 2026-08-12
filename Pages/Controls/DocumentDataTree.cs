using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

[Subclass<Box>("DocumentDataTree")]
[Template<AssemblyResource>("DocumentDataTree.xml")]
public partial class DocumentDataTree
{
    public static DocumentDataTree New()
    {
        DocumentDataTree dt = NewWithProperties([]);
        return dt;
    }

    public void SetValue(ConfigurationDocuments documents)
    {
        async void Activate(string group, string name)
        {
            switch (group)
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
            Edit = (group, name) => Activate(group, name),
            Copy = (group, name) =>
            {

            },
            Delete = (group, name) =>
            {

            }
        }).Fill();

        Append(box);
    }
}
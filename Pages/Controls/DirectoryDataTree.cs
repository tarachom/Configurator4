using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

[Subclass<Box>("DirectoryDataTree")]
[Template<AssemblyResource>("DirectoryDataTree.xml")]
public partial class DirectoryDataTree
{
    public static DirectoryDataTree New()
    {
        DirectoryDataTree dt = NewWithProperties([]);
        return dt;
    }

    public void SetValue(ConfigurationDirectories directory)
    {
        async void Activate(string group, string name)
        {
            switch (group)
            {
                case "Directories":
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

        Box box = new ConfiguratorDirectoriesFieldsTree(directory, Activate, new()
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
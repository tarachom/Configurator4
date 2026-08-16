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
            Console.WriteLine(row.Group);
            switch (row.Group)
            {
                case "Directories":
                    {

                        break;
                    }
                case "Field":
                    {
                        PageField page = PageField.New();
                        Program.BasicForm?.NotebookFunc.CreatePage(page.Caption, page);
                        await page.SetValue();

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
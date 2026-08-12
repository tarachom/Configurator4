using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

[Subclass<Box>("DocumentAutomaticNumbering")]
[Template<AssemblyResource>("DocumentAutomaticNumbering.xml")]
public partial class DocumentAutomaticNumbering
{
    [Connect("check_automatic_numbering")] CheckButton checkAutomaticNumbering;
    [Connect("button_create")] Button buttonCreate;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationDocuments ConfDocuments { get; set; } = new();

    public static DocumentAutomaticNumbering New()
    {
        DocumentAutomaticNumbering dt = NewWithProperties([]);
        return dt;
    }

    partial void Initialize()
    {
        buttonCreate.OnClicked += (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDocuments.Name) || !Conf.Directories.ContainsKey(ConfDocuments.Name))
            {
                Message.Error(Program.BasicForm, "Назва документу не задана або документ не збережений! Документ треба спочатку зберегти!");
                return;
            }

            string block = "НумераціяДокументів";

            if (!Conf.ConstantsBlock.ContainsKey(block))
                Conf.AppendConstantsBlock(new ConfigurationConstantsBlock(block, "Нумерація документів"));

            ConfigurationConstantsBlock blockAutoNum = Conf.ConstantsBlock[block];

            //Назва поля в таблиці
            string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, SpecialTables.Constants, Program.BasicForm!.GetConstantsAllFields());

            if (!blockAutoNum.Constants.ContainsKey(ConfDocuments.Name))
                blockAutoNum.AppendConstant(new ConfigurationConstants(ConfDocuments.Name, nameInTable, "integer", blockAutoNum));

            checkAutomaticNumbering.Active = true;
        };
    }

    public void SetValue(ConfigurationDocuments confDocuments)
    {
        ConfDocuments = confDocuments;

        checkAutomaticNumbering.Active = ConfDocuments.AutomaticNumeration;
    }

    public void GetValue()
    {
        ConfDocuments.AutomaticNumeration = checkAutomaticNumbering.Active;
    }
}
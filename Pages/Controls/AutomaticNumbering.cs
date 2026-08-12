using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

[Subclass<Box>("AutomaticNumbering")]
[Template<AssemblyResource>("AutomaticNumbering.xml")]
public partial class AutomaticNumbering
{
    [Connect("check_automatic_numbering")] CheckButton checkAutomaticNumbering;
    [Connect("button_create")] Button buttonCreate;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    public static AutomaticNumbering New()
    {
        AutomaticNumbering dt = NewWithProperties([]);
        return dt;
    }

    partial void Initialize()
    {
        buttonCreate.OnClicked += (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDirectory.Name) || !Conf.Directories.ContainsKey(ConfDirectory.Name))
            {
                Message.Error(Program.BasicForm, "Назва довідника не задана або довідник не збережений! Довідник треба спочатку зберегти!");
                return;
            }

            string block = "НумераціяДовідників";

            if (!Conf.ConstantsBlock.ContainsKey(block))
                Conf.AppendConstantsBlock(new ConfigurationConstantsBlock(block, "Нумерація довідників"));

            ConfigurationConstantsBlock blockAutoNum = Conf.ConstantsBlock[block];

            //Назва поля в таблиці
            string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, SpecialTables.Constants, Program.BasicForm!.GetConstantsAllFields());

            if (!blockAutoNum.Constants.ContainsKey(ConfDirectory.Name))
                blockAutoNum.AppendConstant(new ConfigurationConstants(ConfDirectory.Name, nameInTable, "integer", blockAutoNum));

            checkAutomaticNumbering.Active = true;
        };
    }

    public void SetValue(ConfigurationDirectories confDirectory)
    {
        ConfDirectory = confDirectory;

        checkAutomaticNumbering.Active = ConfDirectory.AutomaticNumeration;
    }

    public void GetValue()
    {
        ConfDirectory.AutomaticNumeration = checkAutomaticNumbering.Active;
    }
}
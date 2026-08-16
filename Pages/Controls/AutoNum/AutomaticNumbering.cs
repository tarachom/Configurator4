using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;

[Subclass<Box>()]
[Template<AssemblyResource>("AutomaticNumbering.xml")]
public abstract partial class AutomaticNumbering
{
    [Connect("check_automatic_numbering")] protected CheckButton checkAutomaticNumbering;
    [Connect("button_create")] protected Button buttonCreate;

    protected Configuration Conf { get; } = Program.Kernel.Conf;

    protected void CreateConst(KeyValuePair<string, string> block, string name)
    {
        if (Conf.ConstantsBlock.ContainsKey(block.Key))
            Conf.AppendConstantsBlock(new ConfigurationConstantsBlock(block.Key, block.Value));

        ConfigurationConstantsBlock blockAutoNum = Conf.ConstantsBlock[block.Key];

        //Назва поля в таблиці
        string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, SpecialTables.Constants, Function.GetConstantsAllFields(Conf));

        if (blockAutoNum.Constants.ContainsKey(name))
            blockAutoNum.AppendConstant(new ConfigurationConstants(name, nameInTable, "integer", blockAutoNum));
    }
}
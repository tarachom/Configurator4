using AccountingSoftware;

namespace Configurator;

public static class Function
{
    public static Dictionary<string, ConfigurationField> GetConstantsAllFields(Configuration conf)
    {
        Dictionary<string, ConfigurationField> ConstantsAllFields = [];

        foreach (ConfigurationConstantsBlock block in conf.ConstantsBlock.Values)
            foreach (ConfigurationConstants constants in block.Constants.Values)
            {
                string fullName = block.BlockName + "." + constants.Name;
                ConstantsAllFields.Add(fullName, new ConfigurationField(fullName, fullName, constants.NameInTable, constants.Type, constants.Pointer, constants.Desc));
            }

        return ConstantsAllFields;
    }

    public static string FindNewFieldName(Dictionary<string, ConfigurationField> fields, string fieldName, Func<ConfigurationField, bool> func)
    {
        string newFieldName = fieldName;
        for (int i = 1; i <= 10; i++)
            if (fields.TryGetValue(newFieldName, out var field))
            {
                if (func.Invoke(field))
                    newFieldName = fieldName + i.ToString();
                else
                    break;
            }
            else
                break;

        return newFieldName;
    }

    /// <summary>
    /// Функція заповнює новий довідник початковими даними
    /// </summary>
    /// <param name="confDirectory">Довідник</param>
    public static async Task<bool> FillNewDirectory(ConfigurationDirectories confDirectory, List<ConfigurationField>? otherFields = null)
    {
        confDirectory.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

        //Заповнення полями
        {
            //Код
            {
                string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, confDirectory.Table, confDirectory.Fields);
                confDirectory.AppendField(new ConfigurationField("Код", "Код", nameInTable, "string", "", "Код", false, true, false, true));
            }

            //Назва
            {
                string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, confDirectory.Table, confDirectory.Fields);
                confDirectory.AppendField(new ConfigurationField("Назва", "Назва", nameInTable, "string", "", "Назва", true, true, false, true));
            }

            if (otherFields != null)
                foreach (var otherField in otherFields)
                {
                    otherField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, confDirectory.Table, confDirectory.Fields);
                    confDirectory.AppendField(otherField);
                }
        }

        //Табличний список
        {
            ConfigurationTabularList list = new("Записи");
            int sortNum = 0;

            //Заповнення полями
            foreach (var item in confDirectory.Fields.Values)
                list.AppendField(new(item.Name, item.Name, 0, ++sortNum, item.Name == "Назва"));

            //Заповнення списку
            confDirectory.AppendTableList(list);
        }

        //Форми
        {
            {
                string name = "Функції";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.Function);
                confDirectory.AppendForms(forms);
            }

            {
                string name = "Тригери";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.Triggers);
                confDirectory.AppendForms(forms);
            }

            {
                string name = "Реквізит вибору";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.PointerControl);
                confDirectory.AppendForms(forms);
            }

            {
                string name = "Реквізит вибору для таб частини";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.PointerTablePartCell);
                confDirectory.AppendForms(forms);
            }

            {
                string name = "Швидкий вибір";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.ListSmallSelect);
                confDirectory.AppendForms(forms);
            }

            {
                string name = "Список";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.List);
                confDirectory.AppendForms(forms);
            }
        }

        //Тригери
        confDirectory.TriggerFunctions.NewAction = true;
        confDirectory.TriggerFunctions.CopyingAction = true;

        return true;
    }
}
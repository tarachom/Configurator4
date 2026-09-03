using AccountingSoftware;

namespace Configurator;

public static class Function
{
    /// <summary>
    /// Функція формує масив всіх полів констант
    /// </summary>
    /// <param name="conf">Конфігурація</param>
    /// <returns></returns>
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

    /// <summary>
    /// Функція підбирає нову назву для поля якщо вже є таке поле (fieldName) і не спіпадає тип даних (тобто func поверне true)
    /// </summary>
    /// <param name="fields">Всі поля</param>
    /// <param name="fieldName">Поле</param>
    /// <param name="func"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Функція заповнює новий документ початковими даними
    /// </summary>
    /// <param name="confDirectory">Документ</param>
    public static async Task<bool> FillNewDocument(ConfigurationDocuments confDocument, List<ConfigurationField>? otherFields = null)
    {
        confDocument.Table = await Configuration.GetNewUnigueTableName(Program.Kernel);

        //Заповнення полями
        {
            confDocument.AppendField(new ConfigurationField("Назва", "Назва", "docname", "string", "", "Назва", true, true));
            confDocument.AppendField(new ConfigurationField("НомерДок", "Номер", "docnomer", "string", "", "Номер документу", false, true));
            confDocument.AppendField(new ConfigurationField("ДатаДок", "Дата", "docdate", "datetime", "", "Дата документу", false, true));

            //Коментар
            {
                string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, confDocument.Table, confDocument.Fields);
                confDocument.AppendField(new ConfigurationField("Коментар", "Коментар", nameInTable, "string", "", "Коментар"));
            }

            //Підстава
            {
                string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, confDocument.Table, confDocument.Fields);
                confDocument.AppendField(new ConfigurationField("Підстава", "Підстава", nameInTable, "composite_pointer", "", "Підстава"));
            }

            if (otherFields != null)
                foreach (var otherField in otherFields)
                {
                    otherField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, confDocument.Table, confDocument.Fields);
                    confDocument.AppendField(otherField);
                }
        }

        //Табличний список
        {
            ConfigurationTabularList list = new("Записи");
            int sortNum = 0;
            string[] typesIgnor = ["composite_pointer"];

            //Заповнення полями списків (крім типів які ігноруються)
            foreach (var item in confDocument.Fields.Values.Where(x => typesIgnor.Contains(x.Type)))
            {
                string caption = item.Name switch { "ДатаДок" => "Дата", "НомерДок" => "Номер", _ => item.Name };
                list.AppendField(new ConfigurationTabularListField(item.Name, caption, 0, ++sortNum, item.Name == "ДатаДок"));
            }

            //Заповнення списку
            confDocument.AppendTableList(list);
        }

        //Форми
        {
            {
                string name = "Функції";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.Function);
                confDocument.AppendForms(forms);
            }

            {
                string name = "Тригери";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.Triggers);
                confDocument.AppendForms(forms);
            }

            {
                string name = "Реквізит вибору";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.PointerControl);
                confDocument.AppendForms(forms);
            }

            {
                string name = "Реквізит вибору для таб частини";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.PointerTablePartCell);
                confDocument.AppendForms(forms);
            }

            {
                string name = "Швидкий вибір";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.ListSmallSelect);
                confDocument.AppendForms(forms);
            }

            {
                string name = "Список";
                ConfigurationForms forms = new(name, name, ConfigurationForms.TypeForms.List);
                confDocument.AppendForms(forms);
            }
        }

        //Тригери
        confDocument.TriggerFunctions.NewAction = true;
        confDocument.TriggerFunctions.CopyingAction = true;

        return true;
    }

    /// <summary>
    /// Функція заповнює нове поле
    /// </summary>
    /// <param name="confField"></param>
    /// <param name="parentTable"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    public static async Task<bool> FillNewField(ConfigurationField confField, string parentTable, Dictionary<string, ConfigurationField> fields)
    {
        confField.NameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, parentTable, fields);

        return true;
    }
}
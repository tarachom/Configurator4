/*

Стартова сторінка

*/

using Gtk;
using InterfaceGtkLib;
using InterfaceGtk4;
using AccountingSoftware;
using System.Xml.XPath;
using System.Text;

namespace Configurator;

[GObject.Subclass<Form>]
partial class PageSaveConfiguration : Form
{
    Configuration Conf = Program.Kernel.Conf;

    string PathToXsltTemplate = AppContext.BaseDirectory;

    #region Fields

    CheckButton checkButtonIsGenerate = CheckButton.NewWithLabel("Генерувати код");
    Entry entryGenerateCodePath = Entry.New();
    Entry entryCompileProgramPath = Entry.New();
    Button bSaveParam = Button.NewWithLabel("Зберегти параметри");

    Button bAnalize = Button.NewWithLabel("Аналіз змін");
    Button bAnalizeAndCreateSQL = Button.NewWithLabel("Збереження змін. Крок 1");
    Button bExecuteSQLAndGenerateCode = Button.NewWithLabel("Збереження змін. Крок 2");

    ScrolledWindow scrollListBoxTerminal = ScrolledWindow.New();
    TextView textTerminal = TextView.New();

    #endregion

    partial void Initialize()
    {
        Expander expanderParams = Expander.New("Параметри та додаткові налаштування");
        expanderParams.MarginStart = expanderParams.MarginEnd = 10;
        Append(expanderParams);

        Box vBoxParams = Box.New(Orientation.Vertical, 0);
        expanderParams.SetChild(vBoxParams);

        //Параметри 1
        Box hBoxParamIsGenerate = Box.New(Orientation.Horizontal, 0);
        vBoxParams.Append(hBoxParamIsGenerate);

        hBoxParamIsGenerate.Append(checkButtonIsGenerate);

        //Параметри 2
        Box hBoxParamPath = Box.New(Orientation.Horizontal, 0);
        vBoxParams.Append(hBoxParamPath);

        entryGenerateCodePath.WidthRequest = 500;

        hBoxParamPath.Append(Label.New("Шлях до папки куди генерувати код:"));
        hBoxParamPath.Append(entryGenerateCodePath);

        Button bSelectFolderGenerateCode = Button.NewWithLabel("...");
        //bSelectFolderGenerateCode.OnClicked += OnSelectFolderGenerateCode;
        hBoxParamPath.Append(bSelectFolderGenerateCode);

        hBoxParamPath.Append(Label.New("За замовчуванням код генерується в каталог програми"));

        //Параметри 3
        Box hBoxParamCompileProgram = Box.New(Orientation.Horizontal, 0);
        vBoxParams.Append(hBoxParamCompileProgram);

        entryCompileProgramPath.WidthRequest = 500;

        hBoxParamCompileProgram.Append(Label.New("Шлях до папки скомпільованої програми:"));
        hBoxParamCompileProgram.Append(entryCompileProgramPath);

        Button bSelectFolderCompileProgram = Button.NewWithLabel("...");
        //bSelectFolderCompileProgram.OnClicked += OnSelectFolderCompileProgram;
        hBoxParamCompileProgram.Append(bSelectFolderCompileProgram);

        hBoxParamCompileProgram.Append(Label.New("Наприклад 'bin/Debug/net10.0/'. В цю папку буде скопійований файл Confa.xml"));

        //Save
        Box hBoxSaveParam = Box.New(Orientation.Horizontal, 0);
        vBoxParams.Append(hBoxSaveParam);

        bSaveParam.OnClicked += OnSaveParam;
        hBoxSaveParam.Append(bSaveParam);

        Append(Separator.New(Orientation.Horizontal));

        //Кнопки
        Box hBox = Box.New(Orientation.Horizontal, 0);

        bAnalize.OnClicked += async (_, _) => await SaveAndAnalize();
        hBox.Append(bAnalize);

        bAnalizeAndCreateSQL.OnClicked += async (_, _) => await SaveAnalizeAndCreateSQL();
        hBox.Append(bAnalizeAndCreateSQL);

        bExecuteSQLAndGenerateCode.OnClicked += async (_, _) => await ExecuteSQLAndGenerateCode();
        hBox.Append(bExecuteSQLAndGenerateCode);

        Append(hBox);

        //Terminal
        Box hBoxTerminal = Box.New(Orientation.Horizontal, 0);
        hBoxTerminal.Vexpand = hBoxTerminal.Hexpand = true;
        Append(hBoxTerminal);

        scrollListBoxTerminal.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
        scrollListBoxTerminal.Vexpand = scrollListBoxTerminal.Hexpand = true;
        scrollListBoxTerminal.HasFrame = true;
        scrollListBoxTerminal.SetChild(textTerminal);

        textTerminal.Buffer?.Text = "Text Text Text";

        hBoxTerminal.Append(scrollListBoxTerminal);
    }

    public static PageSaveConfiguration New()
    {
        PageSaveConfiguration view = NewWithProperties([]);
        view.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return view;
    }

    public void SetValue()
    {
        if (Program.BasicForm != null)
        {
            ConfigurationParam? openConfigurationParam = Program.BasicForm.OpenConfigurationParam;
            if (openConfigurationParam != null)
            {
                //1
                if (openConfigurationParam.OtherParam.TryGetValue("IsGenerateCode", out string? IsGenerateCode))
                    checkButtonIsGenerate.Active = IsGenerateCode == "True";

                //2
                if (openConfigurationParam.OtherParam.TryGetValue("GenerateCodePath", out string? GenerateCodePath))
                    entryGenerateCodePath.SetText(GenerateCodePath);

                //3
                if (openConfigurationParam.OtherParam.TryGetValue("CompileProgramPath", out string? CompileProgramPath))
                    entryCompileProgramPath.SetText(CompileProgramPath);
            }
        }
    }

    void OnSaveParam(Button button, EventArgs arg)
    {
        if (Program.BasicForm != null)
        {
            ConfigurationParam? openConfigurationParam = Program.BasicForm.OpenConfigurationParam;
            if (openConfigurationParam != null)
            {
                //1
                if (openConfigurationParam.OtherParam.ContainsKey("IsGenerateCode"))
                    openConfigurationParam.OtherParam["IsGenerateCode"] = checkButtonIsGenerate.Active.ToString();
                else
                    openConfigurationParam.OtherParam.Add("IsGenerateCode", checkButtonIsGenerate.Active.ToString());

                //2
                if (openConfigurationParam.OtherParam.ContainsKey("GenerateCodePath"))
                    openConfigurationParam.OtherParam["GenerateCodePath"] = entryGenerateCodePath.GetText();
                else
                    openConfigurationParam.OtherParam.Add("GenerateCodePath", entryGenerateCodePath.GetText());

                //3
                if (openConfigurationParam.OtherParam.ContainsKey("CompileProgramPath"))
                    openConfigurationParam.OtherParam["CompileProgramPath"] = entryCompileProgramPath.GetText();
                else
                    openConfigurationParam.OtherParam.Add("CompileProgramPath", entryCompileProgramPath.GetText());
            }

            ConfigurationParamCollection.SaveConfigurationParamFromXML(ConfigurationParamCollection.PathToXML);
        }
    }

    void ApendLine(string message)
    {
        if (textTerminal != null && textTerminal.Buffer != null)
        {
            //Поміщення курсору в кінець тексту
            textTerminal.Buffer.GetEndIter(out TextIter iterEndText);
            textTerminal.Buffer.PlaceCursor(iterEndText);

            string text = message + "\n";
            textTerminal.Buffer.InsertAtCursor(text, Encoding.UTF8.GetBytes(text).Length);

            scrollListBoxTerminal.Vadjustment?.Value = scrollListBoxTerminal.Vadjustment.Upper;
        }
        ;
    }

    void ClearListBoxTerminal()
    {
        textTerminal.Buffer?.Text = "";
    }

    static string GetNameFromType(string Type)
    {
        return Type switch
        {
            "Constants" => "Константи",
            "Constants.TablePart" => "Константи.Таблична частина",
            "Directory" => "Довідник",
            "Directory.TablePart" => "Довідник.Таблична частина",
            "Document" => "Документ",
            "Document.TablePart" => "Документ.Таблична частина",
            "RegisterInformation" => "Регістер відомостей",
            "RegisterInformation.TablePart" => "Регістер відомостей.Таблична частина",
            "RegisterAccumulation" => "Регістер накопичення",
            "RegisterAccumulation.TablePart" => "Регістер накопичення.Таблична частина",
            _ => "<Невідомий тип>",
        };
    }

    void InfoTableCreateFieldCreate(XPathNavigator? xPathNavigator, string tab)
    {
        XPathNodeIterator? nodeField = xPathNavigator?.Select("TableCreate/FieldCreate");
        while (nodeField!.MoveNext())
        {
            XPathNavigator? nodeName = nodeField?.Current?.SelectSingleNode("Name");
            XPathNavigator? nodeConfType = nodeField?.Current?.SelectSingleNode("ConfType");

            ApendLine(tab + "Поле: " + nodeName?.Value + "(Тип: " + nodeConfType?.Value + ")");
        }
    }

    void ButtonSensitive(bool sensitive)
    {
        bAnalize.Sensitive = sensitive;
        bAnalizeAndCreateSQL.Sensitive = sensitive;
        bExecuteSQLAndGenerateCode.Sensitive = sensitive;
        textTerminal.Sensitive = sensitive;
    }

    async ValueTask SaveAndAnalize()
    {
        ButtonSensitive(false);

        ClearListBoxTerminal();

        ApendLine("[ КОНФІГУРАЦІЯ ]\n");

        ApendLine("1. Створення копії файлу конфігурації");
        Conf.PathToCopyXmlFileConfiguration = Configuration.CreateCopyConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToCopyXmlFileConfiguration);
        ApendLine(" --> " + Conf.PathToCopyXmlFileConfiguration + "\n");

        string fullPathToCopyXmlFileConguratifion = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration);
        Conf.PathToTempXmlFileConfiguration = Configuration.GetTempPathToConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToTempXmlFileConfiguration);

        ApendLine("2. Збереження конфігурації у тимчасовий файл");
        Configuration.Save(Conf.PathToTempXmlFileConfiguration, Conf);
        ApendLine(" --> " + Conf.PathToTempXmlFileConfiguration + "\n");

        ApendLine("3. Отримання структури бази даних");
        ConfigurationInformationSchema informationSchema = await Program.Kernel.DataBase.SelectInformationSchema();

        if (informationSchema.Tables.Count > 0)
        {
            string informationSchemaFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml");

            Configuration.SaveInformationSchema(informationSchema, informationSchemaFile);
            ApendLine(" --> " + informationSchemaFile + "\n");

            ApendLine("4. Створення загального файлу для порівняння");

            string oneFileForComparison = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml");

            Configuration.CreateOneFileForComparison(
                informationSchemaFile,
                Conf.PathToTempXmlFileConfiguration,
                fullPathToCopyXmlFileConguratifion,
                oneFileForComparison
            );
            ApendLine(" --> " + oneFileForComparison + "\n");

            ApendLine("5. Порівняння конфігурації та бази даних");

            string comparisonFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml");

            try
            {
                Configuration.Comparison(
                    oneFileForComparison,
                    System.IO.Path.Combine(PathToXsltTemplate, "xslt/Comparison.xslt"),
                    comparisonFile
                );
            }
            catch (Exception ex)
            {
                ApendLine(ex.Message);
                return;
            }

            ApendLine(" --> " + comparisonFile + "\n");

            XPathDocument xPathDoc = new XPathDocument(
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml")
            );
            XPathNavigator xPathDocNavigator = xPathDoc.CreateNavigator();

            XPathNodeIterator nodeDeleteDirectory = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'delete']");
            int counterDelete = nodeDeleteDirectory?.Count ?? 0;
            ApendLine("Видалених: " + counterDelete + "\n");

            while (nodeDeleteDirectory!.MoveNext())
            {
                XPathNavigator? nodeName = nodeDeleteDirectory?.Current?.SelectSingleNode("Name");
                XPathNavigator? nodeTable = nodeDeleteDirectory?.Current?.SelectSingleNode("Table");
                XPathNavigator? nodeType = nodeDeleteDirectory?.Current?.SelectSingleNode("Type");

                ApendLine("Видалений " + GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
            }

            XPathNodeIterator nodeNewDirectory = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'no']");
            int counterNew = nodeNewDirectory?.Count ?? 0;
            ApendLine("Нових: " + counterNew + "\n");

            while (nodeNewDirectory!.MoveNext())
            {
                XPathNavigator? nodeName = nodeNewDirectory?.Current?.SelectSingleNode("Name");
                XPathNavigator? nodeType = nodeNewDirectory?.Current?.SelectSingleNode("Type");
                ApendLine("Новий " + GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");

                InfoTableCreateFieldCreate(nodeNewDirectory?.Current, "\t ");
                ApendLine("");

                XPathNodeIterator? nodeDirectoryTabularParts = nodeNewDirectory?.Current?.Select("Control_TabularParts");
                while (nodeDirectoryTabularParts!.MoveNext())
                {
                    XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                    ApendLine("\t Нова таблична частина: " + nodeTabularPartsName?.Value + "\n");

                    InfoTableCreateFieldCreate(nodeDirectoryTabularParts?.Current, "\t\t ");
                }
            }

            XPathNodeIterator nodeDirectoryExist = xPathDocNavigator.Select("/root/Control_Table[IsExist = 'yes']");
            ApendLine("Зміни:\n");

            while (nodeDirectoryExist!.MoveNext())
            {
                bool flag = false;

                XPathNodeIterator? nodeDirectoryDeleteField = nodeDirectoryExist.Current?.Select("Control_Field[IsExist = 'delete']");
                if (nodeDirectoryDeleteField?.Count > 0)
                {
                    XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                    ApendLine(GetNameFromType(nodeType!.Value) + ": " + nodeName?.Value + "\n");
                    flag = true;
                }
                while (nodeDirectoryDeleteField!.MoveNext())
                {
                    XPathNavigator? nodeFieldName = nodeDirectoryDeleteField?.Current?.SelectSingleNode("Name");
                    ApendLine("\t Видалене Поле: " + nodeFieldName?.Value + "\n");
                }

                XPathNodeIterator? nodeDirectoryNewField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'no']");
                if (nodeDirectoryNewField?.Count > 0)
                {
                    XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                    ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                    flag = true;
                }
                while (nodeDirectoryNewField!.MoveNext())
                {
                    XPathNavigator? nodeFieldName = nodeDirectoryNewField?.Current?.SelectSingleNode("Name");
                    ApendLine("\t Нове Поле: " + nodeFieldName?.Value + "\n");
                }

                XPathNodeIterator? nodeDirectoryClearField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'clear']");
                if (nodeDirectoryClearField?.Count > 0 && flag == false)
                {
                    XPathNavigator? nodeName = nodeDirectoryClearField?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeType = nodeDirectoryClearField?.Current?.SelectSingleNode("Type");
                    ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                    flag = true;
                }
                while (nodeDirectoryClearField!.MoveNext())
                {
                    XPathNavigator? nodeFieldName = nodeDirectoryClearField?.Current?.SelectSingleNode("../Name");
                    ApendLine("\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних. Можлива втрата даних!" + "\n");
                }

                XPathNodeIterator? nodeDirectoryExistField = nodeDirectoryExist?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'no']");
                if (nodeDirectoryExistField?.Count > 0 && flag == false)
                {
                    XPathNavigator? nodeName = nodeDirectoryExistField?.Current?.SelectSingleNode("Name");
                    XPathNavigator? nodeType = nodeDirectoryExistField?.Current?.SelectSingleNode("Type");
                    ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                    flag = true;
                }
                while (nodeDirectoryExistField!.MoveNext())
                {
                    XPathNavigator? nodeFieldName = nodeDirectoryExistField?.Current?.SelectSingleNode("../Name");
                    XPathNavigator? nodeDataType = nodeDirectoryExistField?.Current?.SelectSingleNode("DataType");
                    XPathNavigator? nodeUdtName = nodeDirectoryExistField?.Current?.SelectSingleNode("UdtName");
                    XPathNavigator? nodeDataTypeCreate = nodeDirectoryExistField?.Current?.SelectSingleNode("DataTypeCreate");

                    ApendLine("\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних (Тип в базі: " + nodeDataType?.Value + "(" + nodeUdtName?.Value + ")" + " -> Новий тип: " + nodeDataTypeCreate?.Value + "). Можлива втрата даних!" + "\n");
                }

                XPathNodeIterator? nodeDirectoryNewTabularParts = nodeDirectoryExist?.Current?.Select("Control_TabularParts[IsExist = 'no']");
                if (nodeDirectoryNewTabularParts?.Count > 0)
                {
                    if (flag == false)
                    {
                        XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                        ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                        flag = true;
                    }
                }
                while (nodeDirectoryNewTabularParts!.MoveNext())
                {
                    XPathNavigator? nodeTabularPartsName = nodeDirectoryNewTabularParts?.Current?.SelectSingleNode("Name");
                    ApendLine("\t Нова таблична частина : " + nodeTabularPartsName?.Value + "\n");

                    InfoTableCreateFieldCreate(nodeDirectoryNewTabularParts?.Current, "\t\t");
                }

                XPathNodeIterator? nodeDirectoryTabularParts = nodeDirectoryExist?.Current?.Select("Control_TabularParts[IsExist = 'yes']");
                while (nodeDirectoryTabularParts!.MoveNext())
                {
                    bool flagTP = false;

                    XPathNodeIterator? nodeDirectoryTabularPartsNewField = nodeDirectoryTabularParts?.Current?.Select("Control_Field[IsExist = 'no']");
                    if (nodeDirectoryTabularPartsNewField?.Count > 0)
                    {
                        if (!flag)
                        {
                            XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                            XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                            ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                            flag = true;
                        }

                        if (!flagTP)
                        {
                            XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                            ApendLine("\t Таблична частина : " + nodeTabularPartsName?.Value + "\n");
                            flagTP = true;
                        }
                    }
                    while (nodeDirectoryTabularPartsNewField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryTabularPartsNewField?.Current?.SelectSingleNode("Name");
                        XPathNavigator? nodeConfType = nodeDirectoryTabularPartsNewField?.Current?.SelectSingleNode("FieldCreate/ConfType");

                        ApendLine("\t\t Нове Поле: " + nodeFieldName?.Value + "(Тип: " + nodeConfType?.Value + ")" + "\n");
                    }

                    XPathNodeIterator? nodeDirectoryTabularPartsField = nodeDirectoryTabularParts?.Current?.Select("Control_Field[IsExist = 'yes']/Type[Coincide = 'no']");
                    if (nodeDirectoryTabularPartsField?.Count > 0)
                    {
                        if (flag == false)
                        {
                            XPathNavigator? nodeName = nodeDirectoryExist?.Current?.SelectSingleNode("Name");
                            XPathNavigator? nodeType = nodeDirectoryExist?.Current?.SelectSingleNode("Type");
                            ApendLine(GetNameFromType(nodeType?.Value ?? "") + ": " + nodeName?.Value + "\n");
                            flag = true;
                        }

                        if (!flagTP)
                        {
                            XPathNavigator? nodeTabularPartsName = nodeDirectoryTabularParts?.Current?.SelectSingleNode("Name");
                            ApendLine("\t Таблична частина : " + nodeTabularPartsName?.Value + "\n");
                            flagTP = true;
                        }
                    }
                    while (nodeDirectoryTabularPartsField!.MoveNext())
                    {
                        XPathNavigator? nodeFieldName = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("../Name");
                        XPathNavigator? nodeDataType = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("DataType");
                        XPathNavigator? nodeDataTypeCreate = nodeDirectoryTabularPartsField?.Current?.SelectSingleNode("DataTypeCreate");

                        ApendLine("\t\t Поле: " + nodeFieldName?.Value + " -> змінений тип даних (Тип в базі: " + nodeDataType?.Value + " -> Новий тип: " + nodeDataTypeCreate?.Value + "). Можлива втрата даних!" + "\n");
                    }
                }
            }
        }
        else
        {
            ApendLine("Нова база даних");
        }

        ApendLine("\nВидалення копії файлу конфігурації");
        if (File.Exists(fullPathToCopyXmlFileConguratifion))
        {
            File.Delete(fullPathToCopyXmlFileConguratifion);
            ApendLine(" --> " + fullPathToCopyXmlFileConguratifion + "\n");
        }

        ApendLine("Видалення тимчасового файлу");
        if (File.Exists(Conf.PathToTempXmlFileConfiguration))
        {
            File.Delete(Conf.PathToTempXmlFileConfiguration);
            ApendLine(" --> " + Conf.PathToTempXmlFileConfiguration + "\n");
        }

        ButtonSensitive(true);

        Thread.Sleep(1000);
        ApendLine("\n\n\n");
    }

    async ValueTask SaveAnalizeAndCreateSQL()
    {
        ButtonSensitive(false);

        ClearListBoxTerminal();

        ApendLine("[ АНАЛІЗ ]\n");

        ApendLine("1. Створення копії файлу конфігурації");
        Conf.PathToCopyXmlFileConfiguration = Configuration.CreateCopyConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToCopyXmlFileConfiguration);
        ApendLine(" --> " + Conf.PathToCopyXmlFileConfiguration + "\n");

        string fullPathToCopyXmlFileConguratifion = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration);

        Conf.PathToTempXmlFileConfiguration = Configuration.GetTempPathToConfigurationFile(Conf.PathToXmlFileConfiguration, Conf.PathToTempXmlFileConfiguration);

        ApendLine("2. Збереження конфігурації у тимчасовий файл");
        Configuration.Save(Conf.PathToTempXmlFileConfiguration, Conf);
        ApendLine(" --> " + Conf.PathToTempXmlFileConfiguration + "\n");

        ApendLine("2. Отримання структури бази даних");
        ConfigurationInformationSchema informationSchema = await Program.Kernel.DataBase.SelectInformationSchema();
        Configuration.SaveInformationSchema(informationSchema,
             System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"));

        ApendLine("3. Створення загального файлу для порівняння");
        Configuration.CreateOneFileForComparison(
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "InformationSchema.xml"),
            Conf.PathToTempXmlFileConfiguration,
            fullPathToCopyXmlFileConguratifion,
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml")
        );

        ApendLine("4. Порівняння конфігурації та бази даних");
        Configuration.Comparison(
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAllData.xml"),
            System.IO.Path.Combine(PathToXsltTemplate, "xslt/Comparison.xslt"),
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml")
        );

        ApendLine("5. Створення команд SQL");
        Configuration.ComparisonAnalizeGeneration(
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "Comparison.xml"),
            System.IO.Path.Combine(PathToXsltTemplate, "xslt/ComparisonAnalize.xslt"),
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAnalize.xml"));

        ApendLine("6. Створення функцій SQL");
        Configuration.GeneratedFunc(
            Conf.PathToTempXmlFileConfiguration,
            System.IO.Path.Combine(PathToXsltTemplate, "xslt/GeneratedFunc.xslt"),
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "GeneratedFunc.xml"));

        if (informationSchema.Tables.Count > 0)
        {
            ApendLine("");

            XPathDocument xPathDoc = new XPathDocument(
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAnalize.xml")
            );
            XPathNavigator xPathDocNavigator = xPathDoc.CreateNavigator();

            XPathNodeIterator nodeInfo = xPathDocNavigator.Select("/root/info");
            if (nodeInfo.Count == 0)
            {
                ApendLine("Інформація відсутня!");
            }
            else
                while (nodeInfo!.MoveNext())
                {
                    ApendLine(nodeInfo?.Current?.Value ?? "");
                }

            ApendLine("\n[ Команди SQL ]\n");

            XPathNodeIterator nodeSQL = xPathDocNavigator.Select("/root/sql");
            if (nodeSQL.Count == 0)
            {
                ApendLine("Команди відсутні!");
            }
            else
            {
                while (nodeSQL!.MoveNext())
                {
                    ApendLine(" - " + nodeSQL?.Current?.Value);
                }

                ApendLine("\n Для внесення змін - натисніть \"Збереження змін. Крок 2\"\n");
            }
        }
        else
        {
            ApendLine("Нова база даних");
        }

        ButtonSensitive(true);

        Thread.Sleep(1000);
        ApendLine("\n\n\n");
    }

    async ValueTask ExecuteSQLAndGenerateCode()
    {
        ButtonSensitive(false);

        ClearListBoxTerminal();

        string pathToSqlCommandFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "ComparisonAnalize.xml");
        string pathToFuncSqlCommandFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, "GeneratedFunc.xml");

        if (File.Exists(pathToSqlCommandFile))
        {
            //Read SQL
            List<string> SqlList = Configuration.ListComparisonSql(pathToSqlCommandFile);

            ApendLine("[ Виконання SQL ]\n");

            if (SqlList.Count == 0)
            {
                ApendLine("Команди відсутні!");
            }
            else
            {
                //Execute
                foreach (string sqlText in SqlList)
                {
                    ApendLine(" --> " + sqlText);

                    try
                    {
                        await Program.Kernel.DataBase.ExecuteSQL(sqlText);
                    }
                    catch (Exception ex)
                    {
                        ApendLine("Помилка: " + ex.Message);
                    }
                }

                //
            }

            ApendLine("\nВидалення файлу команд " + pathToSqlCommandFile + "\n");
            File.Delete(pathToSqlCommandFile);
        }

        if (File.Exists(pathToFuncSqlCommandFile))
        {
            //Read SQL
            List<string> SqlList = Configuration.ListComparisonSql(pathToFuncSqlCommandFile);

            ApendLine("[ Створення функцій SQL ]\n");

            if (SqlList.Count != 0)
            {
                //Execute
                foreach (string sqlText in SqlList)
                {
                    ApendLine(" --> " + (sqlText.Length > 100 ? sqlText[..100] : sqlText));

                    try
                    {
                        await Program.Kernel.DataBase.ExecuteSQL(sqlText);
                    }
                    catch (Exception ex)
                    {
                        ApendLine("Помилка: " + ex.Message);
                    }
                }

                //
            }

            ApendLine("\nВидалення файлу команд " + pathToFuncSqlCommandFile + "\n");
            File.Delete(pathToFuncSqlCommandFile);
        }

        if (File.Exists(Conf.PathToTempXmlFileConfiguration))
        {
            ApendLine("Збереження конфігурації та видалення тимчасових файлів");
            Configuration.RewriteConfigurationFileFromTempFile(
                Conf.PathToXmlFileConfiguration,
                Conf.PathToTempXmlFileConfiguration,
                System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)!, Conf.PathToCopyXmlFileConfiguration)
            );
        }

        //Копіювання файлу конфігурації Confa.xml в каталог зкомпільованої програми
        if (!string.IsNullOrEmpty(entryCompileProgramPath.GetText()))
        {
            /*
            if (entryCompileProgramPath.Text.Substring(entryCompileProgramPath.Text.Length - 1, 1) != "/")
                entryCompileProgramPath.Text += "/";
            */

            string folderCompileProgramPath = System.IO.Path.GetDirectoryName(entryCompileProgramPath.GetText())!;

            if (System.IO.Directory.Exists(folderCompileProgramPath))
            {
                File.Copy(Conf.PathToXmlFileConfiguration,
                    System.IO.Path.Combine(folderCompileProgramPath, "Confa.xml"), true);

                ApendLine("\nСкопійований файл 'Confa.xml' в каталог " + folderCompileProgramPath);
            }
        }

        if (checkButtonIsGenerate.Active)
        {
            string folderGenerateCode = string.IsNullOrEmpty(entryGenerateCodePath.GetText()) ?
                   System.IO.Path.GetDirectoryName(Conf.PathToXmlFileConfiguration)! :
                   entryGenerateCodePath.GetText();

            if (System.IO.Directory.Exists(folderGenerateCode))
            {
                ApendLine("\n[ Генерування коду ]\n");
                ApendLine("Папка: " + folderGenerateCode + "\n");

                if (File.Exists(System.IO.Path.Combine(PathToXsltTemplate, "xslt/GeneratedCode.xslt")))
                {
                    Configuration.GeneratedCode(Conf.PathToXmlFileConfiguration,
                        System.IO.Path.Combine(PathToXsltTemplate, "xslt/GeneratedCode.xslt"),
                        System.IO.Path.Combine(folderGenerateCode, "GeneratedCode.cs"));

                    ApendLine("Файл 'GeneratedCode.cs' згенерований\n");
                }

                ApendLine($"Версія {Conf.GtkLibVersion}\n");

                if (Conf.GtkLibVersion == Configuration.GtkVersion.Gtk4)
                {
                    if (File.Exists(System.IO.Path.Combine(PathToXsltTemplate, "xslt/Gtk4.xslt")))
                    {
                        Configuration.GeneratedCode(Conf.PathToXmlFileConfiguration,
                            System.IO.Path.Combine(PathToXsltTemplate, "xslt/Gtk4.xslt"),
                            System.IO.Path.Combine(folderGenerateCode, "GeneratedCodeGtk.cs"));

                        ApendLine("Файл 'GeneratedCodeGtk.cs' згенерований\n");
                    }
                }
                else
                {
                    if (File.Exists(System.IO.Path.Combine(PathToXsltTemplate, "xslt/Gtk.xslt")))
                    {
                        Configuration.GeneratedCode(Conf.PathToXmlFileConfiguration,
                            System.IO.Path.Combine(PathToXsltTemplate, "xslt/Gtk.xslt"),
                            System.IO.Path.Combine(folderGenerateCode, "GeneratedCodeGtk.cs"));

                        ApendLine("Файл 'GeneratedCodeGtk.cs' згенерований\n");
                    }
                }

                ApendLine("\n[ Генерування форм ]\n");

                static string CreateFolder(string rootPath, string name)
                {
                    string folderPath = System.IO.Path.Combine(rootPath, name);
                    if (!System.IO.Directory.Exists(folderPath)) System.IO.Directory.CreateDirectory(folderPath);

                    return folderPath;
                }

                //Довідники
                {
                    string folderPathDirectories = CreateFolder(folderGenerateCode, "Довідники");

                    foreach (ConfigurationDirectories directory in Conf.Directories.Values)
                    {
                        string folderPathDirectory = "";
                        bool existAnyForm = false;

                        foreach (ConfigurationForms form in directory.Forms.Values)
                            if (form.Modified && !form.NotSaveToFile)
                            {
                                if (!existAnyForm)
                                {
                                    folderPathDirectory = CreateFolder(folderPathDirectories, directory.Name);
                                    existAnyForm = true;
                                }

                                string formPath = System.IO.Path.Combine(folderPathDirectory, form.Name + ".cs");

                                TextWriter tw = System.IO.File.CreateText(formPath);
                                tw.Write(form.GeneratedCode);
                                tw.Flush();
                                tw.Close();

                                form.Modified = false;
                            }

                        //Табличні частини
                        foreach (ConfigurationTablePart tabularPart in directory.TabularParts.Values)
                            foreach (ConfigurationForms form in tabularPart.Forms.Values)
                                if (form.Modified && !form.NotSaveToFile)
                                {
                                    if (!existAnyForm)
                                    {
                                        folderPathDirectory = CreateFolder(folderPathDirectories, directory.Name);
                                        existAnyForm = true;
                                    }

                                    string formPath = System.IO.Path.Combine(folderPathDirectory, directory.Name + "_" + form.Name + ".cs");

                                    TextWriter tw = System.IO.File.CreateText(formPath);
                                    tw.Write(form.GeneratedCode);
                                    tw.Flush();
                                    tw.Close();

                                    form.Modified = false;
                                }
                    }
                }

                //Документи
                {
                    string folderPathDocuments = CreateFolder(folderGenerateCode, "Документи");

                    foreach (ConfigurationDocuments document in Conf.Documents.Values)
                    {
                        string folderPathDocument = "";
                        bool existAnyForm = false;

                        foreach (ConfigurationForms form in document.Forms.Values)
                            if (form.Modified && !form.NotSaveToFile)
                            {
                                if (!existAnyForm)
                                {
                                    folderPathDocument = CreateFolder(folderPathDocuments, document.Name);
                                    existAnyForm = true;
                                }

                                string formPath = System.IO.Path.Combine(folderPathDocument, form.Name + ".cs");

                                TextWriter tw = System.IO.File.CreateText(formPath);
                                tw.Write(form.GeneratedCode);
                                tw.Flush();
                                tw.Close();

                                form.Modified = false;
                            }

                        //Табличні частини
                        foreach (ConfigurationTablePart tabularPart in document.TabularParts.Values)
                            foreach (ConfigurationForms form in tabularPart.Forms.Values)
                                if (form.Modified && !form.NotSaveToFile)
                                {
                                    if (!existAnyForm)
                                    {
                                        folderPathDocument = CreateFolder(folderPathDocuments, document.Name);
                                        existAnyForm = true;
                                    }

                                    string formPath = System.IO.Path.Combine(folderPathDocument, document.Name + "_" + form.Name + ".cs");

                                    TextWriter tw = System.IO.File.CreateText(formPath);
                                    tw.Write(form.GeneratedCode);
                                    tw.Flush();
                                    tw.Close();

                                    form.Modified = false;
                                }
                    }
                }

                //РегістриІнформації
                {
                    string folderPathRegInfos = CreateFolder(folderGenerateCode, "РегістриІнформації");

                    foreach (ConfigurationRegistersInformation regInfo in Conf.RegistersInformation.Values)
                    {
                        string folderPathRegInfo = "";
                        bool existAnyForm = false;

                        foreach (ConfigurationForms form in regInfo.Forms.Values)
                            if (form.Modified && !form.NotSaveToFile)
                            {
                                if (!existAnyForm)
                                {
                                    folderPathRegInfo = CreateFolder(folderPathRegInfos, regInfo.Name);
                                    existAnyForm = true;
                                }

                                string formPath = System.IO.Path.Combine(folderPathRegInfo, form.Name + ".cs");

                                TextWriter tw = System.IO.File.CreateText(formPath);
                                tw.Write(form.GeneratedCode);
                                tw.Flush();
                                tw.Close();

                                form.Modified = false;
                            }
                    }
                }

                //РегістриНакопичення
                {
                    string folderPathRegAccums = CreateFolder(folderGenerateCode, "РегістриНакопичення");

                    foreach (ConfigurationRegistersAccumulation regAccum in Conf.RegistersAccumulation.Values)
                    {
                        string folderPathRegAccum = "";
                        bool existAnyForm = false;

                        foreach (ConfigurationForms form in regAccum.Forms.Values)
                            if (form.Modified && !form.NotSaveToFile)
                            {
                                if (!existAnyForm)
                                {
                                    folderPathRegAccum = CreateFolder(folderPathRegAccums, regAccum.Name);
                                    existAnyForm = true;
                                }

                                string formPath = System.IO.Path.Combine(folderPathRegAccum, form.Name + ".cs");

                                TextWriter tw = System.IO.File.CreateText(formPath);
                                tw.Write(form.GeneratedCode);
                                tw.Flush();
                                tw.Close();

                                form.Modified = false;
                            }
                    }
                }
            }
            else
                ApendLine("\nError: Не знайдена папка " + folderGenerateCode + "\nКод не згенерований\n");
        }

        ApendLine("\nГОТОВО!");

        ButtonSensitive(true);

        Thread.Sleep(1000);
        ApendLine("\n\n\n");
    }
}

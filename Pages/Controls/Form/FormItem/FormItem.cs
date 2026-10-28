/*
Copyright (C) 2019-2026 TARAKHOMYN YURIY IVANOVYCH
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

/*
Автор:    Тарахомин Юрій Іванович
Адреса:   Україна, м. Львів
Сайт:     accounting.org.ua
*/

using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using System.Xml;

namespace Configurator;

/// <summary>
/// Форма
/// </summary>
[Subclass<Box>("FormItem")]
[Template<AssemblyResource>("FormItem.ui")]
public partial class FormItem
{
    [Connect("dropdown_type_form")] DropDownControl dropdownTypeForm;

    [Connect("stack_main")] Stack stackMain;
    [Connect("box_element_settings")] Box boxElementSettings;
    [Connect("box_list_settings")] Box boxListSettings;

    //Код
    [Connect("button_generate_code")] Button buttonGenerateCode;
    [Connect("codeview_code")] CodeView codeviewCode;

    //Налаштування списку
    [Connect("dropdown_tabularlist")] DropDownControl dropdownTabularlist;

    //Налаштування елементів
    [Connect("form_element_settings")] FormElementSettings formElementSettings;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationForms ConfForm { get; set; } = new();
    ConfiguratorItemOwner? Owner { get; set; } = null;

    Dictionary<string, ConfigurationField> Fields { get; set; } = []; //Поля
    Dictionary<string, ConfigurationTablePart> TabularParts { get; set; } = []; //Табличні частини

    partial void Initialize()
    {
        dropdownTypeForm.AllowEmpty = false;
        dropdownTypeForm.Fill(ConfigurationForms.TypeForms_Dict());
        dropdownTypeForm.OnСhanged = () =>
        {
            ConfForm.Type = Enum.Parse<ConfigurationForms.TypeForms>(dropdownTypeForm.Value);

            StackPage pageListSettings = stackMain.GetPage(boxListSettings);
            StackPage pageElementSettings = stackMain.GetPage(boxElementSettings);

            if (IsList())
            {
                pageElementSettings.Visible = !(pageListSettings.Visible = true);

                //Отримую із власника список табличних списків
                Dictionary<string, ConfigurationTabularList> tabularList = (Owner?.Group, Owner?.Obj) switch
                {
                    (ConfiguratorItemOwnerType.Directory, ConfigurationDirectories x) => x.TabularList,
                    (ConfiguratorItemOwnerType.Document, ConfigurationDocuments x) => x.TabularList,
                    (ConfiguratorItemOwnerType.TablePart, ConfigurationTablePart x) => x.TabularList,
                    (ConfiguratorItemOwnerType.RegisterInformation, ConfigurationRegistersInformation x) => x.TabularList,
                    (ConfiguratorItemOwnerType.RegisterAccumulation, ConfigurationRegistersAccumulation x) => x.TabularList,
                    _ => []
                };

                dropdownTabularlist.AllowEmpty = false;
                dropdownTabularlist.Fill(tabularList.ToDictionary(x => x.Key, x => x.Value.Name));
                dropdownTabularlist.Value = ConfForm.TabularList;
                dropdownTabularlist.OnСhanged = () => { };
            }
            else if (ConfForm.Type == ConfigurationForms.TypeForms.Element || ConfForm.Type == ConfigurationForms.TypeForms.TablePart)
            {
                pageListSettings.Visible = !(pageElementSettings.Visible = true);

                //Отримую із власника поля і табличні частини
                (Fields, TabularParts) = (Owner?.Group, Owner?.Obj) switch
                {
                    (ConfiguratorItemOwnerType.Directory, ConfigurationDirectories x) => (x.Fields, x.TabularParts),
                    (ConfiguratorItemOwnerType.Document, ConfigurationDocuments x) => (x.Fields, x.TabularParts),
                    (ConfiguratorItemOwnerType.TablePart, ConfigurationTablePart x) => (x.Fields, []),
                    _ => ([], [])
                };

                formElementSettings.SetValue(Fields, TabularParts, ConfForm);
            }
            else
                pageListSettings.Visible = pageElementSettings.Visible = false;
        };

        //Код
        codeviewCode.SetLanguage("c-sharp");

        buttonGenerateCode.OnClicked += async (_, _) =>
        {
            if (Owner == null)
            {
                await Message.Info(Program.BasicForm, "Помилка", "Не вказаний власник форми!");
                return;
            }

            ConfiguratorItemOwnerType[] allowOwner = [
                ConfiguratorItemOwnerType.Directory, ConfiguratorItemOwnerType.Document,
                ConfiguratorItemOwnerType.RegisterInformation, ConfiguratorItemOwnerType.RegisterAccumulation,
                ConfiguratorItemOwnerType.TablePart
            ];

            if (!allowOwner.Contains(Owner.Group))
            {
                await Message.Info(Program.BasicForm, "Помилка", $"Невірно вказаний тип власника форми. Має бути {string.Join(", ", allowOwner)}");
                return;
            }

            GetValue();
            GenerateCode();
        };
    }

    /*void A(ConfiguratorItemOwner? o)
    {
        Console.WriteLine("o is null " + o is null);
        Console.WriteLine("o group " + o?.Group);
        Console.WriteLine("o obj " + o?.Obj);

        if (o?.Parent != null)
            A(o?.Parent);
    }*/

    public void SetValue(ConfigurationForms confForm, ConfiguratorItemOwner? owner = null)
    {
        ConfForm = confForm;
        Owner = owner;

        dropdownTypeForm.Value = ConfForm.Type.ToString();
        codeviewCode.Value = ConfForm.GeneratedCode;
    }

    public void GetValue()
    {
        ConfForm.Modified = true;
        ConfForm.NotSaveToFile = false;
        ConfForm.GeneratedCode = codeviewCode.Value;

        if (IsList())
        {
            ConfForm.TabularList = dropdownTabularlist.Value;
        }

        if (ConfForm.Type == ConfigurationForms.TypeForms.Element || ConfForm.Type == ConfigurationForms.TypeForms.TablePart)
        {
            formElementSettings.GetValue();
        }
    }

    public static FormItem New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();
        CodeView.GetGType();
        FormElementSettings.GetGType();
        return NewWithProperties([]);
    }

    bool IsList() => ConfForm.Type == ConfigurationForms.TypeForms.List || ConfForm.Type == ConfigurationForms.TypeForms.ListSmall ||
        ConfForm.Type == ConfigurationForms.TypeForms.ListSmallSelect || ConfForm.Type == ConfigurationForms.TypeForms.ListAndTree;

    void GenerateCode()
    {
        if (Owner == null) return;

        XmlDocument xmlDoc = new();
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", ""));

        XmlElement root = xmlDoc.CreateElement("root");
        xmlDoc.AppendChild(root);

        XmlElement nodeOwner = xmlDoc.CreateElement(Owner.Group.ToString());
        root.AppendChild(nodeOwner);

        nodeOwner.AppendChild(xmlDoc.CreateNode("Alias", Configuration.GetShortGuid()));

        //Довідник
        if (Owner.Group == ConfiguratorItemOwnerType.Directory && Owner.Obj is ConfigurationDirectories directory)
        {
            Configuration.SaveTriggerFunctions(directory.TriggerFunctions, xmlDoc, nodeOwner);

            nodeOwner.AppendChild(xmlDoc.CreateNode("Name", directory.Name));
            nodeOwner.AppendChild(xmlDoc.CreateNode("AutomaticNumeration", directory.AutomaticNumeration ? "1" : "0")); //Нумерація
            nodeOwner.AppendChild(xmlDoc.CreateNode("Type", directory.TypeDirectory.ToString())); //Тип довідника

            if (directory.TypeDirectory == ConfigurationDirectories.TypeDirectories.HierarchyInAnotherDirectory)
            {
                //Перевірка вказівника
                string pointerFolders = directory.PointerFolders_HierarchyInAnotherDirectory;
                var (Result, _, PointerType) = Configuration.PointerParse(pointerFolders, out _);
                if (Result)
                {
                    nodeOwner.AppendChild(xmlDoc.CreateNode("PointerFolders", PointerType)); //Назва довідника
                    nodeOwner.AppendChild(xmlDoc.CreateNode("FieldFolder", directory.Fields.Values.SingleOrDefault(x => x.Type == "pointer" && x.Pointer == pointerFolders)?.Name)); //Пошук поля Папки
                }
            }
            else if (directory.TypeDirectory == ConfigurationDirectories.TypeDirectories.Hierarchical)
            {
                nodeOwner.AppendChild(xmlDoc.CreateNode("ParentField", directory.ParentField_Hierarchical));
                nodeOwner.AppendChild(xmlDoc.CreateNode("AllowedContent", directory.AllowedContent_Hierarchical.ToString()));
            }

            nodeOwner.AppendChild(xmlDoc.CreateNode("DirectoryOwner", directory.DirectoryOwner_Subordination));  //Довідник власник
            nodeOwner.AppendChild(xmlDoc.CreateNode("PointerFieldOwner", directory.PointerFieldOwner_Subordination)); //Поле вказівник на довідник власник
        }

        //Документ
        if (Owner.Group == ConfiguratorItemOwnerType.Document && Owner.Obj is ConfigurationDocuments document)
        {
            Configuration.SaveTriggerFunctions(document.TriggerFunctions, xmlDoc, nodeOwner);
            Configuration.SaveSpendFunctions(document.SpendFunctions, xmlDoc, nodeOwner);

            nodeOwner.AppendChild(xmlDoc.CreateNode("Name", document.Name));
            nodeOwner.AppendChild(xmlDoc.CreateNode("AutomaticNumeration", document.AutomaticNumeration ? "1" : "0")); //Нумерація
        }

        //Регістр інформації
        if (Owner.Group == ConfiguratorItemOwnerType.RegisterInformation && Owner.Obj is ConfigurationRegistersInformation regInfo)
        {
            nodeOwner.AppendChild(xmlDoc.CreateNode("Name", regInfo.Name));
        }

        //Регістр накопичення
        if (Owner.Group == ConfiguratorItemOwnerType.RegisterAccumulation && Owner.Obj is ConfigurationRegistersAccumulation regAccum)
        {
            nodeOwner.AppendChild(xmlDoc.CreateNode("Name", regAccum.Name));
            nodeOwner.AppendChild(xmlDoc.CreateNode("Type", regAccum.TypeRegisters.ToString()));
        }

        //Таблична частина
        if (Owner.Group == ConfiguratorItemOwnerType.TablePart && Owner.Obj is ConfigurationTablePart tablePart)
        {
            nodeOwner.AppendChild(xmlDoc.CreateNode("Name", tablePart.Name));

            if (ConfForm.Type == ConfigurationForms.TypeForms.TablePart || ConfForm.Type == ConfigurationForms.TypeForms.TriggersTablePart)
            {
                if (ConfForm.Type == ConfigurationForms.TypeForms.TriggersTablePart)
                    Configuration.SaveTriggerTablePartFunctions(tablePart.TriggerFunctions, xmlDoc, nodeOwner);

                var parent = Owner.Parent;
                if (parent != null)
                {
                    string ownerName = (parent.Group, parent.Obj) switch
                    {
                        (ConfiguratorItemOwnerType.Directory, ConfigurationDirectories x) => x.Name,
                        (ConfiguratorItemOwnerType.Document, ConfigurationDocuments x) => x.Name,
                        (ConfiguratorItemOwnerType.RegisterAccumulation, ConfigurationRegistersAccumulation x) => x.Name,
                        _ => ""
                    };

                    nodeOwner.AppendChild(xmlDoc.CreateNode("OwnerExist", "1"));
                    nodeOwner.AppendChild(xmlDoc.CreateNode("OwnerType", parent.Group.ToString()));
                    nodeOwner.AppendChild(xmlDoc.CreateNode("OwnerName", ownerName));
                    nodeOwner.AppendChild(xmlDoc.CreateNode("OwnerBlockName", ""));
                }

                Configuration.SaveFormElementField(Conf, Fields, ConfForm.ElementFields, xmlDoc, nodeOwner);
            }
        }

        if (IsList())
        {
            nodeOwner.AppendChild(xmlDoc.CreateNode("TabularList", ConfForm.TabularList));
        }
        else if (ConfForm.Type == ConfigurationForms.TypeForms.Element)
        {
            Configuration.SaveFormElementField(Conf, Fields, ConfForm.ElementFields, xmlDoc, nodeOwner);
            Configuration.SaveFormElementTablePart(TabularParts, ConfForm.ElementTableParts, xmlDoc, nodeOwner);
        }
        else if (ConfForm.Type == ConfigurationForms.TypeForms.Triggers || ConfForm.Type == ConfigurationForms.TypeForms.Function)
        {
            Configuration.SaveFields(Conf, Fields, xmlDoc, nodeOwner, Owner.Group.ToString());
            Configuration.SaveTabularParts(Conf, TabularParts, xmlDoc, nodeOwner);
        }

        /*else if (ConfForm.Type == ConfigurationForms.TypeForms.Report)
        {
            if (ParentType == "TablePart")
            {
                XmlElement nodeOwnerName = xmlDoc.CreateElement("OwnerName");
                nodeOwnerName.InnerText = Owner.Name;
                nodeParentType.AppendChild(nodeOwnerName);

                XmlElement nodeOwnerBlockName = xmlDoc.CreateElement("OwnerBlockName");
                nodeOwnerBlockName.InnerText = Owner.BlockName;
                nodeParentType.AppendChild(nodeOwnerBlockName);
            }

            Configuration.SaveFormElementField(Conf, Fields, ConfForm.ElementFields, xmlDoc, nodeOwner);
        }*/

        codeviewCode.Value = Configuration.Transform
        (
            xmlDoc, Path.Combine(AppContext.BaseDirectory, "xslt", $"Constructor{Owner.Group}4.xslt"),
            new Dictionary<string, object>
            {
                { "File",  ConfForm.Type.ToString() },
                { "NameSpaceGeneratedCode", Conf.NameSpaceGeneratedCode },
                { "NameSpace", Conf.NameSpace }
            }
        ) ?? "";
    }
}
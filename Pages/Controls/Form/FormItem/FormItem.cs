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

namespace Configurator;

/// <summary>
/// Форма
/// </summary>
[Subclass<Box>("FormItem")]
[Template<AssemblyResource>("FormItem.ui")]
public partial class FormItem
{
    [Connect("label_type_form")] Label labelTypeForm;

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

    ConfigurationForms ConfForm { get; set; } = new();
    ConfiguratorItemOwner? Owner { get; set; } = null;

    partial void Initialize()
    {
        //Код
        codeviewCode.SetLanguage("c-sharp");

        buttonGenerateCode.OnClicked += (_, _) => { };
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

    void VisiblePages()
    {
        if (IsList())
        {
            StackPage page = stackMain.GetPage(boxListSettings);
            page.Visible = true;
        }
        else if (ConfForm.Type == ConfigurationForms.TypeForms.Element)
        {
            StackPage page = stackMain.GetPage(boxElementSettings);
            page.Visible = true;
        }
    }

    public void SetValue(ConfigurationForms confForm, ConfiguratorItemOwner? owner = null)
    {
        ConfForm = confForm;
        Owner = owner;

        VisiblePages();

        labelTypeForm.SetMarkup($"<b>{ConfigurationForms.TypeForms_Dict()[ConfForm.Type.ToString()]}</b>");
        codeviewCode.Value = ConfForm.GeneratedCode;

        if (IsList())
        {
            //Отримую із власника список табличних списків
            Dictionary<string, ConfigurationTabularList> tabularList = (Owner?.Group, Owner?.Obj) switch
            {
                (ConfiguratorItemOwnerType.Directory, ConfigurationDirectories x) => x.TabularList,
                (ConfiguratorItemOwnerType.Document, ConfigurationDocuments x) => x.TabularList,
                //(ConfiguratorItemOwnerType.TablePart, ConfigurationTablePart x) => x.TabularList,
                (ConfiguratorItemOwnerType.RegisterInformation, ConfigurationRegistersInformation x) => x.TabularList,
                (ConfiguratorItemOwnerType.RegisterAccumulation, ConfigurationRegistersAccumulation x) => x.TabularList,
                _ => []
            };

            dropdownTabularlist.AllowEmpty = false;
            dropdownTabularlist.Fill(tabularList.ToDictionary(x => x.Key, x => x.Value.Name));
            dropdownTabularlist.Value = ConfForm.TabularList;
            dropdownTabularlist.OnСhanged = () => { };
        }

        if (ConfForm.Type == ConfigurationForms.TypeForms.Element)
        {
            var (fields, tabularParts) = (Owner?.Group, Owner?.Obj) switch
            {
                (ConfiguratorItemOwnerType.Directory, ConfigurationDirectories x) => (x.Fields, x.TabularParts),
                (ConfiguratorItemOwnerType.Document, ConfigurationDocuments x) => (x.Fields, x.TabularParts),
                _ => ([], [])
            };

            formElementSettings.SetValue(fields, tabularParts, ConfForm);
        }
    }

    public void GetValue()
    {
        if (IsList())
        {
            ConfForm.TabularList = dropdownTabularlist.Value;
        }

        if (ConfForm.Type == ConfigurationForms.TypeForms.Element)
        {
            formElementSettings.GetValue();
        }
    }
}
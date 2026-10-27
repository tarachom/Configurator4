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
    [Connect("dropdown_type_form")] DropDownControl dropdownTypeForm;

    [Connect("stack_main")] Stack stackMain;

    [Connect("button_generate_code")] Button buttonGenerateCode;
    [Connect("codeview_code")] CodeView codeviewCode;

    [Connect("box_element_settings")] Box boxElementSettings;
    [Connect("box_list_settings")] Box boxListSettings;

    ConfigurationForms ConfForm { get; set; } = new();

    partial void Initialize()
    {
        dropdownTypeForm.AllowEmpty = false;
        dropdownTypeForm.Fill(ConfigurationForms.TypeForms_Dict());
        dropdownTypeForm.OnСhanged = () =>
        {
            ConfigurationForms.TypeForms type = Enum.Parse<ConfigurationForms.TypeForms>(dropdownTypeForm.Value);
            if (type == ConfigurationForms.TypeForms.Element)
            {
                StackPage page = stackMain.GetPage(boxElementSettings);
                page.Visible = true;
            }
            else if (type == ConfigurationForms.TypeForms.List)
            {
                StackPage page = stackMain.GetPage(boxListSettings);
                page.Visible = true;
            }
        };

        buttonGenerateCode.OnClicked += (_, _) => { };

        //Код
        codeviewCode.SetLanguage("c-sharp");
    }

    public static FormItem New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();
        CodeView.GetGType();
        return NewWithProperties([]);
    }

    public void SetValue(ConfigurationForms confForm)
    {
        ConfForm = confForm;

        dropdownTypeForm.Value = ConfForm.Type.ToString();
        codeviewCode.Value = ConfForm.GeneratedCode;
    }
}
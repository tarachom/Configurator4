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

using AccountingSoftware;
using InterfaceGtkLib;

namespace Configurator;

/// <summary>
/// Переоприділення форми вибору бази
/// </summary>
[GObject.Subclass<InterfaceGtk4.FormConfigurationSelection>]
partial class FormConfigurationSelection : InterfaceGtk4.FormConfigurationSelection
{
    public override TypeForm TypeOpenForm { get; set; } = TypeForm.Configurator;

    public static new FormConfigurationSelection New()
    {
        FormConfigurationSelection form = NewWithProperties([]);
        form.Application = Program.BasicApp;
        form.ConfiguratorKernel = Program.Kernel;

        return form;
    }

    public override async Task<bool> OpenConfigurator(ConfigurationParam? openConfigurationParam)
    {
        FormConfigurator form = FormConfigurator.NewConfiguratorStart(openConfigurationParam);
        form.Show();

        Program.BasicForm = form;

        //Відкрити перші сторінки
        await form.OpenFirstPages();

        return true;
    }
}
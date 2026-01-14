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
class FormConfigurationSelection : InterfaceGtk4.FormConfigurationSelection
{
    public FormConfigurationSelection() : base(Program.BasicApp, null, Program.Kernel, TypeForm.Configurator) { }

    public override async ValueTask<bool> OpenConfigurator(ConfigurationParam? openConfigurationParam)
    {
        FormConfigurator form = new() { OpenConfigurationParam = openConfigurationParam };
        form.SetStatusBar();
        form.Show();

        //Відкрити перші сторінки
        await form.OpenFirstPages();

        return true;
    }
}
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
using InterfaceGtk4;
using InterfaceGtkLib;

namespace Configurator;

/// <summary>
/// Стартова сторінка
/// </summary>
[GObject.Subclass<Form>]
partial class PageHome : Form
{
    partial void Initialize()
    {
        {
            ActiveUsersView view = ActiveUsersView.New(Program.Kernel, 800, 200);

            Box hBox = New(Orientation.Horizontal, 0);
            hBox.MarginBottom = 10;
            hBox.Append(view);
            Append(hBox);
        }

        {
            AiChatView view = AiChatView.New(800);

            Box hBox = New(Orientation.Horizontal, 0);
            hBox.MarginBottom = 10;
            hBox.Append(view);
            Append(hBox);
        }

        /*{
            FormElementSettings f = FormElementSettings.New();
            Box hBox = New(Orientation.Horizontal, 0);
            hBox.Hexpand = hBox.Vexpand = true;
            hBox.MarginBottom = 10;
            hBox.Append(f);
            Append(hBox);
        }*/
    }

    public static PageHome New()
    {
        PageHome w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    public async ValueTask SetValue()
    {
        //Автоматичний запуск AI клієнта
        GlobalConfigurationParam? globalConfigurationParam = Program.BasicForm?.GlobalConfigurationParam;
        if (globalConfigurationParam != null && globalConfigurationParam.AIStartOnRun)
            try
            {
                bool result = FunctionForAI.CreateClient(globalConfigurationParam.AIKey, globalConfigurationParam.AIModel);
            }
            catch (Exception ex)
            {
                await Message.Info(Program.BasicForm, "Помилка", "Під час запуску AI клієнта виникла помилка:\n\n" + ex.Message);
            }
    }
}

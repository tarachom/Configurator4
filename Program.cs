/*
Copyright (C) 2019-2025 TARAKHOMYN YURIY IVANOVYCH
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
using Gdk;
using AccountingSoftware;

namespace Configurator;

class Program
{
    public static readonly Application BasicApp = Application.New("ua.org.accounting.configurator", Gio.ApplicationFlags.FlagsNone);

    /// <summary>
    /// Основна форма
    /// </summary>
    public static FormConfigurator? BasicForm { get; set; }

    public static Kernel Kernel { get; set; } = new Kernel();

    static void Main()
    {
        BasicApp.OnActivate += (app, args) => new FormConfigurationSelection().Show();
        BasicApp.OnShutdown += (app, args) => { };

        Display? display = Display.GetDefault();
        if (display != null)
        {
            //Icon
            IconTheme iconTheme = IconTheme.GetForDisplay(display);
            iconTheme.AddSearchPath(Path.Combine(AppContext.BaseDirectory, "images"));

            //Css
            string styleDefaultFile = Path.Combine(AppContext.BaseDirectory, "StyleCss/Default.css");
            if (File.Exists(styleDefaultFile))
            {
                CssProvider provider = CssProvider.New();
                provider.LoadFromPath(styleDefaultFile);
                StyleContext.AddProviderForDisplay(display, provider, 800);
            }
        }

        BasicApp.RunWithSynchronizationContext(null);
    }
}

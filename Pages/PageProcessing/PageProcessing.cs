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
using InterfaceGtkLib;
using InterfaceGtk4;

namespace Configurator;

/// <summary>
/// Обробки
/// </summary>
[Subclass<Box>("PageProcessing")]
[Template<AssemblyResource>("PageProcessing.ui")]
public partial class PageProcessing
{
    Configuration Conf { get; } = Program.Kernel.Conf;

    #region Fields

    [Connect("button_work")] Button buttonWork;

    #endregion

    public static PageProcessing New() => NewWithProperties([]);

    partial void Initialize()
    {
        buttonWork.OnClicked += (_, _) => Work();
    }

    Task Work()
    {
        return Task.CompletedTask;
    }

    public void SetValue()
    {

    }
}
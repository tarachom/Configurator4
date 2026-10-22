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

using GObject;
using AccountingSoftware;
using InterfaceGtk4;

namespace Configurator;

/// <summary>
/// Автоматична нумерація для довідника
/// </summary>
[Subclass<AutomaticNumbering>()]
public partial class DirectoryAutomaticNumbering : AutomaticNumbering
{
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    public static DirectoryAutomaticNumbering New() => NewWithProperties([]);

    partial void Initialize()
    {
        buttonCreate.OnClicked += (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDirectory.Name) || !Conf.Directories.ContainsKey(ConfDirectory.Name))
            {
                Message.Error(Program.BasicForm, "Назва довідника не задана або довідник не збережений! Довідник потрібно спочатку зберегти!");
                return;
            }

            //Створення константи
            CreateConst(new("НумераціяДовідників", "Нумерація довідників"), ConfDirectory.Name);

            checkAutomaticNumbering.Active = true;
        };
    }

    public void SetValue(ConfigurationDirectories confDirectory)
    {
        ConfDirectory = confDirectory;
        checkAutomaticNumbering.Active = ConfDirectory.AutomaticNumeration;
    }

    public void GetValue()
    {
        ConfDirectory.AutomaticNumeration = checkAutomaticNumbering.Active;
    }
}
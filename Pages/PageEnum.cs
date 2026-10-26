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
using AccountingSoftware;

namespace Configurator;

/// <summary>
/// Сторінка перелічення
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageEnum))]
partial class PageEnum : FormPageConfigurator
{
    public ConfigurationEnums ConfEnums { get; set; } = new();
    public Dictionary<string, ConfigurationEnums> Enums = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    EnumsDataTree dataTree = EnumsDataTree.New();

    partial void Initialize()
    {
        PageName = "Перелічення:";
        basicFields.HideTableOrColumn();
    }

    public static PageEnum New()
    {
        PageEnum w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        basicFields.ItemName = ConfEnums.Name;
        basicFields.FullName = ConfEnums.FullName;
        basicFields.Desc = ConfEnums.Desc;

        dataTree.SetValue(ConfEnums);
    }

    protected override async Task GetValue()
    {
        ConfEnums.Name = basicFields.ItemName;
        ConfEnums.FullName = basicFields.FullName;
        ConfEnums.Desc = basicFields.Desc;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfEnums.Name, [.. Enums.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Enums.Remove(ConfEnums.Name);
        }
        else
            return false;

        await GetValue();
        Enums.Add(ConfEnums.Name, ConfEnums);

        Caption = ConfEnums.Name;
        IsNew = false;

        return true;
    }
}

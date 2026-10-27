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
[GObject.Subclass<FormPageConfigurator>(nameof(PageEnumField))]
partial class PageEnumField : FormPageConfigurator
{
    public ConfigurationEnumField ConfEnumField { get; set; } = new();
    public Dictionary<string, ConfigurationEnumField> Fields { get; set; } = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    EnumFieldValue enumFieldValue = EnumFieldValue.New();

    partial void Initialize()
    {
        PageName = "Поле перелічення:";
        basicFields.HideTableOrColumn();
    }

    public static PageEnumField New()
    {
        PageEnumField w = NewWithProperties([]);
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
        vBox.Append(enumFieldValue);
    }

    public override async Task AssignValue()
    {
        basicFields.ItemName = ConfEnumField.Name;
        basicFields.FullName = ConfEnumField.FullName;
        basicFields.Desc = ConfEnumField.Desc;

        enumFieldValue.SetValue(ConfEnumField.Value);
    }

    protected override async Task GetValue()
    {
        ConfEnumField.Name = basicFields.ItemName;
        ConfEnumField.FullName = basicFields.FullName;
        ConfEnumField.Desc = basicFields.Desc;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfEnumField.Name, [.. Fields.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Fields.Remove(ConfEnumField.Name);
        }
        else
            return false;

        await GetValue();
        Fields.Add(ConfEnumField.Name, ConfEnumField);

        Caption = ConfEnumField.Name;
        IsNew = false;

        return true;
    }
}

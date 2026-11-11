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
/// Сторінка поля журналу
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageJournalField))]
partial class PageJournalField : FormPageConfigurator
{
    public ConfigurationJournalField ConfField { get; set; } = new();
    public Dictionary<string, ConfigurationJournalField> Fields = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();

    partial void Initialize()
    {
        PageName = "Поле журналу:";
        basicFields.HideTableOrColumn();
    }

    public static PageJournalField New()
    {
        PageJournalField w = NewWithProperties([]);
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

    }

    public override async Task AssignValue()
    {
        //if (IsNew)
        // _ = Function.FillNewConstant(Conf, ConfConstant);

        basicFields.ItemName = ConfField.Name;
        basicFields.FullName = ConfField.FullName;
        basicFields.Desc = ConfField.Desc;
        basicFields.LatinName = ConfField.LatinName;
    }

    protected override async Task GetValue()
    {
        ConfField.Name = basicFields.ItemName;
        ConfField.FullName = basicFields.FullName;
        ConfField.Desc = basicFields.Desc;
        ConfField.LatinName = basicFields.LatinName;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfField.Name, [.. Fields.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Fields.Remove(ConfField.Name);
        }
        else
            return false;

        await GetValue();
        Fields.Add(ConfField.Name, ConfField);

        Caption = ConfField.Name;
        IsNew = false;

        return true;
    }
}

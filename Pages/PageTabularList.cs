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
/// Сторінка табличний список
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageTabularList))]
partial class PageTabularList : FormPageConfigurator
{
    public ConfigurationTabularList ConfTabularList { get; set; } = new();
    public Dictionary<string, ConfigurationTabularList> TabularLists = [];
    public Dictionary<string, ConfigurationField> Fields = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    DataTabularList dataTabularList = DataTabularList.New();

    partial void Initialize()
    {
        PageName = "Табличний список:";
        basicFields.HideTableOrColumn();
    }

    public static PageTabularList New()
    {
        PageTabularList w = NewWithProperties([]);
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
        //Налаштування
        vBox.Append(dataTabularList);
    }

    public override async Task AssignValue()
    {
        basicFields.ItemName = ConfTabularList.Name;
        basicFields.FullName = ConfTabularList.FullName;
        basicFields.Desc = ConfTabularList.Desc;

        dataTabularList.SetValue(IsNew, Fields, ConfTabularList);
    }

    protected override async Task GetValue()
    {
        ConfTabularList.Name = basicFields.ItemName;
        ConfTabularList.FullName = basicFields.FullName;
        ConfTabularList.Desc = basicFields.Desc;

        dataTabularList.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfTabularList.Name, [.. TabularLists.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                TabularLists.Remove(ConfTabularList.Name);
        }
        else
            return false;

        await GetValue();
        TabularLists.Add(ConfTabularList.Name, ConfTabularList);

        Caption = ConfTabularList.Name;
        IsNew = false;

        return true;
    }
}

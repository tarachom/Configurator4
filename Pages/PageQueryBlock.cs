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
/// Сторінка блоків запитів (тригери)
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageQueryBlock))]
partial class PageQueryBlock : FormPageConfigurator
{
    public ConfigurationQueryBlock ConfQueryBlock { get; set; } = new();
    public Dictionary<string, ConfigurationQueryBlock> QueryBlocks { get; set; } = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    QueryBlockList queryList = QueryBlockList.New();

    partial void Initialize()
    {
        PageName = "Тригер:";
        basicFields.HideTableOrColumn();
    }

    public static PageQueryBlock New()
    {
        PageQueryBlock w = NewWithProperties([]);
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
        //Список запитів
        vBox.Append(queryList);
    }

    public override async Task AssignValue()
    {
        basicFields.ItemName = ConfQueryBlock.Name;
        basicFields.FullName = ConfQueryBlock.FullName;
        basicFields.Desc = ConfQueryBlock.Desc;

        queryList.SetValue(ConfQueryBlock);
    }

    protected override async Task GetValue()
    {
        ConfQueryBlock.Name = basicFields.ItemName;
        ConfQueryBlock.FullName = basicFields.FullName;
        ConfQueryBlock.Desc = basicFields.Desc;

        queryList.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfQueryBlock.Name, [.. QueryBlocks.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                QueryBlocks.Remove(ConfQueryBlock.Name);
        }
        else
            return false;

        await GetValue();
        QueryBlocks.Add(ConfQueryBlock.Name, ConfQueryBlock);

        Caption = ConfQueryBlock.Name;
        IsNew = false;

        return true;
    }
}

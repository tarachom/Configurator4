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
[GObject.Subclass<FormPageConfigurator>(nameof(PageConstantBlock))]
partial class PageConstantBlock : FormPageConfigurator
{
    public ConfigurationConstantsBlock ConfBlock { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    ConstantsBlockDataTree dataTree = ConstantsBlockDataTree.New();

    partial void Initialize()
    {
        PageName = "Блок констант:";
        basicFields.HideTableOrColumn();
    }

    public static PageConstantBlock New()
    {
        PageConstantBlock w = NewWithProperties([]);
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
        basicFields.ItemName = ConfBlock.Name;
        basicFields.FullName = ConfBlock.FullName;
        basicFields.Desc = ConfBlock.Desc;
        basicFields.LatinName = ConfBlock.LatinName;

        dataTree.SetValue(ConfBlock);
    }

    protected override async Task GetValue()
    {
        ConfBlock.Name = basicFields.ItemName;
        ConfBlock.FullName = basicFields.FullName;
        ConfBlock.Desc = basicFields.Desc;
        ConfBlock.LatinName = basicFields.LatinName;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfBlock.Name, [.. Conf.ConstantsBlock.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.ConstantsBlock.Remove(ConfBlock.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendConstantsBlock(ConfBlock);

        Caption = ConfBlock.Name;
        IsNew = false;

        return true;
    }
}

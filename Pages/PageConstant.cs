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
[GObject.Subclass<FormPageConfigurator>(nameof(PageConstant))]
partial class PageConstant : FormPageConfigurator
{
    public ConfigurationConstants ConfConstant { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    ConstantField field = ConstantField.New();
    ConstantsDataTree dataTree = ConstantsDataTree.New();

    partial void Initialize()
    {
        PageName = "Константа:";
        basicFields.HideTableOrColumn();
    }

    public static PageConstant New()
    {
        PageConstant w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Тип даних
        vBox.Append(field);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        if (IsNew)
            _ = Function.FillNewConstant(Conf, ConfConstant);

        basicFields.ItemName = ConfConstant.Name;
        basicFields.FullName = ConfConstant.FullName;
        basicFields.Desc = ConfConstant.Desc;
        basicFields.LatinName = ConfConstant.LatinName;

        field.SetValue(ConfConstant);
        dataTree.SetValue(ConfConstant);
    }

    protected override async Task GetValue()
    {
        ConfConstant.Name = basicFields.ItemName;
        ConfConstant.FullName = basicFields.FullName;
        ConfConstant.Desc = basicFields.Desc;
        ConfConstant.LatinName = basicFields.LatinName;

        field.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfConstant.Name, [.. ConfConstant.Block.Constants.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                ConfConstant.Block.Constants.Remove(ConfConstant.Name);
        }
        else
            return false;

        await GetValue();

        if ((ConfConstant.Type == "pointer" || ConfConstant.Type == "enum") && string.IsNullOrEmpty(ConfConstant.Pointer))
        {
            await Message.Info(Program.BasicForm, "Не заповненні поля", "Потрібна деталізація для типів [ pointer ] або [ enum ]\nВиберіть із списків вказівник або перелічення!");
            return false;
        }

        ConfConstant.Block.AppendConstant(ConfConstant);

        Caption = ConfConstant.Name;
        IsNew = false;

        return true;
    }
}

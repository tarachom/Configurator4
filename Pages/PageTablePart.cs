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
/// Сторінка таблична частина
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageTablePart))]
partial class PageTablePart : FormPageConfigurator
{
    public ConfigurationTablePart ConfTablePart { get; set; } = new();
    public Dictionary<string, ConfigurationTablePart> TabularParts = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    TablePartDataTree dataTree = TablePartDataTree.New();

    partial void Initialize()
    {
        PageName = "Таблична частина:";
        basicFields.TableOrColumnLabel = "Таблиця:";
        basicFields.RenameTableOrColumn.RenameFunc = basicFields.RenameTableOrColumn.RenameTableDef;
    }

    public static PageTablePart New()
    {
        PageTablePart w = NewWithProperties([]);
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
        if (IsNew)
            _ = await Function.FillNewTablePart(ConfTablePart);

        basicFields.ItemName = ConfTablePart.Name;
        basicFields.FullName = ConfTablePart.FullName;
        basicFields.TableOrColumn = ConfTablePart.Table;
        basicFields.Desc = ConfTablePart.Desc;

        dataTree.SetValue(ConfTablePart);
    }

    protected override async Task GetValue()
    {
        ConfTablePart.Name = basicFields.ItemName;
        ConfTablePart.FullName = basicFields.FullName;
        ConfTablePart.Table = basicFields.TableOrColumn;
        ConfTablePart.Desc = basicFields.Desc;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfTablePart.Name, [.. TabularParts.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                TabularParts.Remove(ConfTablePart.Name);
        }
        else
            return false;

        await GetValue();
        TabularParts.Add(ConfTablePart.Name, ConfTablePart);

        Caption = ConfTablePart.Name;
        IsNew = false;

        return true;
    }
}

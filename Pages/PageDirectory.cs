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
/// Сторінка довідника
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageDirectory))]
partial class PageDirectory : FormPageConfigurator
{
    public ConfigurationDirectories ConfDirectory { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    Triggers triggers = Triggers.New();
    DirectoryHierarchy hierarchy = DirectoryHierarchy.New();
    DirectorySubordination subordination = DirectorySubordination.New();
    DirectoryAutomaticNumbering autoNum = DirectoryAutomaticNumbering.New();
    DirectoryDataTree dataTree = DirectoryDataTree.New();

    partial void Initialize()
    {
        PageName = "Довідник:";
        basicFields.TableOrColumnLabel = "Таблиця:";
        basicFields.RenameTableOrColumn.RenameFunc = basicFields.RenameTableOrColumn.RenameTableDef;
    }

    public static PageDirectory New()
    {
        PageDirectory w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Ієрархія
        vBox.Append(hierarchy);

        //Підпорядкування
        vBox.Append(subordination);

        //Автоматична нумерація
        vBox.Append(autoNum);

        //Тригери
        vBox.Append(triggers);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        if (IsNew)
            _ = await Function.FillNewDirectory(ConfDirectory);

        basicFields.ItemName = ConfDirectory.Name;
        basicFields.FullName = ConfDirectory.FullName;
        basicFields.TableOrColumn = ConfDirectory.Table;
        basicFields.Desc = ConfDirectory.Desc;

        triggers.SetValue(ConfDirectory.TriggerFunctions);
        hierarchy.SetValue(ConfDirectory);
        subordination.SetValue(ConfDirectory);
        dataTree.SetValue(ConfDirectory);
        autoNum.SetValue(ConfDirectory);
    }

    protected override async Task GetValue()
    {
        ConfDirectory.Name = basicFields.ItemName;
        ConfDirectory.FullName = basicFields.FullName;
        ConfDirectory.Table = basicFields.TableOrColumn;
        ConfDirectory.Desc = basicFields.Desc;

        ConfDirectory.TriggerFunctions = triggers.GetValue();
        hierarchy.GetValue();
        subordination.GetValue();
        autoNum.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfDirectory.Name, [.. Conf.Directories.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.Directories.Remove(ConfDirectory.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendDirectory(ConfDirectory);

        Caption = ConfDirectory.Name;
        IsNew = false;

        return true;
    }
}

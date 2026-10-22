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
/// Сторінка регістр інформації
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageRegisterInformation))]
partial class PageRegisterInformation : FormPageConfigurator
{
    public ConfigurationRegistersInformation ConfRegInfo { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    RegisterInformationDataTree dataTree = RegisterInformationDataTree.New();

    partial void Initialize()
    {
        PageName = "Регістр інформації:";
        basicFields.TableOrColumnLabel = "Таблиця:";
        basicFields.RenameTableOrColumn.RenameFunc = basicFields.RenameTableOrColumn.RenameTableDef;
    }

    public static PageRegisterInformation New()
    {
        PageRegisterInformation w = NewWithProperties([]);
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
            _ = await Function.FillNewRegInfo(ConfRegInfo);

        basicFields.ItemName = ConfRegInfo.Name;
        basicFields.FullName = ConfRegInfo.FullName;
        basicFields.TableOrColumn = ConfRegInfo.Table;
        basicFields.Desc = ConfRegInfo.Desc;

        dataTree.SetValue(ConfRegInfo);
    }

    protected override async Task GetValue()
    {
        ConfRegInfo.Name = basicFields.ItemName;
        ConfRegInfo.FullName = basicFields.FullName;
        ConfRegInfo.Table = basicFields.TableOrColumn;
        ConfRegInfo.Desc = basicFields.Desc;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfRegInfo.Name, [.. Conf.Directories.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.RegistersInformation.Remove(ConfRegInfo.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendRegistersInformation(ConfRegInfo);

        Caption = ConfRegInfo.Name;
        IsNew = false;

        return true;
    }
}

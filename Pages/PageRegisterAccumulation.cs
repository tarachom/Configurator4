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
/// Сторінка регістр накопичення
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageRegisterAccumulation))]
partial class PageRegisterAccumulation : FormPageConfigurator
{
    public ConfigurationRegistersAccumulation ConfRegAccum { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    AllowDocumentSpend allowDocumentSpend = AllowDocumentSpend.New();
    RegisterAccumulationDataTree dataTree = RegisterAccumulationDataTree.New();

    partial void Initialize()
    {
        PageName = "Регістр накопичення:";
        basicFields.TableOrColumnLabel = "Таблиця:";
        basicFields.RenameTableOrColumn.RenameFunc = basicFields.RenameTableOrColumn.RenameTableDef;
    }

    public static PageRegisterAccumulation New()
    {
        PageRegisterAccumulation w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Документи які роблять рухи по регістру
        vBox.Append(allowDocumentSpend);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        if (IsNew)
            _ = await Function.FillNewRegAccum(ConfRegAccum);

        basicFields.ItemName = ConfRegAccum.Name;
        basicFields.FullName = ConfRegAccum.FullName;
        basicFields.TableOrColumn = ConfRegAccum.Table;
        basicFields.Desc = ConfRegAccum.Desc;

        allowDocumentSpend.SetValue(ConfRegAccum.AllowDocumentSpend);
        dataTree.SetValue(ConfRegAccum);
    }

    protected override async Task GetValue()
    {
        ConfRegAccum.Name = basicFields.ItemName;
        ConfRegAccum.FullName = basicFields.FullName;
        ConfRegAccum.Table = basicFields.TableOrColumn;
        ConfRegAccum.Desc = basicFields.Desc;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) =await IsValid(basicFields.ItemName, ConfRegAccum.Name, [.. Conf.Directories.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.RegistersAccumulation.Remove(ConfRegAccum.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendRegistersAccumulation(ConfRegAccum);

        Caption = ConfRegAccum.Name;
        IsNew = false;

        return true;
    }
}

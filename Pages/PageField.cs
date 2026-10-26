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
/// Сторінка поля
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageField))]
partial class PageField : FormPageConfigurator
{
    public ConfigurationField ConfField { get; set; } = new();
    public Dictionary<string, ConfigurationField> Fields = [];
    Configuration Conf { get; } = Program.Kernel.Conf;
    string ParentTable { get; set; } = "";
    Dictionary<string, ConfigurationField> AllFields = [];

    BasicFields basicFields = BasicFields.New();
    Field field = Field.New();

    partial void Initialize()
    {
        PageName = "Поле:";
        basicFields.TableOrColumnLabel = "В таблиці:";
        basicFields.RenameTableOrColumn.RenameFunc = () => basicFields.RenameTableOrColumn.RenameFieldDef(ParentTable);
    }

    public static PageField New()
    {
        PageField w = NewWithProperties([]);
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
        //Інші
        vBox.Append(field);
    }

    public override async Task AssignValue()
    {
        //Визначення таблиці власника із самого власника
        ParentTable = Owner?.Group switch
        {
            ConfiguratorItemOwnerType.Directory => Owner.Obj is ConfigurationDirectories x ? x.Table : "",
            ConfiguratorItemOwnerType.Document => Owner.Obj is ConfigurationDocuments x ? x.Table : "",
            ConfiguratorItemOwnerType.TablePart => Owner.Obj is ConfigurationTablePart x ? x.Table : "",
            ConfiguratorItemOwnerType.RegisterInformation => Owner.Obj is ConfigurationRegistersInformation x ? x.Table : "",
            ConfiguratorItemOwnerType.RegisterAccumulation => Owner.Obj is ConfigurationRegistersAccumulation x ? x.Table : "",
            _ => ""
        };

        //Для регістрів поля збираються в один масив
        AllFields = Owner?.Group switch
        {
            ConfiguratorItemOwnerType.RegisterInformation => Owner.Obj is ConfigurationRegistersInformation x ? Configuration.CombineAllFieldForRegister(x) : throw new Exception(),
            ConfiguratorItemOwnerType.RegisterAccumulation => Owner.Obj is ConfigurationRegistersAccumulation x ? Configuration.CombineAllFieldForRegister(x) : throw new Exception(),
            _ => Fields
        };

        if (IsNew)
            _ = await Function.FillNewField(ConfField, ParentTable, AllFields);

        basicFields.ItemName = ConfField.Name;
        basicFields.FullName = ConfField.FullName;
        basicFields.TableOrColumn = ConfField.NameInTable;
        basicFields.Desc = ConfField.Desc;

        field.SetValue(ConfField);
    }

    protected override async Task GetValue()
    {
        ConfField.Name = basicFields.ItemName;
        ConfField.FullName = basicFields.FullName;
        ConfField.NameInTable = basicFields.TableOrColumn;
        ConfField.Desc = basicFields.Desc;

        field.GetValue();
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfField.Name, [.. AllFields.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Fields.Remove(ConfField.Name);
        }
        else
            return false;

        await GetValue();

        if ((ConfField.Type == "pointer" || ConfField.Type == "enum") && string.IsNullOrEmpty(ConfField.Pointer))
        {
            await Message.Info(Program.BasicForm, "Не заповненні поля", "Потрібна деталізація для типів [ pointer ] або [ enum ]\nВиберіть із списків вказівник або перелічення!");
            return false;
        }

        Fields.Add(ConfField.Name, ConfField);

        Caption = ConfField.Name;
        IsNew = false;

        return true;
    }
}

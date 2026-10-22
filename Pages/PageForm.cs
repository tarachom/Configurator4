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
/// Сторінка форми
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageForm))]
partial class PageForm : FormPageConfigurator
{
    public ConfigurationForms ConfForm { get; set; } = new();
    //public Dictionary<string, ConfigurationTabularList> TabularLists = [];
    public Dictionary<string, ConfigurationForms> Forms = [];
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();

    partial void Initialize()
    {
        PageName = "Форма:";
        basicFields.HideTableOrColumn();
    }

    public static PageForm New()
    {
        PageForm w = NewWithProperties([]);
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
        basicFields.ItemName = ConfForm.Name;
        basicFields.FullName = ConfForm.FullName;
        basicFields.Desc = ConfForm.Desc;
    }

    protected override async Task GetValue()
    {
        ConfForm.Name = basicFields.ItemName;
        ConfForm.FullName = basicFields.FullName;
        ConfForm.Desc = basicFields.Desc;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = IsValid(basicFields.ItemName, ConfForm.Name, [.. Forms.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Forms.Remove(ConfForm.Name);
        }
        else
            return false;

        await GetValue();
        Forms.Add(ConfForm.Name, ConfForm);

        Caption = ConfForm.Name;
        IsNew = false;

        return true;
    }
}

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
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

namespace Configurator;

/// <summary>
/// Дерево даних
/// </summary>
[Subclass<Box>("DataTree")]
[Template<AssemblyResource>("DataTree.ui")]
public abstract partial class DataTree
{
    protected static async Task OpenPageField(bool isNew, Dictionary<string, ConfigurationField> fields, ConfigurationField? field = null, ConfiguratorItemOwner? owner = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageField(isNew, fields, field, owner);
    }

    protected static async Task OpenPageTablePart(bool isNew, Dictionary<string, ConfigurationTablePart> tabularParts, ConfigurationTablePart? tablePart = null, ConfiguratorItemOwner? owner = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageTablePart(isNew, tabularParts, tablePart, owner);
    }

    protected static async Task OpenPageTabularList(bool isNew, Dictionary<string, ConfigurationTabularList> tabularLists, Dictionary<string, ConfigurationField> fields, ConfigurationTabularList? tabularList = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageTabularList(isNew, tabularLists, fields, tabularList);
    }

    protected static async Task OpenPageForm(bool isNew, Dictionary<string, ConfigurationForms> forms, ConfigurationForms? form = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageForm(isNew, forms, form);
    }

    protected static async Task OpenPageQueryBlock(bool isNew, Dictionary<string, ConfigurationQueryBlock> blocks, ConfigurationQueryBlock? block = null)
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageQueryBlock(isNew, blocks, block);
    }
}
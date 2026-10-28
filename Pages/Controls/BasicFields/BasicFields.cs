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

namespace Configurator;

/// <summary>
/// Базові поля: 
///     Назва
///     Повна назва
///     Таблиця або поле
///     Опис
/// 
/// Використовується для довідника, документу, поля і т.д.
/// </summary>
[Subclass<Box>("BasicFields")]
[Template<AssemblyResource>("BasicFields.ui")]
public partial class BasicFields
{
    [Connect("entry_item_name")] Entry entryItemName;
    [Connect("entry_full_name")] Entry entryFullName;
    [Connect("box_table_or_column")] Box boxTableOrColumn;
    [Connect("label_table_or_column")] Label labelTableOrColumn;
    [Connect("entry_table_or_column")] Entry entryTableOrColumn;
    [Connect("rename_table_or_column")] public RenameTableOrColumn RenameTableOrColumn;
    [Connect("textview_desc")] TextView textviewDesc;

    partial void Initialize()
    {
        //Втрата фокусу полем entryItemName
        {
            EventControllerFocus controller = EventControllerFocus.New();
            controller.OnLeave += (_, _) =>
            {
                if (string.IsNullOrEmpty(FullName))
                {
                    FullName = Configuration.CreateFullName(ItemName);
                    if (string.IsNullOrEmpty(Desc)) Desc = FullName;
                }
            };
            entryItemName.AddController(controller);
        }

        // Зміна значення в полі entryTableOrColumn
        {
            entryTableOrColumn.OnNotify += (sender, args) =>
            {
                if (args.Pspec.GetName() == "text")
                {
                    string text = entryTableOrColumn.GetText();
                    RenameTableOrColumn.OldName = text;
                }
            };
        }

        RenameTableOrColumn.CallBack_Update = (x) => TableOrColumn = x;
    }

    public static BasicFields New()
    {
        //Реєстрація типів
        RenameTableOrColumn.GetGType();
        return NewWithProperties([]);
    }

    /// <summary>
    /// Приховати поле ТаблицяЧиСтовпчик
    /// </summary>
    public void HideTableOrColumn() => labelTableOrColumn.Visible = boxTableOrColumn.Visible = false;

    public string ItemName
    {
        get => entryItemName.GetText();
        set => entryItemName.SetText(value);
    }

    public string FullName
    {
        get => entryFullName.GetText();
        set => entryFullName.SetText(value);
    }

    public string TableOrColumnLabel
    {
        get => labelTableOrColumn.GetText();
        set => labelTableOrColumn.SetText(value);
    }

    public string TableOrColumn
    {
        get => entryTableOrColumn.GetText();
        set => entryTableOrColumn.SetText(value);
    }

    public string Desc
    {
        get => textviewDesc.Buffer?.Text ?? string.Empty;
        set => textviewDesc.Buffer?.Text = value;
    }
}
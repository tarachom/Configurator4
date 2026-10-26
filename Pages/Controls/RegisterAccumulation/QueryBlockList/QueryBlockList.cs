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
/// Вивід списку запитів в блоці запитів
/// </summary>
[Subclass<Box>("QueryBlockList")]
[Template<AssemblyResource>("QueryBlockList.ui")]
public partial class QueryBlockList
{
    [Connect("check_final_calculation")] CheckButton checkFinalCalculation;
    [Connect("button_add")] Button buttonAdd;
    [Connect("button_edit")] Button buttonEdit;
    [Connect("button_copy")] Button buttonCopy;
    [Connect("button_delete")] Button buttonDelete;
    [Connect("button_refresh")] Button buttonRefresh;
    [Connect("button_up")] Button buttonUp;
    [Connect("button_down")] Button buttonDown;
    [Connect("listbox")] ListBox listbox;

    ConfigurationQueryBlock QueryBlock = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    partial void Initialize()
    {
        buttonAdd.OnClicked += async (_, _) => await Add();
        buttonEdit.OnClicked += async (_, _) => await Edit();
        buttonCopy.OnClicked += async (_, _) => await Copy();
        buttonDelete.OnClicked += async (_, _) => await Delete();
        buttonRefresh.OnClicked += (_, _) => Fill();
        buttonUp.OnClicked += (_, _) => UpDown(true);
        buttonDown.OnClicked += (_, _) => UpDown(false);

        GestureClick gesture = GestureClick.New();
        gesture.OnPressed += async (_, args) =>
        {
            if (args.NPress >= 2)
                await Edit();
        };
        listbox.AddController(gesture);
    }

    public static QueryBlockList New() => NewWithProperties([]);

    /// <summary>
    /// Заповню є список
    /// </summary>
    /// <param name="queryList"></param>
    public void SetValue(ConfigurationQueryBlock queryBlock)
    {
        QueryBlock = queryBlock;
        checkFinalCalculation.Active = QueryBlock.FinalCalculation;

        Fill();
    }

    /// <summary>
    /// Перезаповнює основну коленкцію
    /// </summary>
    public void GetValue()
    {
        QueryBlock.FinalCalculation = checkFinalCalculation.Active;
    }

    async Task Add()
    {
        if (Program.BasicForm != null)
            await Program.BasicForm.PageQuery(true, QueryBlock.Query);
    }

    async Task Edit()
    {
        ListBoxRow? selectedRow = listbox.GetSelectedRow();
        if (Program.BasicForm != null && selectedRow != null)
            await Program.BasicForm.PageQuery(false, QueryBlock.Query, selectedRow.GetName());
    }

    async Task Delete()
    {
        ListBoxRow? selectedRow = listbox.GetSelectedRow();
        if (Program.BasicForm != null && selectedRow != null)
            if (await Message.Request(Program.BasicForm, "Питання", "Видалити?") == Message.YesNo.Yes)
            {
                QueryBlock.Query.Remove(selectedRow.GetName());
                Fill();
            }
    }

    async Task Copy()
    {
        ListBoxRow? selectedRow = listbox.GetSelectedRow();
        if (Program.BasicForm != null && selectedRow != null)
        {
            string name = selectedRow.GetName();
            string newName = name + Configuration.GetNewRandomName();
            string value = QueryBlock.Query[name];

            await Program.BasicForm.PageQuery(true, QueryBlock.Query, newName, value);
        }
    }

    /// <summary>
    /// Заповнення
    /// </summary>
    void Fill()
    {
        //Очистка
        FunctionForListBox.RemoveAll(listbox);

        //Заповнення
        foreach (var query in QueryBlock.Query)
        {
            Label label = Label.New(query.Key);
            label.Halign = Align.Start;

            ListBoxRow row = ListBoxRow.New();
            row.Name = query.Key;
            row.Child = label;
            listbox.Append(row);
        }
    }

    /// <summary>
    /// Переміщення вверх-вниз
    /// </summary>
    /// <param name="isUp"></param>
    void UpDown(bool isUp)
    {
        ListBoxRow? row = listbox.GetSelectedRow();
        if (row != null)
        {
            int currIndex = row.GetIndex();
            int newIndex = isUp ? currIndex - 1 : currIndex + 1;
            if (newIndex >= 0 && newIndex < QueryBlock.Query.Count)
            {
                KeyValuePair<string, string> item = QueryBlock.Query.GetAt(currIndex);
                QueryBlock.Query.RemoveAt(currIndex);
                QueryBlock.Query.Insert(newIndex, item.Key, item.Value);

                Fill();

                var targetRow = listbox.GetRowAtIndex(newIndex);
                if (targetRow != null)
                {
                    listbox.SelectRow(targetRow);
                    targetRow.GrabFocus();
                }
            }
        }
    }
}
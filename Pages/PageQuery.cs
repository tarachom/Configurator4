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
/// Сторінка запит
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageQuery))]
partial class PageQuery : FormPageConfigurator
{
    public OrderedDictionary<string, string> QueryList { get; set; } = [];
    public string QueryKey { get; set; } = "";
    public string QueryValue { get; set; } = "";

    Configuration Conf { get; } = Program.Kernel.Conf;

    Entry QueryName = Entry.New();
    CodeView CodeView = CodeView.NewWithLang("sql");

    partial void Initialize()
    {
        PageName = "Запит:";
        QueryName.WidthRequest = 1200;
    }

    public static PageQuery New()
    {
        PageQuery w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Назва
        CreateField(vBox, "Назва", QueryName, Align.Start);

        //Код
        vBox.Append(CodeView);
    }

    public override async Task AssignValue()
    {
        if (!IsNew && QueryList.TryGetValue(QueryKey, out string? queryValue))
            QueryValue = queryValue;

        QueryName.SetText(QueryKey);
        CodeView.Value = QueryValue;
    }

    protected override async Task GetValue()
    {
        QueryKey = QueryName.GetText();
        QueryValue = CodeView.Value;
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(QueryName.GetText(), QueryKey, [.. QueryList.Keys], true);
        QueryName.SetName(name);

        int index = -1;
        if (result)
        {
            if (!IsNew)
            {
                index = QueryList.IndexOf(QueryKey);
                QueryList.Remove(QueryKey);
            }
        }
        else
            return false;

        await GetValue();

        if (index != -1)
            QueryList.Insert(index, QueryKey, QueryValue);
        else
            QueryList.Add(QueryKey, QueryValue);

        Caption = QueryKey;
        IsNew = false;

        return true;
    }
}

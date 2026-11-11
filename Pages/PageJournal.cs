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
/// Сторінка журналу
/// </summary>
[GObject.Subclass<FormPageConfigurator>(nameof(PageJournal))]
partial class PageJournal : FormPageConfigurator
{
    public ConfigurationJournals ConfJournals { get; set; } = new();
    Configuration Conf { get; } = Program.Kernel.Conf;

    BasicFields basicFields = BasicFields.New();
    DocumentsInJournal documents = DocumentsInJournal.New();
    JournalDataTree dataTree = JournalDataTree.New();

    partial void Initialize()
    {
        PageName = "Журнал:";
        basicFields.HideTableOrColumn();
    }

    public static PageJournal New()
    {
        PageJournal w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    protected override void CreateStart(Box vBox)
    {
        //Основні поля
        vBox.Append(basicFields);

        //Документи які входять в журнал
        vBox.Append(documents);
    }

    protected override void CreateEnd(Box vBox)
    {
        vBox.Append(dataTree);
    }

    public override async Task AssignValue()
    {
        //if (IsNew)
        // _ = Function.FillNewConstant(Conf, ConfConstant);

        basicFields.ItemName = ConfJournals.Name;
        basicFields.FullName = ConfJournals.FullName;
        basicFields.Desc = ConfJournals.Desc;
        basicFields.LatinName = ConfJournals.LatinName;

        documents.SetValue(ConfJournals, ConfJournals.AllowDocuments);
        dataTree.SetValue(ConfJournals);
    }

    protected override async Task GetValue()
    {
        ConfJournals.Name = basicFields.ItemName;
        ConfJournals.FullName = basicFields.FullName;
        ConfJournals.Desc = basicFields.Desc;
        ConfJournals.LatinName = basicFields.LatinName;

        ConfJournals.AllowDocuments.Clear();
        ConfJournals.AllowDocuments.AddRange(documents.GetValue());
    }

    protected override async Task<bool> Save()
    {
        (bool result, string name) = await IsValid(basicFields.ItemName, ConfJournals.Name, [.. Conf.Journals.Keys]);
        basicFields.ItemName = name;

        if (result)
        {
            if (!IsNew)
                Conf.Journals.Remove(ConfJournals.Name);
        }
        else
            return false;

        await GetValue();
        Conf.AppendJournal(ConfJournals);

        Caption = ConfJournals.Name;
        IsNew = false;

        return true;
    }
}

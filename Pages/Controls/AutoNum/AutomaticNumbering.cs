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
/// Автоматична нумерація
/// </summary>
[Subclass<Box>("AutomaticNumbering")]
[Template<AssemblyResource>("AutomaticNumbering.ui")]
public abstract partial class AutomaticNumbering
{
    [Connect("check_automatic_numbering")] protected CheckButton checkAutomaticNumbering;
    [Connect("button_create")] protected Button buttonCreate;

    protected Configuration Conf { get; } = Program.Kernel.Conf;

    protected void CreateConst(KeyValuePair<string, string> block, string name) //!!! Можливо винести в функції
    {
        if (!Conf.ConstantsBlock.ContainsKey(block.Key))
            Conf.AppendConstantsBlock(new ConfigurationConstantsBlock(block.Key, block.Value));

        ConfigurationConstantsBlock blockAutoNum = Conf.ConstantsBlock[block.Key];

        //Назва поля в таблиці
        string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, SpecialTables.Constants, Function.GetConstantsAllFields(Conf));

        if (!blockAutoNum.Constants.ContainsKey(name))
            blockAutoNum.AppendConstant(new ConfigurationConstants(name, nameInTable, "integer", blockAutoNum));
    }
}
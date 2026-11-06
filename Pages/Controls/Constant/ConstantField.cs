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
/// Поле
/// </summary>
[Subclass<Box>("ConstantField")]
[Template<AssemblyResource>("ConstantField.ui")]
public partial class ConstantField
{
    [Connect("dropdown_block")] DropDownControl dropdownBlock;
    [Connect("dropdown_type")] DropDownControl dropdownType;
    [Connect("dropdown_pointer")] DropDownControl dropdownPointer;
    [Connect("dropdown_enum")] DropDownControl dropdownEnum;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationConstants ConfConstant { get; set; } = new();

    public static ConstantField New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();
        return NewWithProperties([]);
    }

    partial void Initialize()
    {
        //Блок
        dropdownBlock.AllowEmpty = false;
        dropdownBlock.Fill(Conf.ConstantsBlock.ToDictionary(x => x.Key, x => x.Key));
        dropdownBlock.OnСhanged = () => { };

        //Тип
        dropdownType.AllowEmpty = false;
        dropdownType.Fill(FieldType.GetFieldTypes_Dict());
        dropdownType.Value = "string";
        dropdownType.OnСhanged = () =>
        {
            string typeName = dropdownType.Value;

            dropdownPointer.Sensitive = typeName == "pointer";
            dropdownEnum.Sensitive = typeName == "enum";
        };

        //Вказівники
        {
            Dictionary<string, string> dict =
                Conf.Directories.Values.Select(x => $"Довідники.{x.Name}")
                .Concat(Conf.Documents.Values.Select(x => $"Документи.{x.Name}"))
                .ToDictionary(x => x, x => x);

            dropdownPointer.Fill(dict);
            dropdownPointer.OnСhanged = () => { };
        }

        //Перелічення
        {
            Dictionary<string, string> dict =
                Conf.Enums.Values.Select(x => $"Перелічення.{x.Name}")
                .ToDictionary(x => x, x => x);

            dropdownEnum.Fill(dict);
            dropdownEnum.OnСhanged = () => { };
        }
    }

    public void SetValue(ConfigurationConstants сonstant)
    {
        ConfConstant = сonstant;

        dropdownBlock.Value = ConfConstant.Block.Name;
        dropdownType.Value = ConfConstant.Type;

        if (ConfConstant.Type == "pointer")
            dropdownPointer.Value = ConfConstant.Pointer;

        if (ConfConstant.Type == "enum")
            dropdownEnum.Value = ConfConstant.Pointer;
    }

    public void GetValue()
    {
        //Пошук блоку
        if (Conf.ConstantsBlock.TryGetValue(dropdownBlock.Value, out var block))
            ConfConstant.Block = block;

        ConfConstant.Type = dropdownType.Value;
        ConfConstant.Pointer = ConfConstant.Type switch { "pointer" => dropdownPointer.Value, "enum" => dropdownEnum.Value, _ => "" };
    }
}
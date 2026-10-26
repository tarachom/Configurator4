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
/// Підпорядкування
/// </summary>
[Subclass<Box>("DirectorySubordination")]
[Template<AssemblyResource>("DirectorySubordination.ui")]
public partial class DirectorySubordination
{
    [Connect("dropdown_directory_owner")] DropDownControl dropdownDirectoryOwner;
    [Connect("dropdown_pointer_field_owner")] DropDownControl dropdownPointerFieldOwner;
    [Connect("button_create")] Button buttonCreate;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    public static DirectorySubordination New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();

        DirectorySubordination w = NewWithProperties([]);
        return w;
    }

    partial void Initialize()
    {
        dropdownDirectoryOwner.OnСhanged = () => FillDirectoryField();

        buttonCreate.OnClicked += async (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDirectory.Name) || !Conf.Directories.ContainsKey(ConfDirectory.Name))
            {
                await Message.Info(Program.BasicForm, "Назва довідника не задана або довідник не збережений! Довідник потрібно спочатку зберегти!");
                return;
            }

            if (!string.IsNullOrEmpty(dropdownDirectoryOwner.Value))
            {
                var (Result, _, PointerType) = Configuration.PointerParse(dropdownDirectoryOwner.Value, out var _);
                if (Result)
                {
                    string fieldName = Function.FindNewFieldName(ConfDirectory.Fields, PointerType, (x) => !(x.Type == "pointer" && x.Pointer == dropdownDirectoryOwner.Value));

                    if (!ConfDirectory.Fields.ContainsKey(fieldName))
                    {
                        string nameInTable = Configuration.GetNewUnigueColumnName(Program.Kernel, ConfDirectory.Table, ConfDirectory.Fields);
                        ConfDirectory.AppendField(new ConfigurationField(fieldName, "", nameInTable, "pointer", dropdownDirectoryOwner.Value, "", false, true));

                        FillDirectoryField();
                    }

                    dropdownPointerFieldOwner.Value = fieldName;
                }
            }
        };
    }

    void FillDirectoryField()
    {
        dropdownPointerFieldOwner.RemoveAll();

        var pointer = dropdownDirectoryOwner.Value;
        bool selectFirstField = true;

        foreach (var item in ConfDirectory.Fields.Values)
            if (item.Type == "pointer" && item.Pointer == pointer)
            {
                dropdownPointerFieldOwner.Append(item.Name);

                //Вибрати зразу перше значення
                if (selectFirstField)
                {
                    dropdownPointerFieldOwner.Value = item.Name;
                    selectFirstField = false;
                }
            }
    }

    void FillDirectory()
    {
        foreach (var item in Conf.Directories.Values)
            dropdownDirectoryOwner.Append($"Довідники.{item.Name}", item.Name);
    }

    public void SetValue(ConfigurationDirectories confDirectory)
    {
        ConfDirectory = confDirectory;

        FillDirectory();

        dropdownDirectoryOwner.Value = ConfDirectory.DirectoryOwner_Subordination;
        dropdownPointerFieldOwner.Value = ConfDirectory.PointerFieldOwner_Subordination;
    }

    public void GetValue()
    {
        ConfDirectory.DirectoryOwner_Subordination = dropdownDirectoryOwner.Value;
        ConfDirectory.PointerFieldOwner_Subordination = dropdownPointerFieldOwner.Value;
    }
}
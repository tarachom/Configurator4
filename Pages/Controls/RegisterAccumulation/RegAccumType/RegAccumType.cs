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

[Subclass<Box>("RegAccumType")]
[Template<AssemblyResource>("RegAccumType.ui")]
public partial class RegAccumType
{
    [Connect("dropdown_type")] DropDownControl dropDownType;
    [Connect("button_create")] Button buttonCreate;

    ConfigurationRegistersAccumulation ConfRegAccum { get; set; } = new();

    partial void Initialize()
    {
        dropDownType.AllowEmpty = false;
        dropDownType.Fill(ConfigurationRegistersAccumulation.TypeRegistersAccumulation_Dict());
        dropDownType.OnСhanged = () => ConfRegAccum.TypeRegisters = Enum.Parse<ConfigurationRegistersAccumulation.TypeRegistersAccumulation>(dropDownType.Value);

        buttonCreate.OnClicked += async (_, _) =>
        {
            
        };
    }

    public static RegAccumType New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();
        return NewWithProperties([]);
    }

    public void SetValue(ConfigurationRegistersAccumulation regAccum)
    {
        ConfRegAccum = regAccum;
        dropDownType.Value = ConfRegAccum.TypeRegisters.ToString();
    }
}
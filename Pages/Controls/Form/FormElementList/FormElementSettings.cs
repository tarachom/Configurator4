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
/// Табличний список
/// </summary>
[Subclass<Box>("FormElementSettings")]
[Template<AssemblyResource>("FormElementSettings.ui")]
public partial class FormElementSettings
{
    #region Data

    [Subclass<GObject.Object>()]
    public partial class ItemRow
    {
        public static ItemRow New() => NewWithProperties([]);

        /* Видимість */
        public bool Visible
        {
            get => Visible_;
            set
            {
                if (!Visible_.Equals(value))
                {
                    Visible_ = value;
                    Сhanged_Visible?.Invoke();
                }
            }
        }
        bool Visible_ = false;
        public Action? Сhanged_Visible { get; set; } = null;

        /* Назва */
        public string Name
        {
            get => Name_;
            set
            {
                if (!Name_.Equals(value))
                {
                    Name_ = value;
                    Сhanged_Name?.Invoke();
                }
            }
        }
        string Name_ = "";
        public Action? Сhanged_Name { get; set; } = null;

        /* Заголовок */
        public string Caption
        {
            get => Caption_;
            set
            {
                if (!Caption_.Equals(value))
                {
                    Caption_ = value;
                    Сhanged_Caption?.Invoke();
                }
            }
        }
        string Caption_ = "";
        public Action? Сhanged_Caption { get; set; } = null;

        /* Розмір */
        public uint Size
        {
            get => Size_;
            set
            {
                if (!Size_.Equals(value))
                {
                    Size_ = value;
                    Сhanged_Size?.Invoke();
                }
            }
        }
        uint Size_ = 0;
        public Action? Сhanged_Size { get; set; } = null;

        /* Висота */
        public uint Height
        {
            get => Height_;
            set
            {
                if (!Height_.Equals(value))
                {
                    Height_ = value;
                    Сhanged_Height?.Invoke();
                }
            }
        }
        uint Height_ = 0;
        public Action? Сhanged_Height { get; set; } = null;

        /* Порядок сортування */
        public int SortNum
        {
            get => SortNum_;
            set
            {
                if (!SortNum_.Equals(value))
                {
                    SortNum_ = value;
                    Сhanged_SortNum?.Invoke();
                }
            }
        }
        int SortNum_ = 0;
        public Action? Сhanged_SortNum { get; set; } = null;
    }

    [Subclass<ItemRow>()]
    public partial class ItemRowFields : ItemRow
    {
        public static new ItemRowFields New() => NewWithProperties([]);

        /* Тільки читання */
        public bool ReadOnly
        {
            get => ReadOnly_;
            set
            {
                if (!ReadOnly_.Equals(value))
                {
                    ReadOnly_ = value;
                    Сhanged_ReadOnly?.Invoke();
                }
            }
        }
        bool ReadOnly_ = false;
        public Action? Сhanged_ReadOnly { get; set; } = null;

        /* Тип даних поля */
        public string Type
        {
            get => Type_;
            set
            {
                if (!Type_.Equals(value))
                {
                    Type_ = value;
                    Сhanged_Type?.Invoke();
                }
            }
        }
        string Type_ = "";
        public Action? Сhanged_Type { get; set; } = null;
    }

    #endregion

    #region Fields

    // Верхня секція
    [Connect("button_fields_action")] Button buttonFieldsAction;
    [Connect("button_fields_up")] Button buttonFieldsUp;
    [Connect("button_fields_down")] Button buttonFieldsDown;
    [Connect("column_view_fields")] ColumnView columnViewFields;

    // Нижня секція
    [Connect("button_tabularparts_action")] Button buttonTabularPartsAction;
    [Connect("button_tabularparts_up")] Button buttonTabularPartsUp;
    [Connect("button_tabularparts_down")] Button buttonTabularPartsDown;
    [Connect("column_view_tabularparts")] ColumnView columnViewTabularParts;

    #endregion

    Dictionary<string, ConfigurationField> Fields { get; set; } = [];

    Gio.ListStore StoreFields { get; } = Gio.ListStore.New(ItemRowFields.GetGType());
    Gio.ListStore StoreTabularParts { get; } = Gio.ListStore.New(ItemRow.GetGType());

    /// <summary>
    /// Сортувальник
    /// </summary>
    static CustomSorter Sorter { get; } = CustomSorter.New((ItemRow obj1, ItemRow obj2) => obj1.SortNum.CompareTo(obj2.SortNum));

    /// <summary>
    /// Управління динамічним сортуванням
    /// </summary>
    static bool SorterChangedOnOff { get; set; } = true;

    partial void Initialize()
    {
        {
            SortListModel sort = SortListModel.New(StoreFields, Sorter);
            SingleSelection model = SingleSelection.New(sort);
            model.Autoselect = true;

            columnViewFields.Model = model;
        }

        {
            SortListModel sort = SortListModel.New(StoreTabularParts, Sorter);
            SingleSelection model = SingleSelection.New(sort);
            model.Autoselect = true;

            columnViewTabularParts.Model = model;
        }

        buttonFieldsAction.OnClicked += (_, _) => SelectAll(columnViewFields);
        buttonFieldsUp.OnClicked += (_, _) => UpDown(columnViewFields, true);
        buttonFieldsDown.OnClicked += (_, _) => UpDown(columnViewFields, false);

        buttonTabularPartsAction.OnClicked += (_, _) => SelectAll(columnViewTabularParts);
        buttonTabularPartsUp.OnClicked += (_, _) => UpDown(columnViewTabularParts, true);
        buttonTabularPartsDown.OnClicked += (_, _) => UpDown(columnViewTabularParts, false);

        ColumnStart(columnViewFields);
        ColumnStart(columnViewTabularParts);

        ColumnFields(columnViewFields);

        ColumnEnd(columnViewFields);
        ColumnEnd(columnViewTabularParts);
    }

    static void ColumnStart(ColumnView columnView)
    {
        //Видимість
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = CheckTablePartCell.New();

                cell.Halign = Align.Center;

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not CheckTablePartCell cell) return;
                if (listItem.Item is not ItemRow row) return;

                cell.OnСhanged = () => row.Visible = cell.Value;
                (row.Сhanged_Visible = () => cell.Value = row.Visible).Invoke();

            };
            ColumnViewColumn column = ColumnViewColumn.New("", factory);
            column.Resizable = true;

            columnView.AppendColumn(column);
        }

        //Назва
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = LabelTablePartCell.New();

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not LabelTablePartCell cell) return;
                if (listItem.Item is not ItemRow row) return;

                (row.Сhanged_Name = () => cell.SetText(row.Name)).Invoke();

            };
            ColumnViewColumn column = ColumnViewColumn.New("Назва", factory);
            column.FixedWidth = 200;
            column.Resizable = true;

            columnView.AppendColumn(column);
        }

        //Заголовок
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = TextTablePartCell.New();

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not TextTablePartCell cell) return;
                if (listItem.Item is not ItemRow row) return;

                cell.OnСhanged = () => row.Caption = cell.Value;
                (row.Сhanged_Caption = () => cell.Value = row.Caption).Invoke();

            };
            ColumnViewColumn column = ColumnViewColumn.New("Заголовок", factory);
            column.FixedWidth = 300;
            column.Resizable = true;

            columnView.AppendColumn(column);
        }

        //Розмір
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = IntegerTablePartCell.New();
                cell.Validate = (x) => x >= 0;

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not IntegerTablePartCell cell) return;
                if (listItem.Item is not ItemRow row) return;

                cell.OnСhanged = () => row.Size = (uint)cell.Value;
                (row.Сhanged_Size = () => cell.Value = (int)row.Size).Invoke();

            };
            ColumnViewColumn column = ColumnViewColumn.New("Розмір", factory);
            column.Resizable = true;
            column.FixedWidth = 65;

            columnView.AppendColumn(column);
        }

        //Висота
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = IntegerTablePartCell.New();
                cell.Validate = (x) => x >= 0;

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not IntegerTablePartCell cell) return;
                if (listItem.Item is not ItemRow row) return;

                cell.OnСhanged = () => row.Height = (uint)cell.Value;
                (row.Сhanged_Height = () => cell.Value = (int)row.Height).Invoke();

            };
            ColumnViewColumn column = ColumnViewColumn.New("Висота", factory);
            column.Resizable = true;
            column.FixedWidth = 65;

            columnView.AppendColumn(column);
        }

        //Порядок
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = IntegerTablePartCell.New();

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not IntegerTablePartCell cell) return;
                if (listItem.Item is not ItemRow row) return;

                cell.OnСhanged = () =>
                {
                    row.SortNum = cell.Value;
                    if (SorterChangedOnOff) Sorter.Changed(SorterChange.Different);
                };
                (row.Сhanged_SortNum = () => cell.Value = row.SortNum).Invoke();

            };
            ColumnViewColumn column = ColumnViewColumn.New("Порядок", factory);
            column.Resizable = true;
            column.FixedWidth = 80;
            column.Sorter = Sorter;

            columnView.AppendColumn(column);
        }
    }

    static void ColumnFields(ColumnView columnView)
    {
        //Тільки читання
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = CheckTablePartCell.New();
                cell.Halign = Align.Center;

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not CheckTablePartCell cell) return;
                if (listItem.Item is not ItemRowFields row) return;

                cell.OnСhanged = () => row.ReadOnly = cell.Value;
                (row.Сhanged_ReadOnly = () => cell.Value = row.ReadOnly).Invoke();
            };
            ColumnViewColumn column = ColumnViewColumn.New("Читання", factory);
            column.Resizable = true;
            //column.FixedWidth = 65;

            columnView.AppendColumn(column);
        }

        //Тип
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = LabelTablePartCell.New();

                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not LabelTablePartCell cell) return;
                if (listItem.Item is not ItemRowFields row) return;

                (row.Сhanged_Type = () => cell.SetText(row.Type)).Invoke();
            };
            ColumnViewColumn column = ColumnViewColumn.New("Тип", factory);
            column.FixedWidth = 300;
            column.Resizable = true;

            columnView.AppendColumn(column);
        }
    }

    static void ColumnEnd(ColumnView columnView)
    {
        { /* Пуста колонка для заповнення вільного простору */
            ColumnViewColumn column = ColumnViewColumn.New(null, null);
            column.Resizable = true;
            column.Expand = true;
            columnView.AppendColumn(column);
        }
    }

    public static FormElementSettings New() => NewWithProperties([]);

    static void SelectAll(ColumnView columnView)
    {
        if (columnView.GetModel() is not SingleSelection model) return;
        for (uint i = 0; i < model.GetNItems(); i++)
        {
            ItemRow? row = (ItemRow?)model.GetObject(i);
            row?.Visible = true;
        }
    }

    static void UpDown(ColumnView columnView, bool isUp)
    {
        if (columnView.GetModel() is SingleSelection model && model.SelectedItem is ItemRow currentRow)
        {
            uint selected = model.Selected;

            if (isUp)
            {
                if (selected > 0)
                {
                    uint previous = selected - 1;
                    if (model.GetObject(previous) is ItemRow previousRow)
                        if (currentRow.SortNum != previousRow.SortNum)
                            (previousRow.SortNum, currentRow.SortNum) = (currentRow.SortNum, previousRow.SortNum);
                        else if (currentRow.SortNum > 0)
                            currentRow.SortNum--;
                }
            }
            else if (selected < model.GetNItems() - 1)
            {
                uint next = selected + 1;
                if (model.GetObject(next) is ItemRow nextRow)
                    if (currentRow.SortNum != nextRow.SortNum)
                        (nextRow.SortNum, currentRow.SortNum) = (currentRow.SortNum, nextRow.SortNum);
                    else
                        currentRow.SortNum++;
            }
        }
    }

    public void SetValue(Dictionary<string, ConfigurationField> fields, Dictionary<string, ConfigurationTablePart> tabularParts, ConfigurationForms form)
    {
        Fields = fields;

        {
            StoreFields.RemoveAll();

            int maxSortNum = form.ElementFields.Count > 0 ? form.ElementFields.Values.Max(x => x.SortNum) : 0;
            foreach (ConfigurationField field in fields.Values)
            {
                ItemRowFields item = ItemRowFields.New();
                item.Name = field.Name;

                if (form.ElementFields.TryGetValue(field.Name, out ConfigurationFormsElementField? elemField))
                {
                    item.Visible = true;
                    item.Caption = !string.IsNullOrEmpty(elemField.Caption) ? elemField.Caption : Configuration.CreateFullName(field.Name);
                    item.Size = elemField.Size;
                    item.Height = elemField.Height;
                    item.SortNum = elemField.SortNum;
                    item.ReadOnly = elemField.ReadOnly;
                    Console.WriteLine(item.Name  + " " + item.ReadOnly);
                }
                else
                {
                    item.Caption = Configuration.CreateFullName(field.Name);
                    item.SortNum = ++maxSortNum;
                }

                item.Type = field.Type == "pointer" || field.Type == "enum" ? field.Pointer : field.Type;

                StoreFields.Append(item);
            }
        }

        {
            StoreTabularParts.RemoveAll();

            int maxSortNum = form.ElementTableParts.Count > 0 ? form.ElementTableParts.Values.Max(x => x.SortNum) : 0;
            foreach (ConfigurationTablePart tablePart in tabularParts.Values)
            {
                ItemRow item = ItemRow.New();
                item.Name = tablePart.Name;

                if (form.ElementTableParts.TryGetValue(tablePart.Name, out ConfigurationFormsElementTablePart? elemTablePart))
                {
                    item.Visible = true;
                    item.Caption = !string.IsNullOrEmpty(elemTablePart.Caption) ? elemTablePart.Caption : Configuration.CreateFullName(tablePart.Name);
                    item.Size = elemTablePart.Size;
                    item.Height = elemTablePart.Height;
                    item.SortNum = elemTablePart.SortNum;
                }
                else
                {
                    item.Caption = Configuration.CreateFullName(tablePart.Name);
                    item.SortNum = ++maxSortNum;
                }

                StoreTabularParts.Append(item);
            }
        }
    }

    public void GetValue(ConfigurationForms form)
    {
        form.ElementFields.Clear();
        form.ElementTableParts.Clear();

        if (form.Type == ConfigurationForms.TypeForms.Element || form.Type == ConfigurationForms.TypeForms.TablePart)
        {
            SorterChangedOnOff = false;

            //Поля
            {
                if (columnViewFields.GetModel() is SingleSelection model)
                {
                    int counter = 0;

                    //Обхід видимих полів
                    for (uint i = 0; i < model.GetNItems(); i++)
                        if (model.GetObject(i) is ItemRowFields row && row.Visible)
                        {
                            row.SortNum = ++counter;
                            form.AppendElementField(new(row.Name, row.Caption, row.Size, row.Height, row.SortNum, false, row.ReadOnly));
                        }

                    //Додаткова нумерація невидимих полів
                    for (uint i = 0; i < model.GetNItems(); i++)
                        if (model.GetObject(i) is ItemRowFields row && !row.Visible)
                            row.SortNum = ++counter;
                }
            }

            //Табличні частини
            {
                if (columnViewTabularParts.GetModel() is SingleSelection model)
                {
                    int counter = 0;

                    //Обхід видимих таб частин
                    for (uint i = 0; i <= model.GetNItems(); i++)
                        if (model.GetObject(i) is ItemRow row && row.Visible)
                        {
                            row.SortNum = ++counter;
                            form.AppendElementTablePart(new(row.Name, row.Caption, row.Size, row.Height, row.SortNum));
                        }

                    //Додаткова нумерація невидимих полів
                    for (uint i = 0; i < model.GetNItems(); i++)
                        if (model.GetObject(i) is ItemRow row && !row.Visible)
                            row.SortNum = ++counter;
                }
            }

            SorterChangedOnOff = true;
            Sorter.Changed(SorterChange.Different);
        }
    }
}
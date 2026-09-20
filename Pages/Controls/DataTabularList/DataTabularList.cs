using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

namespace Configurator;

[Subclass<Box>("DataTabularList")]
[Template<AssemblyResource>("DataTabularList.ui")]
public partial class DataTabularList
{
    #region Data

    [Subclass<GObject.Object>()]
    public abstract partial class ItemRow
    {
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
        public static ItemRowFields New() => NewWithProperties([]);

        /* Сортувати по полю */
        public bool SortField
        {
            get => SortField_;
            set
            {
                if (!SortField_.Equals(value))
                {
                    SortField_ = value;
                    Сhanged_SortField?.Invoke();
                }
            }
        }
        bool SortField_ = false;
        public Action? Сhanged_SortField { get; set; } = null;

        /* Зворотнє сортування */
        public bool SortDirection
        {
            get => SortDirection_;
            set
            {
                if (!SortDirection_.Equals(value))
                {
                    SortDirection_ = value;
                    Сhanged_SortDirection?.Invoke();
                }
            }
        }
        bool SortDirection_ = false;
        public Action? Сhanged_SortDirection { get; set; } = null;

        /* Фільтрувати по полю */
        public bool FilterField
        {
            get => FilterField_;
            set
            {
                if (!FilterField_.Equals(value))
                {
                    FilterField_ = value;
                    Сhanged_FilterField?.Invoke();
                }
            }
        }
        bool FilterField_ = false;
        public Action? Сhanged_FilterField { get; set; } = null;

        /* Тип */
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

    [Subclass<ItemRow>()]
    public partial class ItemRowExtraFields : ItemRow
    {
        public static ItemRowExtraFields New() => NewWithProperties([]);

        /* Значення */
        public string Value
        {
            get => Value_;
            set
            {
                if (!Value_.Equals(value))
                {
                    Value_ = value;
                    Сhanged_Value?.Invoke();
                }
            }
        }
        string Value_ = "";
        public Action? Сhanged_Value { get; set; } = null;
    }

    #endregion

    #region Controls

    [Subclass<Box>()]
    public partial class GroupSort : Box
    {
        public static GroupSort New() => NewWithProperties([]);

        partial void Initialize()
        {
            SetOrientation(Orientation.Horizontal);
            SortField.MarginStart = SortField.MarginEnd = 15;

            Append(SortField);
            Append(SortDirection);
        }

        public CheckTablePartCell SortField { get; } = CheckTablePartCell.New();
        public DropDownTablePartCell SortDirection { get; } = DropDownTablePartCell.NewWithValues(new Dictionary<string, string>
        {
            { "Asc", "Asc" },
            { "Desc", "Desc" }
        }, false);
    }

    #endregion

    #region Fields

    // Верхня секція
    [Connect("button_fields_action")] Button buttonFieldsAction;
    [Connect("button_fields_up")] Button buttonFieldsUp;
    [Connect("button_fields_down")] Button buttonFieldsDown;
    [Connect("column_view_fields")] ColumnView columnViewFields;

    // Нижня секція
    [Connect("button_extra_add")] Button buttonExtraAdd;
    [Connect("button_extra_copy")] Button buttonExtraCopy;
    [Connect("button_extra_delete")] Button buttonExtraDelete;
    [Connect("button_extra_up")] Button buttonExtraUp;
    [Connect("button_extra_down")] Button buttonExtraDown;
    [Connect("column_view_extra_fields")] ColumnView columnViewExtraFields;

    #endregion

    Dictionary<string, ConfigurationField> Fields { get; set; } = [];
    ConfigurationTabularList TabularList { get; set; } = new();

    Gio.ListStore StoreFields { get; } = Gio.ListStore.New(ItemRowFields.GetGType());
    Gio.ListStore StoreExtraFields { get; } = Gio.ListStore.New(ItemRowExtraFields.GetGType());

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
            SortListModel sort = SortListModel.New(StoreExtraFields, Sorter);
            SingleSelection model = SingleSelection.New(sort);
            model.Autoselect = true;

            columnViewExtraFields.Model = model;
        }

        buttonFieldsAction.OnClicked += (_, _) => SelectAll(columnViewFields);

        buttonFieldsUp.OnClicked += (_, _) => UpDown(columnViewFields, true);
        buttonFieldsDown.OnClicked += (_, _) => UpDown(columnViewFields, false);
        buttonExtraUp.OnClicked += (_, _) => UpDown(columnViewExtraFields, true);
        buttonExtraDown.OnClicked += (_, _) => UpDown(columnViewExtraFields, false);

        buttonExtraAdd.OnClicked += (_, _) =>
        {
            ItemRowExtraFields item = ItemRowExtraFields.New();

            item.Visible = false;
            item.Name = "Поле";
            item.Caption = item.Name;
            item.SortNum = (int)StoreExtraFields.GetNItems() + 1;

            StoreExtraFields.Append(item);
        };
        buttonExtraCopy.OnClicked += (_, _) =>
        {
            if (columnViewExtraFields.GetModel() is SingleSelection model && model.SelectedItem is ItemRowExtraFields currentRow)
            {
                ItemRowExtraFields item = ItemRowExtraFields.New();
                item.Visible = false;
                item.Name = currentRow.Name;
                item.Size = currentRow.Size;
                item.Caption = currentRow.Caption;
                item.SortNum = (int)StoreExtraFields.GetNItems() + 1;
                item.Value = currentRow.Value;

                StoreExtraFields.Append(item);
            }
        };
        buttonExtraDelete.OnClicked += (_, _) =>
        {
            if (columnViewExtraFields.GetModel() is SingleSelection model && model.SelectedItem is ItemRowExtraFields currentRow)
                StoreExtraFields.Remove(model.Selected);
        };

        ColumnStart(columnViewFields);
        ColumnStart(columnViewExtraFields);

        ColumnFields(columnViewFields);
        ColumnExtraFields(columnViewExtraFields);

        ColumnEnd(columnViewFields);
        ColumnEnd(columnViewExtraFields);
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

                cell.OnСhanged = () => row.Visible = cell.Check.Active;
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

    void ColumnFields(ColumnView columnView)
    {
        //Сортування
        {
            SignalListItemFactory factory = SignalListItemFactory.New();
            factory.OnSetup += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                var cell = GroupSort.New();
                listItem.Child = cell;
            };
            factory.OnBind += (_, args) =>
            {
                if (args.Object is not ListItem listItem) return;
                if (listItem.Child is not GroupSort cell) return;
                if (listItem.Item is not ItemRowFields row) return;

                cell.SortField.OnСhanged = () => row.SortField = cell.SortField.Check.Active;
                (row.Сhanged_SortField = () => cell.SortField.Value = row.SortField).Invoke();

                cell.SortDirection.OnСhanged = () => row.SortDirection = cell.SortDirection.Value == "Desc";
                (row.Сhanged_SortDirection = () => cell.SortDirection.Value = row.SortDirection ? "Desc" : "Asc").Invoke();
            };
            ColumnViewColumn column = ColumnViewColumn.New("Сортування", factory);
            column.Resizable = true;

            columnView.AppendColumn(column);
        }

        //Фільтр
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

                cell.OnСhanged = () => row.FilterField = cell.Check.Active;
                (row.Сhanged_FilterField = () => cell.Value = row.FilterField).Invoke();

            };
            ColumnViewColumn column = ColumnViewColumn.New("Фільтр", factory);
            column.Resizable = true;

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

                (row.Сhanged_Name = () => cell.SetText(row.Type)).Invoke();
            };
            ColumnViewColumn column = ColumnViewColumn.New("Тип", factory);
            column.FixedWidth = 300;
            column.Resizable = true;

            columnView.AppendColumn(column);
        }
    }

    void ColumnExtraFields(ColumnView columnView)
    {
        //Значення
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
                if (listItem.Item is not ItemRowExtraFields row) return;

                cell.OnСhanged = () => row.Value = cell.Value;
                (row.Сhanged_Value = () => cell.Value = row.Value).Invoke();
            };
            ColumnViewColumn column = ColumnViewColumn.New("Значення", factory);
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

    public static DataTabularList New()
    {
        DataTabularList w = NewWithProperties([]);
        return w;
    }

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
            else
                currentRow.SortNum++;
        }
    }

    public void SetValue(bool isNew, Dictionary<string, ConfigurationField> fields, ConfigurationTabularList tabularList)
    {
        Fields = fields;
        TabularList = tabularList;

        int maxSortNum = tabularList.Fields.Count > 0 ? tabularList.Fields.Values.Max(x => x.SortNum) : 0;
        foreach (ConfigurationField field in fields.Values)
        {
            ItemRowFields item = ItemRowFields.New();

            item.Visible = tabularList.Fields.ContainsKey(field.Name); ;
            item.Name = field.Name;
            item.Caption = item.Visible ? (!string.IsNullOrEmpty(tabularList.Fields[field.Name].Caption) ? tabularList.Fields[field.Name].Caption : Configuration.CreateFullName(field.Name)) : Configuration.CreateFullName(field.Name);
            item.Size = item.Visible ? tabularList.Fields[field.Name].Size : 0;
            item.SortNum = item.Visible ? tabularList.Fields[field.Name].SortNum : isNew ? (int)StoreFields.GetNItems() + 1 : ++maxSortNum;
            item.SortField = item.Visible && tabularList.Fields[field.Name].SortField;
            item.SortDirection = item.Visible && tabularList.Fields[field.Name].SortDirection;
            item.FilterField = item.Visible && tabularList.Fields[field.Name].FilterField;
            item.Type = field.Type == "pointer" || field.Type == "enum" ? field.Pointer : field.Type;

            StoreFields.Append(item);
        }

        foreach (ConfigurationTabularListAdditionalField field in tabularList.AdditionalFields.Values)
        {
            ItemRowExtraFields item = ItemRowExtraFields.New();

            item.Visible = field.Visible;
            item.Name = field.Name;
            item.Caption = field.Caption;
            item.Size = field.Size;
            item.SortNum = field.SortNum;
            item.Value = field.Value;

            StoreExtraFields.Append(item);
        }
    }

    public void GetValue()
    {
        TabularList.Fields.Clear();
        TabularList.AdditionalFields.Clear();

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
                        TabularList.AppendField(new(row.Name, row.Caption, row.Size, row.SortNum, row.SortField, row.SortDirection, row.FilterField));
                    }

                //Додаткова нумерація невидимих полів
                for (uint i = 0; i < model.GetNItems(); i++)
                    if (model.GetObject(i) is ItemRowFields row && !row.Visible)
                        row.SortNum = ++counter;
            }
        }

        //Додаткові поля
        {
            int counter = 0;
            if (columnViewExtraFields.GetModel() is SingleSelection model)
                for (uint i = 0; i <= model.GetNItems(); i++)
                    if (model.GetObject(i) is ItemRowExtraFields row)
                    {
                        //Перевірка унікальності імені поля і створення нового імені
                        string newName = row.Name;
                        for (int n = 1; TabularList.AdditionalFields.ContainsKey(newName) && n < 100; n++)
                            newName = row.Name + n;
                        row.Name = newName;

                        row.SortNum = ++counter;
                        TabularList.AppendAdditionalField(new(row.Visible, row.Name, row.Caption, row.Size, row.SortNum, "string", row.Value));
                    }
        }

        SorterChangedOnOff = true;
        Sorter.Changed(SorterChange.Different);
    }
}
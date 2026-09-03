using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;

namespace Configurator;

[Subclass<Box>("Field")]
[Template<AssemblyResource>("Field.ui")]
public partial class Field
{
    [Connect("dropdown_type")] DropDownControl dropdownType;
    [Connect("dropdown_pointer")] DropDownControl dropdownPointer;
    [Connect("dropdown_enum")] DropDownControl dropdownEnum;

    [Connect("check_index")] CheckButton checkIndex;
    [Connect("check_presentation")] CheckButton checkPresentation;
    [Connect("check_fulltext_search")] CheckButton checkFulltextSearch;
    [Connect("check_journal_search")] CheckButton checkJournalSearch;
    [Connect("check_multiline")] CheckButton checkMultiline;
    [Connect("check_auto_numbering")] CheckButton checkAutoNumbering;
    [Connect("check_allow_export")] CheckButton checkAllowExport;

    [Connect("check_directories_not_use")] CheckButton checkDirectoriesNotUse;
    [Connect("listbox_directories")] ListBox listboxDirectories;
    [Connect("check_documents_not_use")] CheckButton checkDocumentsNotUse;
    [Connect("listbox_documents")] ListBox listboxDocuments;

    Configuration Conf { get; } = Program.Kernel.Conf;
    ConfigurationField ConfField { get; set; } = new();

    public static Field New()
    {
        //Реєстрація типів
        DropDownControl.GetGType();

        Field widget = NewWithProperties([]);
        return widget;
    }

    partial void Initialize()
    {
        dropdownType.AllowEmpty = false;
        dropdownType.Fill(FieldType.GetFieldTypes_Dict());
        dropdownType.Value = "string";
        dropdownType.OnСhanged = () =>
        {
            string typeName = dropdownType.Value;

            dropdownPointer.Sensitive = typeName == "pointer";
            dropdownEnum.Sensitive = typeName == "enum";

            checkPresentation.Sensitive =
                typeName == "string" ||
                typeName == "integer" ||
                typeName == "numeric" ||
                typeName == "boolean" ||
                typeName == "date" ||
                typeName == "datetime" ||
                typeName == "time";

            //string only
            {
                bool isString = typeName == "string";

                checkFulltextSearch.Sensitive = isString;
                checkMultiline.Sensitive = isString;
            }

            //integer only
            {
                bool isInteger = typeName == "integer";

                checkAutoNumbering.Sensitive = isInteger;
            }

            checkJournalSearch.Sensitive =
                typeName == "string" ||
                typeName == "integer" ||
                typeName == "numeric" ||
                typeName == "date" ||
                typeName == "datetime" ||
                typeName == "time";

            //composite_pointer only
            {
                bool isCompositePointer = typeName == "composite_pointer";

                checkDirectoriesNotUse.Sensitive = isCompositePointer;
                checkDocumentsNotUse.Sensitive = isCompositePointer;
                listboxDirectories.Sensitive = isCompositePointer;
                listboxDocuments.Sensitive = isCompositePointer;
            }
        };

        //Вказівники
        {
            Dictionary<string, string> dict =
                Conf.Directories.Values.Select(x => $"Довідники.{x.Name}")
                .Concat(Conf.Documents.Values.Select(x => $"Документи.{x.Name}"))
                .ToDictionary(x => x, x => x);

            dropdownPointer.Fill(dict);
            dropdownPointer.OnСhanged = () =>
            {

            };
        }

        //Перелічення
        {
            Dictionary<string, string> dict =
                Conf.Enums.Values.Select(x => $"Перелічення.{x.Name}")
                .ToDictionary(x => x, x => x);

            dropdownEnum.Fill(dict);
            dropdownEnum.OnСhanged = () =>
            {

            };
        }

        //Для композитного типу
        {
            checkDirectoriesNotUse.OnNotify += (_, args) =>
            {
                if (args.Pspec.GetName() == "active")
                    listboxDirectories.Sensitive = !checkDirectoriesNotUse.Active;
            };

            checkDocumentsNotUse.OnNotify += (_, args) =>
            {
                if (args.Pspec.GetName() == "active")
                    listboxDocuments.Sensitive = !checkDocumentsNotUse.Active;
            };
        }
    }

    public void SetValue(ConfigurationField field)
    {
        ConfField = field;

        dropdownType.Value = ConfField.Type;

        if (ConfField.Type == "pointer")
            dropdownPointer.Value = ConfField.Pointer;

        if (ConfField.Type == "enum")
            dropdownEnum.Value = ConfField.Pointer;

        checkIndex.Active = ConfField.IsIndex;
        checkPresentation.Active = ConfField.IsPresentation;
        checkFulltextSearch.Active = ConfField.IsFullTextSearch;
        checkJournalSearch.Active = ConfField.IsSearch;
        checkMultiline.Active = ConfField.Multiline;
        checkAutoNumbering.Active = ConfField.AutomaticNumbering;
        checkAllowExport.Active = ConfField.IsExport;

        checkDirectoriesNotUse.Active = ConfField.CompositePointerNotUseDirectories;
        checkDocumentsNotUse.Active = ConfField.CompositePointerNotUseDocuments;

        //Для композитного типу
        {
            foreach (KeyValuePair<string, ConfigurationDirectories> directories in Conf.Directories)
            {
                CheckButton cb = CheckButton.NewWithLabel(directories.Key);
                cb.Active = ConfField.CompositePointerAllowDirectories.Contains(directories.Key);
                listboxDirectories.Append(cb);
            }

            foreach (KeyValuePair<string, ConfigurationDocuments> documents in Conf.Documents)
            {
                CheckButton cb = CheckButton.NewWithLabel(documents.Key);
                cb.Active = ConfField.CompositePointerAllowDocuments.Contains(documents.Key);
                listboxDocuments.Append(cb);
            }
        }
    }

    public void GetValue()
    {
        ConfField.Type = string.IsNullOrEmpty(dropdownType.Value) ? "string" : dropdownType.Value;
        ConfField.Pointer = ConfField.Type switch { "pointer" => dropdownPointer.Value, "enum" => dropdownEnum.Value, _ => "" };
        ConfField.IsIndex = checkIndex.Active;
        ConfField.IsPresentation = checkPresentation.Active;
        ConfField.IsFullTextSearch = checkFulltextSearch.Active;
        ConfField.IsSearch = checkJournalSearch.Active;
        ConfField.Multiline = checkMultiline.Active;
        ConfField.AutomaticNumbering = checkAutoNumbering.Active;
        ConfField.IsExport = checkAllowExport.Active;

        ConfField.CompositePointerNotUseDirectories = checkDirectoriesNotUse.Active;
        ConfField.CompositePointerNotUseDocuments = checkDocumentsNotUse.Active;

        //Для композитного типу
        if (ConfField.Type == "composite_pointer")
        {
            //Локальна функція отримує значення із списків
            static List<string> getAllow(ListBox listBox)
            {
                List<string> list = [];
                var row = listBox.GetFirstChild();
                while (row != null)
                {
                    if (row.GetFirstChild() is CheckButton cb && cb.Active && cb.Label != null) list.Add(cb.Label);
                    row = row.GetNextSibling();
                }

                return list;
            }

            ConfField.CompositePointerAllowDirectories = getAllow(listboxDirectories);
            ConfField.CompositePointerAllowDocuments = getAllow(listboxDocuments);
        }
    }
}
using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

namespace Configurator;

[Subclass<Box>("Field")]
[Template<AssemblyResource>("Field.xml")]
public partial class Field
{
    [Connect("entry_name")] Entry entryName;
    [Connect("entry_full_name")] Entry entryFullName;
    [Connect("entry_table_column")] Entry entryTableColumn;
    [Connect("textview_description")] TextView textviewDescription;

    [Connect("dropdown_type")] DropDown dropdownType;
    [Connect("dropdown_pointer")] DropDown dropdownPointer;
    [Connect("dropdown_enum")] DropDown dropdownEnum;

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

    public static Field New()
    {
        Field w = NewWithProperties([]);
        return w;
    }

    partial void Initialize()
    {

    }
}
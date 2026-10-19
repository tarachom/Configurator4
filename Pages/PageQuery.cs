/*

Стартова довідника

*/

using Gtk;
using InterfaceGtk4;
using AccountingSoftware;

namespace Configurator;

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
        QueryName.WidthRequest = 800;

        //Прибираю другу панель, так як вона не потрібна
        HPanedTop.SetEndChild(null);
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
        (bool result, string name) = IsValid(QueryName.GetText(), QueryKey, [.. QueryList.Keys]);
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

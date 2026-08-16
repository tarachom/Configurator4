using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

[Subclass<AutomaticNumbering>()]
public partial class DocumentAutomaticNumbering : AutomaticNumbering
{
    ConfigurationDocuments ConfDocuments { get; set; } = new();

    public static DocumentAutomaticNumbering New()
    {
        DocumentAutomaticNumbering w = NewWithProperties([]);
        return w;
    }

    partial void Initialize()
    {
        buttonCreate.OnClicked += (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDocuments.Name) || !Conf.Documents.ContainsKey(ConfDocuments.Name))
            {
                Message.Error(Program.BasicForm, "Назва документу не задана або документ не збережений! Документ потрібно спочатку зберегти!");
                return;
            }

            //Створення константи
            CreateConst(new("НумераціяДокументів", "Нумерація документів"), ConfDocuments.Name);

            checkAutomaticNumbering.Active = true;
        };
    }

    public void SetValue(ConfigurationDocuments confDocuments)
    {
        ConfDocuments = confDocuments;
        checkAutomaticNumbering.Active = ConfDocuments.AutomaticNumeration;
    }

    public void GetValue()
    {
        ConfDocuments.AutomaticNumeration = checkAutomaticNumbering.Active;
    }
}
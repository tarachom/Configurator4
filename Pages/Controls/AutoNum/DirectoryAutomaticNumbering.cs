using Gtk;
using GObject;
using AccountingSoftware;
using InterfaceGtk4;
using Configurator;

[Subclass<AutomaticNumbering>()]
public partial class DirectoryAutomaticNumbering : AutomaticNumbering
{
    ConfigurationDirectories ConfDirectory { get; set; } = new();

    public static DirectoryAutomaticNumbering New()
    {
        DirectoryAutomaticNumbering w = NewWithProperties([]);
        return w;
    }

    partial void Initialize()
    {
        buttonCreate.OnClicked += (_, _) =>
        {
            if (string.IsNullOrEmpty(ConfDirectory.Name) || !Conf.Directories.ContainsKey(ConfDirectory.Name))
            {
                Message.Error(Program.BasicForm, "Назва довідника не задана або довідник не збережений! Довідник потрібно спочатку зберегти!");
                return;
            }

            //Створення константи
            CreateConst(new("НумераціяДовідників", "Нумерація довідників"), ConfDirectory.Name);

            checkAutomaticNumbering.Active = true;
        };
    }

    public void SetValue(ConfigurationDirectories confDirectory)
    {
        ConfDirectory = confDirectory;
        checkAutomaticNumbering.Active = ConfDirectory.AutomaticNumeration;
    }

    public void GetValue()
    {
        ConfDirectory.AutomaticNumeration = checkAutomaticNumbering.Active;
    }
}
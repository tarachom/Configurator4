/*

Стартова сторінка

*/

using Gtk;
using InterfaceGtk4;
using InterfaceGtkLib;

namespace Configurator;

[GObject.Subclass<Form>]
partial class PageHome : Form
{
    partial void Initialize()
    {
        {
            ActiveUsersView view = ActiveUsersView.New(Program.Kernel, 800, 200);

            Box hBox = New(Orientation.Horizontal, 0);
            hBox.MarginBottom = 10;
            hBox.Append(view);
            Append(hBox);
        }

        {
            AiChatView view = AiChatView.New(800);

            Box hBox = New(Orientation.Horizontal, 0);
            hBox.MarginBottom = 10;
            hBox.Append(view);
            Append(hBox);
        }
    }

    public static PageHome New()
    {
        PageHome w = NewWithProperties([]);
        w.NotebookFunc = Program.BasicForm?.NotebookFunc;

        return w;
    }

    public async ValueTask SetValue()
    {
        //Автоматичний запуск AI клієнта
        GlobalConfigurationParam? globalConfigurationParam = Program.BasicForm?.GlobalConfigurationParam;
        if (globalConfigurationParam != null && globalConfigurationParam.AIStartOnRun)
            try
            {
                bool result = FunctionForAI.CreateClient(globalConfigurationParam.AIKey, globalConfigurationParam.AIModel);
            }
            catch (Exception ex)
            {
                Message.Error(Program.BasicForm, "Помилка", "Під час запуску AI клієнта виникла помилка:\n\n" + ex.Message);
            }
    }
}

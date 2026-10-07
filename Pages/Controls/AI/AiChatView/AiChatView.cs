using Gtk;
using GObject;
using AccountingSoftware;
using Configurator;
using InterfaceGtk4;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace Configurator;

[Subclass<Box>("AiChatView")]
[Template<AssemblyResource>("AiChatView.ui")]
public partial class AiChatView
{
    [Connect("scroll_history")] ScrolledWindow scrollHistory;
    [Connect("textview_history")] TextView textviewHistory;
    [Connect("scroll_prompt")] ScrolledWindow scrollPrompt;
    [Connect("textview_prompt")] TextView textviewPrompt;
    [Connect("button_send")] Button buttonSend;

    protected Configuration Conf { get; } = Program.Kernel.Conf;

    public static AiChatView New(int widthRequest)
    {
        AiChatView w = NewWithProperties([]);
        w.WidthRequest = widthRequest;

        return w;
    }

    partial void Initialize()
    {
        textviewPrompt.Buffer?.Text = "Покажи коротко списком операції які ти можеш виконати";

        buttonSend.OnClicked += (_, _) => Send();

        var keyController = EventControllerKey.New();
        textviewPrompt.AddController(keyController);
        keyController.OnKeyPressed += (sender, args) =>
        {
            bool isEnter = args.Keyval == Gdk.Constants.KEY_Return || args.Keyval == Gdk.Constants.KEY_KP_Enter;
            bool isCtrl = (args.State & Gdk.ModifierType.ControlMask) != 0;
            if (isEnter && isCtrl)
            {
                Send();
                return true;
            }
            return false;
        };
    }

    async void Send()
    {
        if (FunctionForAI.Client != null)
        {
            string? prompt = textviewPrompt.Buffer?.Text;
            //string? history = textviewHistory.Buffer?.Text;
            if (string.IsNullOrEmpty(prompt))
                return;

            ApendLine($"User: {prompt}\n");
            textviewPrompt.Buffer?.Text = "";

            ChatOptions options = new()
            {
                Temperature = 0,
                ResponseFormat = ChatResponseFormat.Text,
                Tools = [
                    AIFunctionFactory.Create(ConstantsBlockList, name: "ConstantsBlockList", description: "Повний список блоків констант у форматі ключ і опис в дужках. Завжди використовуй ключ, опис тільки для розуміння призначення блоку"),
                        AIFunctionFactory.Create(ConstantsList, name: "ConstantsList", description: "Повний список констант певного блоку у форматі ключ і опис в дужках. Завжди використовуй ключ, опис тільки для розуміння призначення константи"),

                        AIFunctionFactory.Create(DirectoryList, name: "DirectoryList", description: "Повний список довідників у форматі ключ і опис в дужках. Завжди використовуй ключ, опис тільки для розуміння призначення довідника"),
                        AIFunctionFactory.Create(DocumentsList, name: "DocumentsList", description: "Повний список документів у форматі ключ і опис в дужках. Завжди використовуй ключ, опис тільки для розуміння призначення документу"),

                        AIFunctionFactory.Create(EnumsList, name: "EnumsList", description: "Повний список перелічень у форматі ключ і опис в дужках. Завжди використовуй ключ, опис тільки для розуміння призначення перелічення"),

                        AIFunctionFactory.Create(RegistersInformationList, name: "RegistersInformationList", description: "Повний список регістрів інформації у форматі ключ і опис в дужках. Завжди використовуй ключ, опис тільки для розуміння призначення регістру"),
                        AIFunctionFactory.Create(RegistersAccumulationList, name: "RegistersAccumulationList", description: "Повний список регістрів накопичення у форматі ключ і опис в дужках. Завжди використовуй ключ, опис тільки для розуміння призначення регістру"),

                        AIFunctionFactory.Create(DirectoryOpen, name: "DirectoryOpen", description: "Відкрити довідник. Спочатку обов'язково потрібно отримати список довідників. Список всіх довідників повертає функція DirectoryList"),
                        AIFunctionFactory.Create(DocumentOpen, name: "DocumentOpen", description: "Відкрити документ. Спочатку обов'язково потрібно отримати список документів. Список всіх документів повертає функція DocumentsList"),

                        AIFunctionFactory.Create(NewDirectory, name: "NewDirectory",  description: "Додати новий довідник. Спочатку обов'язково потрібно отримати список довідників. Список всіх довідник повертає функція DirectoryList"),
                        AIFunctionFactory.Create(NewDocument, name: "NewDocument",  description: "Додати новий документ. Спочатку обов'язково потрібно отримати список документів. Список всіх документів повертає функція DocumentsList"),
                    ]
            };

            try
            {
                string result = "";
                await foreach (var update in FunctionForAI.Client.GetStreamingResponseAsync(prompt, options))
                    result += update.ToString();

                ApendLine(result);
            }
            catch (Exception ex)
            {
                ApendLine($"Err: {ex.Message}");
            }

            //await Task.Delay(500);
            //ApendLine("");
        }
    }

    void ApendLine(string message)
    {
        if (textviewHistory != null && textviewHistory.Buffer != null)
        {
            var buffer = textviewHistory.Buffer;
            buffer.GetEndIter(out TextIter iterEndText);

            string text = message + "\n";
            buffer.Insert(iterEndText, text, -1);

            GLib.Functions.IdleAdd(GLib.Constants.PRIORITY_DEFAULT_IDLE, () =>
            {
                buffer.GetEndIter(out TextIter iterEndText);
                textviewHistory.ScrollToIter(iterEndText, 0.0, false, 0.0, 0.0);
                return false;
            });
        }
    }

    #region Function

    #region List

    [Description("Повертає список всіх блоків констант у форматі ключ та назва і опис в дужках для пояснення")]
    async Task<string[]> ConstantsBlockList()
    {
        ApendLine($"[F] Вибірка списку блоків констант");
        return [.. Conf.ConstantsBlock.Select(x => x.Key + " (" + x.Value.BlockName + ", " + x.Value.Desc + ")")];
    }

    [Description("Повертає список всіх констант певного блоку у форматі ключ та назва і опис в дужках для пояснення")]
    async Task<string[]> ConstantsList(
        [Description(@"Ключ блоку констант отриманий із списку блоків констант який повертає функція ConstantsBlockList. Використовуй тільки ключ для цього параметру.")]
        string constantsBlockKey)
    {
        ApendLine($"[F] Вибірка списку констант блоку: {constantsBlockKey}");
        if (Conf.ConstantsBlock.TryGetValue(constantsBlockKey, out ConfigurationConstantsBlock? block))
            return [.. block.Constants.Select(x => x.Key + " (" + x.Value.Name + ", " + x.Value.Desc + ")")];
        else
        {
            ApendLine($"[F] Не знайдено блок констант з назвою: {constantsBlockKey} в колекції");
            return [];
        }
    }



    [Description(@"Повертає список всіх довідників у форматі ключ та повна назва і опис в дужках для пояснення")]
    async Task<string[]> DirectoryList()
    {
        ApendLine($"[F] Вибірка списку довідників");
        return [.. Conf.Directories.Select(x => x.Key + " (" + x.Value.FullName + ", " + x.Value.Desc + ")")];
    }

    [Description("Повертає список всіх документів у форматі ключ та повна назва і опис в дужках для пояснення")]
    async Task<string[]> DocumentsList()
    {
        ApendLine($"[F] Вибірка списку документів");
        return [.. Conf.Documents.Select(x => x.Key + " (" + x.Value.FullName + ", " + x.Value.Desc + ")")];
    }

    [Description("Повертає список всіх перелічень у форматі ключ та назва і опис в дужках для пояснення")]
    async Task<string[]> EnumsList()
    {
        ApendLine($"[F] Вибірка списку переліченнь");
        return [.. Conf.Enums.Select(x => x.Key + " (" + x.Value.Name + ", " + x.Value.Desc + ")")];
    }

    [Description("Повертає список всіх регістрів інформації у форматі ключ та повна назва і опис в дужках для пояснення")]
    async Task<string[]> RegistersInformationList()
    {
        ApendLine($"[F] Вибірка списку регістрів інформації");
        return [.. Conf.RegistersInformation.Select(x => x.Key + " (" + x.Value.FullName + ", " + x.Value.Desc + ")")];
    }

    [Description("Повертає список всіх регістрів накопичення у форматі ключ та повна назва і опис в дужках для пояснення")]
    async Task<string[]> RegistersAccumulationList()
    {
        ApendLine($"[F] Вибірка списку регістрів накопичення");
        return [.. Conf.RegistersAccumulation.Select(x => x.Key + " (" + x.Value.FullName + ", " + x.Value.Desc + ")")];
    }

    #endregion

    #region Open

    [Description(@"Відкриває довідник. Спочатку потрібно отримати список всіх довідників функцією DirectoryList. Потрібно перевірити чи є ключ довідника в списку довідників")]
    async Task DirectoryOpen([Description(@"Ключ довідника отриманий із списку довідників який повертає функція DirectoryList. Використовуй тільки ключ для цього параметру.")] string directoryKey)
    {
        ApendLine($"[F] Відкрити довідник: {directoryKey}");
        if (Program.BasicForm != null)
            if (Conf.Directories.TryGetValue(directoryKey, out ConfigurationDirectories? directory))
                await Program.BasicForm.PageDirectory(false, directory);
            else
                ApendLine($"[F] Не знайдено довідник з назвою: {directoryKey} в колекції");
    }

    [Description(@"Відкриває документ. Спочатку потрібно отримати список всіх документів функцією DocumentsList. Потрібно перевірити чи є ключ документу в списку документів")]
    async Task DocumentOpen([Description(@"Ключ документу отриманий із списку документів який повертає функція DocumentsList. Використовуй тільки ключ для цього параметру.")] string documentKey)
    {
        ApendLine($"[F] Відкрити документ: {documentKey}");
        if (Program.BasicForm != null)
            if (Conf.Documents.TryGetValue(documentKey, out ConfigurationDocuments? document))
                await Program.BasicForm.PageDocument(false, document);
            else
                ApendLine($"[F] Не знайдено документ з назвою: {documentKey} в колекції");
    }



    #endregion

    #region New

    async Task NewDirectory(
        [Description(@"Ключ довідника. Спочатку вибери список довідників функцією DirectoryList і перевір чи не має вже такого.")]
        string directoryKey,
        [Description(@"Повна назва довідника")]
        string directoryName
            )
    {
        ApendLine($"[F] Новий довідник: {directoryKey}, {directoryName}");
        if (Program.BasicForm != null)
        {
            ConfigurationDirectories directory = new()
            {
                Name = directoryKey,
                FullName = directoryName,
                Desc = directoryName
            };

            await Program.BasicForm.PageDirectory(true, directory);
        }
    }

    async Task NewDocument(
        [Description(@"Ключ документу. Спочатку вибери список документів функцією DocumentsList і перевір чи не має вже такого.")]
        string documentKey,
        [Description(@"Повна назва документу")]
        string documentName
        )
    {
        ApendLine($"[F] Новий документ : {documentKey}, {documentName}");
        if (Program.BasicForm != null)
        {
            ConfigurationDocuments document = new()
            {
                Name = documentKey,
                FullName = documentName,
                Desc = documentName
            };

            await Program.BasicForm.PageDocument(true, document);
        }
    }

    #endregion

    #endregion
}
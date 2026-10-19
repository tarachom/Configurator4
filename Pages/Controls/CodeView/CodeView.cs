using Gtk;
using GObject;
using GtkSource;

namespace Configurator;

[Subclass<Box>("CodeView")]
[Template<AssemblyResource>("CodeView.ui")]
public partial class CodeView
{
    [Connect("source_view")] View sourceView;

    GtkSource.Buffer SourceBuffer = GtkSource.Buffer.New(null);

    partial void Initialize()
    {
        sourceView.SetBuffer(SourceBuffer);
    }

    public static CodeView NewWithLang(string lang)
    {
        CodeView view = NewWithProperties([]);
        view.SourceBuffer.SetLanguage(LanguageManager.New().GetLanguage(lang));

        return view;
    }

    public string Value
    {
        get => SourceBuffer.Text ?? "";
        set => SourceBuffer.Text = value;
    }
}
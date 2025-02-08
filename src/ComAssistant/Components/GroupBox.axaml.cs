using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace ComAssistant.Components;

public class GroupBox : HeaderedContentControl
{
    public static readonly StyledProperty<IBrush?> ActualBackgroundProperty =
        AvaloniaProperty.Register<GroupBox, IBrush?>(
            nameof(ActualBackground));

    public IBrush? ActualBackground
    {
        get => GetValue(ActualBackgroundProperty);
        set => SetValue(ActualBackgroundProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var background = Background;
        var parent = this.GetVisualParent<TemplatedControl>();
        while ((background == null ||
                (background is SolidColorBrush solidColorBrush && solidColorBrush.Color == Colors.Transparent)) &&
               parent != null)
        {
            background = parent.Background;
            parent = this.GetVisualParent<TemplatedControl>();
        }

        if (background == null ||
            (background is SolidColorBrush solidColorBrush1 && solidColorBrush1.Color == Colors.Transparent))
        {
            background = TopLevel.GetTopLevel(this)?.Background;
        }

        ActualBackground = background;
    }
}
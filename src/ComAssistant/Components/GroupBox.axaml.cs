using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace ComAssistant.Components;

public class GroupBox : HeaderedContentControl
{
    private Control? _headerPart;
    private Path? _borderPart;
    private Grid? _rootPart;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _headerPart = this.GetTemplateChildren().SingleOrDefault(x => x.Name == "PART_Header") as ContentControl;
        _borderPart = this.GetTemplateChildren().SingleOrDefault(x => x.Name == "PART_Border") as Path;
        _rootPart = this.GetTemplateChildren().SingleOrDefault(x => x.Name == "PART_Root") as Grid;
        SetBorder();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        SetBorder();
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        SetBorder();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        switch (change.Property.Name)
        {
            case nameof(CornerRadius):
                SetBorder();
                break;
            case nameof(BorderThickness):
                SetBorder();
                break;
        }
    }

    private void SetBorder()
    {
        if (_headerPart != null && _rootPart != null && _borderPart != null)
        {
            _headerPart.Margin = new Thickness(CornerRadius.BottomLeft + 10, 0, 0, 0);
            var top = (_headerPart.Bounds.Bottom - _headerPart.Bounds.Top) / 2;
            var left = 0;
            var bottom = _borderPart.Bounds.Bottom;
            var right = _borderPart.Bounds.Right;
            var startPoint = new Point(CornerRadius.BottomLeft + 10, top);
            var endPoint = new Point(CornerRadius.BottomLeft + 10 + _headerPart.Bounds.Width, top);
            var path = $"M{startPoint.X} {startPoint.Y}";
            path += $" L{CornerRadius.TopLeft} {startPoint.Y}";
            if (CornerRadius.TopLeft > 0)
            {
                path += $" A{CornerRadius.TopLeft} {CornerRadius.TopLeft} 0 0 0 {left} {CornerRadius.TopLeft + startPoint.Y}";
            }

            path += $" L0 {bottom - CornerRadius.BottomLeft}";

            if (CornerRadius.BottomLeft > 0)
            {
                path += $" A{CornerRadius.BottomLeft} {CornerRadius.BottomLeft} 0 0 0 {CornerRadius.BottomLeft} {bottom}";
            }

            path += $" L{right - CornerRadius.BottomRight} {bottom}";

            if (CornerRadius.BottomRight > 0)
            {
                path += $" A{CornerRadius.BottomRight} {CornerRadius.BottomRight} 0 0 0 {right} {bottom - CornerRadius.BottomRight}";
            }

            path += $" L{right} {top + CornerRadius.TopRight}";

            if (CornerRadius.TopRight > 0)
            {
                path += $" A{CornerRadius.TopRight} {CornerRadius.TopRight} 0 0 0 {right - CornerRadius.TopRight} {top}";
            }

            path += $" L{endPoint.X} {endPoint.Y}";

            _borderPart.Data = PathGeometry.Parse(path);
            _borderPart.StrokeThickness = new List<double>()
                    { BorderThickness.Right, BorderThickness.Bottom, BorderThickness.Left, BorderThickness.Top }
                .Average();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return base.MeasureOverride(availableSize);
    }
}
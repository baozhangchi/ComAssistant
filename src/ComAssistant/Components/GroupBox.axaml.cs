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
        //_headerPart = this.FindControl<ContentControl>("PART_Header");
        //_borderPart = this.FindControl<Path>("PART_Border");
        //_rootPart = this.FindControl<Grid>("PART_Root");
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        if (_headerPart != null && _rootPart != null)
        {
            var top = (_headerPart.Bounds.Bottom - _headerPart.Bounds.Top) / 2;
            var left = Margin.Left;
            var bottom = this.Bounds.Bottom - Margin.Bottom;
            var right = this.Bounds.Right - Margin.Right;
            var startPoint = new Point(_headerPart.Bounds.Left, top);
            var endPoint = new Point(_headerPart.Bounds.Right, top);
            var pathFigure = new PathFigure { StartPoint = startPoint, Segments = new PathSegments(), IsClosed = false, IsFilled = false };
            pathFigure.Segments.Add(new LineSegment() { Point = new Point(CornerRadius.TopLeft, top) });
            pathFigure.Segments.Add(new ArcSegment() { Point = new Point(left, CornerRadius.TopLeft + top), Size = new Size(CornerRadius.TopLeft, CornerRadius.TopLeft), IsLargeArc = false, RotationAngle = 90, SweepDirection = SweepDirection.Clockwise });
            //pathFigure.Segments.Add(new LineSegment() { Point = new Point(left, bottom - CornerRadius.BottomLeft) });
            //pathFigure.Segments.Add(new ArcSegment() { Point = new Point(left + CornerRadius.TopLeft, bottom), Size = new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft), IsLargeArc = false, RotationAngle = 90, SweepDirection = SweepDirection.CounterClockwise });
            //pathFigure.Segments.Add(new LineSegment() { Point = new Point(right - CornerRadius.BottomRight, bottom) });
            //pathFigure.Segments.Add(new ArcSegment() { Point = new Point(right, bottom - CornerRadius.BottomRight), Size = new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), IsLargeArc = false, RotationAngle = 90, SweepDirection = SweepDirection.CounterClockwise });
            //pathFigure.Segments.Add(new LineSegment() { Point = new Point(right, top + CornerRadius.TopRight) });
            //pathFigure.Segments.Add(new ArcSegment() { Point = new Point(right - CornerRadius.TopRight, top), Size = new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), IsLargeArc = false, RotationAngle = 90, SweepDirection = SweepDirection.CounterClockwise });
            //pathFigure.Segments.Add(new LineSegment() { Point = new Point(endPoint.X, top) });
            var geometry = new PathGeometry
            {
                Figures = new PathFigures { pathFigure }
            };
            if (_borderPart != null)
            {
                _borderPart.Data = geometry;
                _borderPart.StrokeThickness = new List<double>()
                        { BorderThickness.Right, BorderThickness.Bottom, BorderThickness.Left, BorderThickness.Top }
                    .Average();
            }
        }
    }
}
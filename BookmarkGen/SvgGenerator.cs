﻿using System.Text;

namespace BookmarkGen;

public record Vector2(float x, float y);
public record SvgVector2(float x, float y, string modifier) : Vector2(x,y);

public class SvgGenerator
{
    private const string generationNotice = "Generated by SVG Bookmark Generator";

    // Measurements are in *millimeters*
    private float width;
    private float minHeight;
    private float maxHeight;

    // Controls the rounding distance of corners of the rectangular bookmark
    // Also Defines the horizontal padding to add to both sides
    private float roundEdges;

    private string leftText;
    private string rightText;

    public SvgGenerator(float width, float minHeight, float maxHeight, float roundEdges, string left, string right) {
        this.width = width;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.roundEdges = roundEdges;
        this.leftText = left;
        this.rightText = right;
    }

    public string GenerateBookmark(float[] values) {
        return SvgString(values);
    }

    public string GetPathBookmark(float[] values) {
        List<SvgVector2> points = new();
        var heights = ConvertValuesToHeights(values);
        var barsWidth = width - roundEdges * 2;
        var barWidthStep = barsWidth / heights.Length;
        var x = roundEdges;
        InitPoint(points, new Vector2(0, heights[0]-roundEdges));
        AddBezier(points, new Vector2(0, heights[0]-roundEdges), new Vector2(0,heights[0]), new Vector2(roundEdges,heights[0]));
        foreach(var height in heights) {
            AddLine(points, new Vector2(x, height), new Vector2(x+barWidthStep, height));
            x += barWidthStep;
        }
        AddBezier(points, new Vector2(x, heights.Last()), new Vector2(width, heights.Last()), new Vector2(width, heights.Last()-roundEdges));
        AddLine(points, new Vector2(width, heights.Last()-roundEdges), new Vector2(width, roundEdges));
        AddBezier(points, new Vector2(width, roundEdges), new Vector2(width, 0), new Vector2(width - roundEdges, 0));
        AddLine(points, new Vector2(width - roundEdges, 0), new Vector2(roundEdges, 0));
        AddBezier(points, new Vector2(roundEdges, 0), new Vector2(0, 0), new Vector2(0, roundEdges));
        AddLine(points, new Vector2(0, roundEdges), new Vector2(0, heights[0]-roundEdges));
        
        StringBuilder pathDataStringBuilder = new();
        foreach(var point in points) {
            pathDataStringBuilder.Append($"{PointToString(point)} ");
        }
        return pathDataStringBuilder.ToString().Trim();
    }

    public string GetPathBackground() {
        List<SvgVector2> points = new();
        InitPoint(points, new Vector2(0, maxHeight-roundEdges));
        AddBezier(points, new Vector2(0, maxHeight-roundEdges), new Vector2(0,maxHeight), new Vector2(roundEdges,maxHeight));
        AddLine(points, new Vector2(roundEdges,maxHeight), new Vector2(width-roundEdges, maxHeight));
        AddBezier(points, new Vector2(width-roundEdges, maxHeight), new Vector2(width, maxHeight), new Vector2(width, maxHeight-roundEdges));
        AddLine(points, new Vector2(width, maxHeight-roundEdges), new Vector2(width, roundEdges));
        AddBezier(points, new Vector2(width, roundEdges), new Vector2(width, 0), new Vector2(width - roundEdges, 0));
        AddLine(points, new Vector2(width - roundEdges, 0), new Vector2(roundEdges, 0));
        AddBezier(points, new Vector2(roundEdges, 0), new Vector2(0, 0), new Vector2(0, roundEdges));
        AddLine(points, new Vector2(0, roundEdges), new Vector2(0, maxHeight-roundEdges));
        
        StringBuilder pathDataStringBuilder = new();
        foreach(var point in points) {
            pathDataStringBuilder.Append($"{PointToString(point)} ");
        }
        return pathDataStringBuilder.ToString().Trim();
    }

    public string SvgString(float[] values) {
        return @$"<!-- {generationNotice} -->
<svg viewBox=""0 0 {width} {maxHeight}""
     width=""{width}mm""
     height=""{maxHeight}mm""
     style=""font-size: {minHeight/10}mm; vertical-align: middle; font-family: 'Cascadia Code'""
     xmlns=""http://www.w3.org/2000/svg"">
    <path d=""{GetPathBackground()}""
            fill=""none"" stroke=""black"" />
    <path d=""{GetPathBookmark(values)}""
            fill=""none"" stroke=""black"" />
    <text x=""{roundEdges}"" y=""{minHeight/2}"" alignment-baseline=""middle"" style=""font-weight: bolder"">{leftText}</text>
    <text x=""{width-roundEdges}"" y=""{minHeight/2}"" alignment-baseline=""middle"" text-anchor=""end"">{rightText}</text>
</svg>
";
    }

    public string PointToString(SvgVector2 vector) {
        return $"{vector.modifier}{vector.x},{vector.y}";
    }

    protected void InitPoint(IList<SvgVector2> vectors, Vector2 point) {
        vectors.Add(new SvgVector2(point.x, point.y, "M"));
    }

    protected void AddBezier(IList<SvgVector2> vectors, Vector2 start, Vector2 modifier, Vector2 end) {
        vectors.Add(new SvgVector2(start.x, start.y, "C"));
        vectors.Add(new SvgVector2(modifier.x, modifier.y, string.Empty));
        vectors.Add(new SvgVector2(end.x, end.y, string.Empty));
    }

    protected void AddLine(IList<SvgVector2> vectors, Vector2 start, Vector2 end) {
        vectors.Add(new SvgVector2(start.x, start.y, "L"));
        vectors.Add(new SvgVector2(end.x, end.y, string.Empty));
    }

    public float[] ConvertValuesToHeights(float[] values) {
        float max = values.Max();
        return values.Select(value => Lerp(minHeight, maxHeight, value/max)).ToArray();
    }

    // Linear Interpolation between point a and b
    private float Lerp(float a, float b, float amount) {
        return a + amount * (b - a);
    }
}
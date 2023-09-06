using Plotly.Blazor;

namespace Squill.Data;

public abstract class GraphProvider
{
    public PlotlyChart Chart { get; set; }
    public Config Config { get; set; } = new Config
    {
        Responsive = true,
    };
    public Layout Layout { get; set; } = new Layout
    {
        PaperBgColor = "#222222",
        PlotBgColor = "#333333",
        Font = new Plotly.Blazor.LayoutLib.Font
        {
            Family = "Gravitas One",
            Color = "#fff",
            Size = 16,
        },
        AutoSize = true,
    };
    public abstract IList<ITrace> Data { get; }
    protected abstract int Height { get; }
}
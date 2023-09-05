using Plotly.Blazor;

namespace Squill.Data;

public abstract class GraphProvider
{
    public PlotlyChart Chart { get; set; }
    public Config Config { get; set; }
    public Layout Layout { get; set; }
    public abstract IList<ITrace> Data { get; }
}
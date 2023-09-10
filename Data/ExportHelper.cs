
using HtmlRendererCore.PdfSharp;
using PdfSharpCore.Pdf;
using Squill.Data.Elements;
using System.Reflection.Metadata;
using System.Text;

namespace Squill.Data;

public static class ExportHelper
{
    public static string CssStyle => File.ReadAllText("export.css");

    public enum eExportFormat
    {
        HTML, PDF
    }

    public static async Task Export(ProjectSession session, ElementMetaData meta, Manuscript manuscript, eExportFormat format)
    {
        switch (format)
        {
            case eExportFormat.HTML:
                await ExportHTML(session, meta, manuscript);
                break;
            case eExportFormat.PDF:
                await ExportPDF(session, meta, manuscript);
                break;
        }
    }

    private static async Task ExportPDF(ProjectSession session, ElementMetaData meta, Manuscript manuscript)
    {
        var document = new PdfDocument();
        document.Info.Title = meta.Name;

        var css = PdfGenerator.ParseStyleSheet(CssStyle);
        for (int i = 0; i < manuscript.Chapters.Count; i++)
        {
            string? chapter = manuscript.Chapters[i];
            var sb = new StringBuilder();
            var chapterMeta = session.GetMetaData(Guid.Parse(chapter));
            var content = session.GetElement<Chapter>(chapterMeta);

            var contentHTML = Markdig.Markdown.ToHtml(content.Content)
                .Replace("\n\n", "\n");

            sb.AppendLine($"<h1>Chapter {i + 1}: {chapterMeta.Name}</h1>");
            sb.AppendLine(contentHTML);

            PdfGenerator.AddPdfPages(
                document,
                sb.ToString(),
                PdfSharpCore.PageSize.A5,
                cssData: css);
        }
        document.Save($"{meta.Name}.pdf");
    }

    private static async Task ExportHTML(ProjectSession session, ElementMetaData meta, Manuscript manuscript)
    {
        var sb = new StringBuilder();
        sb.Append($"<style>{CssStyle}</style>");
        sb.Append("<body>");
        for (int i = 0; i < manuscript.Chapters.Count; i++)
        {
            string? chapter = manuscript.Chapters[i];
            var chapterMeta = session.GetMetaData(Guid.Parse(chapter));
            var content = session.GetElement<Chapter>(chapterMeta);

            var contentHTML = Markdig.Markdown.ToHtml(content.Content)
                .Replace("\n\n", "\n");

            sb.AppendLine($"<h2>Chapter {i + 1}</h2>");
            sb.AppendLine($"<h1>{chapterMeta.Name}</h1>");
            sb.AppendLine(contentHTML);
        }
        sb.Append("</body>");
        File.WriteAllText($"{meta.Name}.html", sb.ToString());
    }
}

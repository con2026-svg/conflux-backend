namespace ConFlux.Pdf
{
    using ConFlux.DTOs;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

    public class ReportDocument : IDocument
    {
        private readonly ReportDto _data;
        private readonly byte[]? _logoBytes;

        public ReportDocument(ReportDto data, byte[]? logoBytes)
        {
            _data = data;
            _logoBytes = logoBytes;
        }

        // Globalni stil (font sa ćirilicom)
        static ReportDocument()
        {
            // Ako koristiš custom font za ćirilicu:
            // QuestPDF.Settings.License = LicenseType.Community;
            // FontManager.RegisterFont(File.OpenRead("Resources/Fonts/DejaVuSans.ttf"));
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Strana ");
                    txt.CurrentPageNumber();
                    txt.Span(" / ");
                    txt.TotalPages();
                });
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("ConFlux izveštaj")
                        .FontSize(20).SemiBold();
                    col.Item().Text($"ID: {_data.Id}").FontSize(10).Light();
                    col.Item().Text($"Korisnik: {_data.Username}").FontSize(10).Light();
                    col.Item().Text($"Datum: {_data.CreatedAt:dd.MM.yyyy. HH:mm}").FontSize(10).Light();
                });

                if (_logoBytes is not null)
                {
                    row.ConstantItem(80).Image(_logoBytes);
                }
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(col =>
            {
                col.Spacing(10);

                col.Item().Element(ComposeSummaryCard);
                col.Item().Element(ComposeKeyValuesTable);

                if (!string.IsNullOrWhiteSpace(_data.AiDescription))
                {
                    col.Item().PaddingTop(10).Text("AI opis")
                        .FontSize(14).SemiBold();
                    col.Item().Element(ComposeAiBox);
                }

                if (!string.IsNullOrWhiteSpace(_data.UserNote))
                {
                    col.Item().PaddingTop(10).Text("Napomena korisnika")
                        .FontSize(12).SemiBold();
                    col.Item().Background(Colors.Grey.Lighten4)
                        .Padding(10)
                        .Text(_data.UserNote!)
                        .FontSize(10);
                }
            });
        }

        void ComposeSummaryCard(IContainer container)
        {
            container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Background(Colors.White)
                .Padding(12)
                .Column(col =>
                {
                    col.Spacing(6);
                    col.Item().Text("Sažetak parametara").SemiBold().FontSize(12);
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Površina: {_data.M2} m²").FontSize(10);
                        row.RelativeItem().Text($"Region: {_data.Region}").FontSize(10);
                        row.RelativeItem().Text($"Kategorija: {_data.Category}").FontSize(10);
                        row.RelativeItem().Text($"Struktura: {_data.Structure}").FontSize(10);
                    });
                });
        }

        void ComposeKeyValuesTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(2);
                });

                table.Header(h =>
                {
                    h.Cell().Element(Th).Text("Polje");
                    h.Cell().Element(Th).Text("Vrednost");

                    static IContainer Th(IContainer c) => c
                        .DefaultTextStyle(x => x.SemiBold().FontSize(10))
                        .Background(Colors.Grey.Lighten3)
                        .Padding(6);
                });

                foreach (var (label, value) in _data.KeyValues)
                {
                    table.Cell().Element(Td).Text(label);
                    table.Cell().Element(Td).Text(value);

                    static IContainer Td(IContainer c) => c
                        .DefaultTextStyle(x => x.FontSize(10))
                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                        .Padding(6);
                }
            });
        }

        void ComposeAiBox(IContainer container)
        {
            container
                .Border(1).BorderColor(Colors.Blue.Lighten2)
                .Background(Colors.Blue.Lighten5)
                .Padding(12)
                .Text(_data.AiDescription!)
                .FontSize(10);
        }
    }
}
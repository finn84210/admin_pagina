from pathlib import Path
from xml.sax.saxutils import escape


OUT = Path(__file__).parent
DRAWIO = OUT / "matrix-inc-backoffice-interface.drawio"
GUIDE = OUT / "drawio-import-guide.md"


STYLE_FRAME = "rounded=1;whiteSpace=wrap;html=1;fillColor=#f4f7fa;strokeColor=#d9e0e8;fontFamily=Segoe UI;"
STYLE_NAV = "rounded=0;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#d9e0e8;fontFamily=Segoe UI;"
STYLE_TITLE = "text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=middle;fontFamily=Segoe UI;fontSize=28;fontStyle=1;fontColor=#0B2545;"
STYLE_SUB = "text;html=1;strokeColor=none;fillColor=none;align=left;verticalAlign=middle;fontFamily=Segoe UI;fontSize=13;fontColor=#5F6B7A;"
STYLE_CARD = "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#d9e0e8;fontFamily=Segoe UI;arcSize=8;"
STYLE_HERO = "rounded=1;whiteSpace=wrap;html=1;fillColor=#eef5fb;strokeColor=#d9e0e8;fontFamily=Segoe UI;arcSize=8;"
STYLE_PRIMARY = "rounded=1;whiteSpace=wrap;html=1;fillColor=#2E74B5;strokeColor=#2E74B5;fontColor=#ffffff;fontFamily=Segoe UI;fontStyle=1;arcSize=12;"
STYLE_SECONDARY = "rounded=1;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#d9e0e8;fontColor=#1F4D78;fontFamily=Segoe UI;fontStyle=1;arcSize=12;"
STYLE_SUCCESS = "rounded=1;whiteSpace=wrap;html=1;fillColor=#198754;strokeColor=#198754;fontColor=#ffffff;fontFamily=Segoe UI;fontStyle=1;arcSize=12;"
STYLE_DANGER = "rounded=1;whiteSpace=wrap;html=1;fillColor=#DC3545;strokeColor=#DC3545;fontColor=#ffffff;fontFamily=Segoe UI;fontStyle=1;arcSize=12;"
STYLE_BADGE_WARN = "rounded=1;whiteSpace=wrap;html=1;fillColor=#FFD76A;strokeColor=#FFD76A;fontColor=#0B2545;fontFamily=Segoe UI;fontStyle=1;arcSize=50;"
STYLE_BADGE_OK = "rounded=1;whiteSpace=wrap;html=1;fillColor=#198754;strokeColor=#198754;fontColor=#ffffff;fontFamily=Segoe UI;fontStyle=1;arcSize=50;"
STYLE_TABLE_HEADER = "rounded=0;whiteSpace=wrap;html=1;fillColor=#E8EEF5;strokeColor=#d9e0e8;fontFamily=Segoe UI;fontStyle=1;fontColor=#1F4D78;"
STYLE_CELL = "rounded=0;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#d9e0e8;fontFamily=Segoe UI;fontSize=12;"
STYLE_NOTE = "rounded=1;whiteSpace=wrap;html=1;fillColor=#F2F4F7;strokeColor=#d9e0e8;fontFamily=Segoe UI;arcSize=8;"


class Page:
    def __init__(self, name):
        self.name = name
        self.cells = []
        self.next_id = 2

    def add(self, value, style, x, y, w, h, parent=1):
        cid = str(self.next_id)
        self.next_id += 1
        self.cells.append(
            f'<mxCell id="{cid}" value="{escape(value)}" style="{style}" vertex="1" parent="{parent}">'
            f'<mxGeometry x="{x}" y="{y}" width="{w}" height="{h}" as="geometry"/></mxCell>'
        )
        return cid

    def edge(self, source, target, label=""):
        cid = str(self.next_id)
        self.next_id += 1
        style = "edgeStyle=orthogonalEdgeStyle;rounded=1;orthogonalLoop=1;jettySize=auto;html=1;strokeColor=#2E74B5;fontFamily=Segoe UI;"
        self.cells.append(
            f'<mxCell id="{cid}" value="{escape(label)}" style="{style}" edge="1" parent="1" source="{source}" target="{target}">'
            '<mxGeometry relative="1" as="geometry"/></mxCell>'
        )

    def base(self, active):
        self.add("", STYLE_FRAME, 0, 0, 1366, 850)
        self.add("Matrix Inc.", "text;html=1;strokeColor=none;fillColor=none;fontFamily=Segoe UI;fontSize=18;fontStyle=1;fontColor=#0B2545;", 32, 18, 140, 28)
        self.add("", STYLE_NAV, 0, 0, 1366, 64)
        items = ["Home", "Customers", "Admin", "Products", "Privacy", "Orders"]
        x = 220
        for item in items:
            style = "rounded=1;whiteSpace=wrap;html=1;fillColor=#eef5fb;strokeColor=none;fontFamily=Segoe UI;fontStyle=1;fontColor=#1F4D78;" if item == active else "text;html=1;strokeColor=none;fillColor=none;fontFamily=Segoe UI;fontSize=13;fontColor=#5F6B7A;"
            self.add(item, style, x, 16, 90, 32)
            x += 98

    def xml(self):
        body = "\n".join([
            '<mxCell id="0"/>',
            '<mxCell id="1" parent="0"/>',
            *self.cells,
        ])
        return (
            f'<diagram name="{escape(self.name)}">'
            f'<mxGraphModel dx="1422" dy="794" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="1400" pageHeight="900" math="0" shadow="0">'
            f'<root>{body}</root></mxGraphModel></diagram>'
        )


def add_button(page, label, x, y, w=150, variant="primary"):
    style = {"primary": STYLE_PRIMARY, "secondary": STYLE_SECONDARY, "success": STYLE_SUCCESS, "danger": STYLE_DANGER}[variant]
    return page.add(label, style, x, y, w, 42)


def add_table(page, x, y, headers, rows, widths):
    cx = x
    for header, width in zip(headers, widths):
        page.add(header, STYLE_TABLE_HEADER, cx, y, width, 42)
        cx += width
    for r, row in enumerate(rows):
        cx = x
        yy = y + 42 + r * 54
        for value, width in zip(row, widths):
            page.add(value, STYLE_CELL, cx, yy, width, 54)
            cx += width


def home_page():
    p = Page("01 Home - navigatie")
    p.base("Home")
    p.add("Back-office overzicht", STYLE_TITLE, 48, 102, 520, 44)
    p.add("Navigeer snel naar dashboard, producten, orders en klanten.", STYLE_SUB, 48, 144, 540, 26)
    p.add("Matrix Inc. Admin&lt;br&gt;&lt;br&gt;&lt;b&gt;Rustige startpagina voor dagelijks beheer&lt;/b&gt;&lt;br&gt;&lt;br&gt;Primaire routes staan direct zichtbaar op de pagina.", STYLE_HERO, 48, 200, 820, 240)
    add_button(p, "Open dashboard", 80, 370, 170)
    add_button(p, "Productbeheer", 268, 370, 150, "secondary")
    p.add("&lt;b&gt;Producten&lt;/b&gt;&lt;br&gt;CRUD&lt;hr&gt;&lt;b&gt;Orders&lt;/b&gt;&lt;br&gt;Historie&lt;hr&gt;&lt;b&gt;Voorraad&lt;/b&gt;&lt;br&gt;Signalering", STYLE_CARD, 920, 200, 300, 240)
    cards = [
        ("Dashboard", "Admin overzicht", "Totaalproducten en lage voorraad."),
        ("Producten", "Productbeheer", "Zoeken, filteren, toevoegen en bewerken."),
        ("Orders", "Orderhistorie", "Klantorders terugvinden."),
        ("Klanten", "Klantenbeheer", "Klanten bekijken en beheren."),
    ]
    ids = []
    for i, (kicker, title, body) in enumerate(cards):
        x = 48 + i * 304
        ids.append(p.add(f"{kicker.upper()}&lt;br&gt;&lt;br&gt;&lt;b&gt;{title}&lt;/b&gt;&lt;br&gt;{body}", STYLE_CARD, x, 500, 280, 170))
    p.add("&lt;b&gt;Gebruikelijke workflow&lt;/b&gt;&lt;br&gt;Dashboard controleren -&gt; Producten bijwerken -&gt; Orders raadplegen", STYLE_NOTE, 48, 730, 860, 70)
    add_button(p, "Nieuw product", 1040, 744, 160, "success")
    return p


def dashboard_page():
    p = Page("02 Admin dashboard")
    p.base("Admin")
    p.add("Admin dashboard", STYLE_TITLE, 48, 102, 420, 44)
    p.add("Beheer producten en houd voorraad snel in de gaten.", STYLE_SUB, 48, 144, 520, 26)
    add_button(p, "Producten bekijken", 960, 106, 180)
    add_button(p, "Nieuw product", 1160, 106, 140, "secondary")
    p.add("Totaal producten&lt;br&gt;&lt;br&gt;&lt;font style='font-size:42px'&gt;&lt;b&gt;3&lt;/b&gt;&lt;/font&gt;", STYLE_CARD, 48, 210, 290, 150)
    p.add("Lage voorraad&lt;br&gt;&lt;br&gt;&lt;font style='font-size:42px'&gt;&lt;b&gt;2&lt;/b&gt;&lt;/font&gt;", "rounded=1;whiteSpace=wrap;html=1;fillColor=#FFF8E1;strokeColor=#FFD76A;fontFamily=Segoe UI;arcSize=8;", 370, 210, 290, 150)
    p.add("&lt;b&gt;Beheertaken&lt;/b&gt;&lt;br&gt;&lt;br&gt;1. Controleer lage voorraad&lt;br&gt;2. Werk productinformatie bij&lt;br&gt;3. Raadpleeg orders bij klantvragen", STYLE_CARD, 48, 430, 560, 260)
    p.add("&lt;b&gt;HCI onderbouwing&lt;/b&gt;&lt;br&gt;&lt;br&gt;Het dashboard toont alleen de belangrijkste signalen. Hierdoor ziet de gebruiker snel waar actie nodig is.", STYLE_NOTE, 680, 430, 520, 260)
    return p


def product_page():
    p = Page("03 Productbeheer")
    p.base("Products")
    p.add("Productbeheer", STYLE_TITLE, 48, 102, 420, 44)
    p.add("Zoek, filter en beheer producten van Matrix Inc.", STYLE_SUB, 48, 144, 520, 26)
    add_button(p, "Nieuw product", 1080, 106, 160)
    p.add("Productnaam", STYLE_SUB, 76, 205, 120, 24)
    p.add("Zoek op naam", STYLE_CARD, 76, 232, 300, 42)
    p.add("☐ Alleen lage voorraad", "text;html=1;strokeColor=none;fillColor=none;fontFamily=Segoe UI;fontSize=14;fontColor=#0B2545;", 420, 242, 200, 26)
    add_button(p, "Filter", 660, 232, 100)
    add_button(p, "Reset", 776, 232, 90, "secondary")
    p.add("3 resultaten | 2 van 3 producten hebben lage voorraad", STYLE_SUB, 48, 318, 520, 28)
    add_table(
        p,
        48,
        360,
        ["Naam", "Omschrijving", "Prijs", "Voorraad", "Acties"],
        [
            ["Nebuchadnezzar", "Schip uit Matrix", "EUR 10000", "3 laag", "Details | Edit | Delete"],
            ["Jack-in Chair", "Stoel om in te pluggen", "EUR 500,50", "12", "Details | Edit | Delete"],
            ["EMP Device", "Wapentuig", "EUR 129,99", "4 laag", "Details | Edit | Delete"],
        ],
        [210, 410, 150, 150, 260],
    )
    p.add("Voorraad &lt; 5 wordt als badge of waarschuwing getoond.", STYLE_NOTE, 48, 620, 520, 80)
    return p


def orders_page():
    p = Page("04 Orderhistorie")
    p.base("Orders")
    p.add("Orderhistorie", STYLE_TITLE, 48, 102, 420, 44)
    p.add("Alle orders van alle klanten in een overzicht.", STYLE_SUB, 48, 144, 520, 26)
    p.add("4 orders", "rounded=1;whiteSpace=wrap;html=1;fillColor=#6C757D;strokeColor=#6C757D;fontColor=#ffffff;fontFamily=Segoe UI;fontStyle=1;arcSize=50;", 1100, 108, 100, 34)
    p.add("Klant", STYLE_SUB, 76, 205, 120, 24)
    p.add("Alle klanten", STYLE_CARD, 76, 232, 200, 42)
    p.add("Zoeken", STYLE_SUB, 312, 205, 120, 24)
    p.add("Ordernr, klant of adres", STYLE_CARD, 312, 232, 260, 42)
    p.add("Vanaf", STYLE_SUB, 604, 205, 120, 24)
    p.add("Datum", STYLE_CARD, 604, 232, 140, 42)
    p.add("Tot en met", STYLE_SUB, 770, 205, 120, 24)
    p.add("Datum", STYLE_CARD, 770, 232, 140, 42)
    add_button(p, "Filter", 950, 232, 94)
    add_button(p, "Reset", 1060, 232, 90, "secondary")
    add_table(
        p,
        48,
        340,
        ["Ordernummer", "Orderdatum", "Klant", "Adres", "Status", "Producten"],
        [
            ["#4", "01-03-2021", "Trinity", "789 Pine St", "Actief", "Geen producten gekoppeld"],
            ["#3", "01-02-2021", "Morpheus", "456 Oak St", "Actief", "Geen producten gekoppeld"],
            ["#2", "01-02-2021", "Neo", "123 Elm St", "Actief", "Geen producten gekoppeld"],
            ["#1", "01-01-2021", "Neo", "123 Elm St", "Actief", "Geen producten gekoppeld"],
        ],
        [150, 150, 160, 220, 120, 300],
    )
    return p


def hci_page():
    p = Page("05 HCI en design system")
    p.add("HCI en design system", STYLE_TITLE, 48, 60, 520, 44)
    p.add("Onderbouwing voor het interface design.", STYLE_SUB, 48, 102, 520, 26)
    p.add("&lt;b&gt;Zichtbaarheid&lt;/b&gt;&lt;br&gt;Belangrijke routes staan op de homepagina en in de navigatie.", STYLE_CARD, 48, 170, 360, 130)
    p.add("&lt;b&gt;Consistentie&lt;/b&gt;&lt;br&gt;Knoppen, tabellen, filters en kaarten gebruiken vaste patronen.", STYLE_CARD, 438, 170, 360, 130)
    p.add("&lt;b&gt;Feedback&lt;/b&gt;&lt;br&gt;Badges tonen status zoals lage voorraad of actieve klanten.", STYLE_CARD, 828, 170, 360, 130)
    p.add("&lt;b&gt;Foutpreventie&lt;/b&gt;&lt;br&gt;Verwijderen gebeurt via een aparte bevestigingspagina.", STYLE_CARD, 48, 330, 360, 130)
    p.add("&lt;b&gt;Responsive ontwerp&lt;/b&gt;&lt;br&gt;Op mobiel stapelen kaarten, filters en knoppen onder elkaar.", STYLE_CARD, 438, 330, 360, 130)
    p.add("&lt;b&gt;Kleuren&lt;/b&gt;&lt;br&gt;Primary #2E74B5&lt;br&gt;Ink #0B2545&lt;br&gt;Muted #5F6B7A&lt;br&gt;Border #D9E0E8&lt;br&gt;Warning #FFD76A", STYLE_NOTE, 828, 330, 360, 190)
    add_button(p, "Primair", 48, 560, 120)
    add_button(p, "Secundair", 188, 560, 130, "secondary")
    add_button(p, "Opslaan", 338, 560, 120, "success")
    add_button(p, "Verwijderen", 478, 560, 150, "danger")
    p.add("Lage voorraad", STYLE_BADGE_WARN, 48, 640, 130, 32)
    p.add("Actief", STYLE_BADGE_OK, 198, 640, 90, 32)
    return p


def build():
    pages = [home_page(), dashboard_page(), product_page(), orders_page(), hci_page()]
    xml = [
        '<mxfile host="app.diagrams.net" modified="2026-06-04T00:00:00.000Z" agent="Codex" version="24.7.17" type="device">',
        *(page.xml() for page in pages),
        '</mxfile>',
    ]
    DRAWIO.write_text("\n".join(xml), encoding="utf-8")
    GUIDE.write_text(
        """# Draw.io ontwerp openen

Bestand: `matrix-inc-backoffice-interface.drawio`

Openen in Visual Studio Code:

1. Open de map `C:\\documentatie\\figma-design`.
2. Open `matrix-inc-backoffice-interface.drawio`.
3. De draw.io extensie toont vijf pagina's:
   - 01 Home - navigatie
   - 02 Admin dashboard
   - 03 Productbeheer
   - 04 Orderhistorie
   - 05 HCI en design system

Doel van het bestand:

- Interface design onderbouwen.
- Navigatie en schermopbouw tonen.
- HCI-principes zichtbaar maken.
- Schermen eventueel verder aanpassen in draw.io.
""",
        encoding="utf-8",
    )
    print(DRAWIO)


if __name__ == "__main__":
    build()

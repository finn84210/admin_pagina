# Interface design onderbouwing - Matrix Inc. Back-office

## Doel

Het ontwerp ondersteunt een admin back-office voor Matrix Inc. De gebruiker moet snel kunnen navigeren naar productbeheer, orderhistorie, klantenbeheer en dashboardinformatie.

## Ontwerpkeuzes

### Homepagina

De homepagina is ontworpen als startpunt voor dagelijks beheer. De primaire acties staan bovenaan: dashboard openen en productbeheer. Daaronder staan vier navigatiekaarten voor de belangrijkste routes.

### Admin dashboard

Het dashboard toont alleen de belangrijkste signalen: totaal aantal producten en lage voorraad. Dit voorkomt onnodige cognitieve belasting en helpt de gebruiker direct prioriteiten te zien.

### Productbeheer

Productbeheer gebruikt een tabel omdat producten vergelijkbare eigenschappen hebben: naam, omschrijving, prijs, voorraad en acties. De zoekbalk en lage-voorraadfilter staan boven de tabel, zodat de gebruiker eerst kan beperken en daarna kan handelen.

### Orderhistorie

Orderhistorie gebruikt filters voor klant, zoekterm en datumbereik. Dit sluit aan op de taak: snel een order terugvinden bij een klantvraag.

## HCI-principes

- Zichtbaarheid: belangrijke routes staan zichtbaar op de homepagina.
- Consistentie: knoppen, kaarten en tabellen gebruiken dezelfde visuele stijl.
- Feedback: status en lage voorraad worden met badges weergegeven.
- Foutpreventie: verwijderen blijft een aparte bevestigingsstap in de applicatie.
- Herkenbaarheid: tabellen en formulieren volgen bekende admin patronen.

## Design system

De interface gebruikt een rustige zakelijke kleurstelling met blauw als primaire actiekleur. De achtergrond is licht, kaarten zijn wit en randen zijn subtiel. Daardoor blijft de applicatie overzichtelijk en geschikt voor herhaald administratief gebruik.

## Responsive gedrag

Op desktop staan kaarten en filters naast elkaar. Op mobiel stapelen de elementen onder elkaar, zodat knoppen en invoervelden goed bereikbaar blijven.

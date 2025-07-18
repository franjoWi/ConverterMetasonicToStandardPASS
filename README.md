# Metasonic2PASS-OWL Konverter

Dieses Tool konvertiert Prozessmodelle aus der Metasonic Suite (.jpp-Dateien) in den PASS OWL-Exchange-Standard. Es wurde im Rahmen einer Bachelorarbeit an der Universität Münster entwickelt.

## Funktionen

- Einlesen von Metasonic-XML-Dateien
- Mapping der Modellelemente auf OWL-Strukturen
- Export als standardkonformes OWL-Modell
- Unterstützung für Subjekte, Zustände, Transitionen, Nachrichten u.v.m.

## Voraussetzungen

- `alps.net.api` Bibliothek (z. B. über NuGet oder lokal einbinden)

## Nutzung

```csharp
string sourcePath = @"Pfad\zu\modell.jpp";
string destinationPath = @"Pfad\zu\modell.owl";
string baseuri = "http://www.example.org/meinprozess";

var converter = new PASSConverterMetasonicToStandard(sourcePath);
converter.convertMetasonicModelToOWLStandard(destinationPath, baseuri);

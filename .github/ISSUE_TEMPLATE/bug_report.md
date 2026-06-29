---
name: Bug report
about: Report a reproducible defect in the truth-table project
title: "Bug: "
labels: bug
assignees: ""
---

## Beschreibung

Beschreibe den Fehler kurz und konkret.

Beispiel: Beim Aufruf von `dotnet run --project csharp/src/LogischeAusdrücke/LogischeAusdrücke.csproj -- tabelle` wird eine gueltige Eingabe nicht korrekt ausgewertet.

## Schritte zum Reproduzieren

1. Befehl ausfuehren: `...`
2. Eingabe verwenden: `...`
3. Ergebnis beobachten: `...`

## Erwartetes Verhalten

Beschreibe, was stattdessen passieren sollte.

## Tatsächliches Verhalten

Beschreibe die beobachtete Ausgabe oder Fehlermeldung.

## Akzeptanzkriterien

- [ ] Der Fehler ist mit einem automatisierten Test abgedeckt.
- [ ] Der Test schlaegt vor dem Fix fehl und ist nach dem Fix gruen.
- [ ] Die CI-Pipeline laeuft erfolgreich.

## Prioritaet

- [ ] Niedrig
- [ ] Mittel
- [ ] Hoch

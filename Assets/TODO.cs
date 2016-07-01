using System.Collections;
using UnityEngine;

public class TODO : MonoBehaviour
{
    /*
    + v1.2 01.07.2016
    *
    *
    *
    *
     * v1.1
     * TODO-Liste des Spiels; In welcher jedes der zukünftigen Spielelemente als Stichpukt plus Erläuterug aufgelistet ist.
     * Dabei gilt folgende Regel: Erst das aufs grundlegende reduzierte Game, dann das Hauptmenü und die Story planen.
     * Später dann in der selben reinfolge auch entwickeln.
     * Die einzelnen Elemente dieser Liste werden nach wichtigkeit sortiert und ebenso von oben nach unten abgearbeitet,
     * wobei folglich sichtbare fortschritte erkennbar und kleinschrittig abschließbar sind.
     * Wenn wir jetzt alles stückweise, erst nach und nach perfektionieren und dann erst weiterplanen,
     * dann verlieren wir die Objektivität auf unser Projekkt,
     * wodurch das Gesammtbild und die Möglichkeit auf dessen Erweiterung verloren geht.

    - kamera herein/herauszoomen je nach gravitation. smoothe übergänge beim zoomen, auch beim aprupten stoppen

    - Levelselection animieren

    - Datenspeicherung verstehen und einsetzen

    - Editorscript schrieben

    - GitHub nutzen

    - Den Spielablauf mit einfachen Kästchen durchplanen, sodass sich dannach eine passendere story um die sichtbare meachanik herum erfinden lässt.
        Dabei das Haupmenü und jegliche Eingangsanimationen sowie Kamerafahrten weglassen.

        a Implementiere die beiden Buttons für Mirror und Hold mit ausreichend vielen variablen Parametern
        b Verändere die Parameter beider Spielfunktionen und teste, ob eine Bewegung nach rechts oder oben spaßiger sein kann als der Fall nach unten.
          Setze dies mittels von außen einstellbarem Vorzeichenwechsel und Vektorverändrung an den Bewegungskräften um.

    - Füge ersten generischen Gegnertypen hinzu
        a Spikes als fallende Objekte, ausgelöst durch vertikalen Bewegungssensor
        b Geschütze mit linearen Geschossen
        c Geschütze mut verfolgenden Geschossen
        d Geschosstyp welcher die Funktionen der beiden Interaktionsflächen kurzzeitig auswechelt

     * To be continuted...
     * Changelog:
     * v1.0 Erstellen der Todo Liste - 12.12.2015
     * v1.1 Hinzufügen der Festgelegten Höhe für jedes Horizontstück - 23.12.2015
     * v1.1 Hinzufügen eines Verwischungseffektes bei Kamerafahrten - 23.12.2015
     * v1.2 Änderung von Prioritäten
     * v1.3
     *
     *  Helpfull Code Snippets:
     *
     * CameraMovement cm = gameObject.AddComponent<CameraMovement>();
     * cm.moveCamTo(target, 1F);
     *
     * Vector3 PlayerPOS = GameObject.Find("Player").transform.transform.position;
     *
     *
     *
     *
     *
     *
     *
     *
     *

     */
}
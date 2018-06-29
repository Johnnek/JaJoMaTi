using AntMe.Deutsch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//AntMe Jan, John, Marius und Tim

namespace AntMe.Player.JaJoMaTi
{
    /// <summary>
    /// Diese Datei enthält die Beschreibung für deine Ameise. Die einzelnen Code-Blöcke 
    /// (Beginnend mit "public override void") fassen zusammen, wie deine Ameise in den 
    /// entsprechenden Situationen reagieren soll. Welche Befehle du hier verwenden kannst, 
    /// findest du auf der Befehlsübersicht im Wiki (http://wiki.antme.net/de/API1:Befehlsliste).
    /// 
    /// Wenn du etwas Unterstützung bei der Erstellung einer Ameise brauchst, findest du
    /// in den AntMe!-Lektionen ein paar Schritt-für-Schritt Anleitungen.
    /// (http://wiki.antme.net/de/Lektionen)
    /// </summary>
    [Spieler(
        Volkname = "JaJoMaTi",   // Hier kannst du den Namen des Volkes festlegen
        Vorname = "JaJoMaTi",       // An dieser Stelle kannst du dich als Schöpfer der Ameise eintragen
        Nachname = "JaJoMaTi"       // An dieser Stelle kannst du dich als Schöpfer der Ameise eintragen
    )]

    //Unsere Verschiedenen Kasten; Angreifer & Appler

    //Angreifer, bereits angepasst
    [Kaste(
        Name = "Attacker",
        AngriffModifikator = 2,             // Angriffsstärke einer Ameise
        DrehgeschwindigkeitModifikator = -1, // Drehgeschwindigkeit einer Ameise
        EnergieModifikator = 2,             // Lebensenergie einer Ameise
        GeschwindigkeitModifikator = -1,     // Laufgeschwindigkeit einer Ameise
        LastModifikator = -1,                // Tragkraft einer Ameise
        ReichweiteModifikator = -1,          // Ausdauer einer Ameise
        SichtweiteModifikator = 0           // Sichtweite der Ameise
        )]

    //Apfelsammler, bereits angepasst
    [Kaste(
        Name = "Appler",
        AngriffModifikator = -1,             // Angriffsstärke einer Ameise
        DrehgeschwindigkeitModifikator = -1, // Drehgeschwindigkeit einer Ameise
        EnergieModifikator = -1,             // Lebensenergie einer Ameise
        GeschwindigkeitModifikator = 2,     // Laufgeschwindigkeit einer Ameise
        LastModifikator = 2,                // Tragkraft einer Ameise
        ReichweiteModifikator = -1,          // Ausdauer einer Ameise
        SichtweiteModifikator = 0           // Sichtweite der Ameise
        )]


    public class JaJoMaTiKlasse : Basisameise
    {
        #region Kasten

        private static bool erzeugeAppler = false;
        private static int attackerTyp = 1;
        private static int applerTyp = 1;

        /// <summary>
        /// Jedes mal, wenn eine neue Ameise geboren wird, muss ihre Berufsgruppe
        /// bestimmt werden. Das kannst du mit Hilfe dieses Rückgabewertes dieser 
        /// Methode steuern.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:BestimmeKaste
        /// </summary>
        /// <param name="anzahl">Anzahl Ameisen pro Kaste</param>
        /// <returns>Name der Kaste zu der die geborene Ameise gehören soll</returns>
        public override string BestimmeKaste(Dictionary<string, int> anzahl)
        {
            erzeugeAppler = ! erzeugeAppler;
            if (erzeugeAppler)
                return "Appler" + applerTyp;
            return "Attacker" + attackerTyp;
        }

        #endregion

        #region Fortbewegung

        /// <summary>
        /// Wenn die Ameise keinerlei Aufträge hat, wartet sie auf neue Aufgaben. Um dir das 
        /// mitzuteilen, wird diese Methode hier aufgerufen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:Wartet
        /// </summary>
        public override void Wartet()
        {
            if (Kaste == "Attacker")
            {
                // Ameise soll möglichst gut gestreut aber ziellos umherirren um möglichst
                // schnell Käfer zu finden.
                GeheGeradeaus(40);
                DreheUmWinkel(Zufall.Zahl(-10, 10));
            }                
            if (Kaste == "Appler")
            {
                // Sollte die Ameise außerhalb des Nahrungsmittelradiuses liegen...
                if (EntfernungZuBau > 400)
                {
                    // ... soll sie wieder heim gehen.
                    GeheZuBau();
                }
                else
                {
                    // ... ansonsten soll sie sich ein bischen drehen (zufälliger Winkel
                    // zwischen -10 und 10 Grad) und wieder ein paar Schritte laufen.
                    DreheUmWinkel(Zufall.Zahl(-10, 10));
                    GeheGeradeaus(20);
                }

                // Wenn die restliche verfügbare Strecke der Ameise (minus einem Puffer
                // von 50 Schritten) kleiner als die Entfernung zum Bau ist...
                if (Reichweite - ZurückgelegteStrecke - 50 < EntfernungZuBau)
                {
                    // ... soll sie nach Hause gehen um nicht zu sterben.
                    GeheZuBau();
                }
            }
        }

        /// <summary>
        /// Erreicht eine Ameise ein drittel ihrer Laufreichweite, wird diese Methode aufgerufen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:WirdM%C3%BCde
        /// </summary>
        public override void WirdMüde()
        {
            GeheZuBau();
        }

        /// <summary>
        /// Wenn eine Ameise stirbt, wird diese Methode aufgerufen. Man erfährt dadurch, wie 
        /// die Ameise gestorben ist. Die Ameise kann zu diesem Zeitpunkt aber keinerlei Aktion 
        /// mehr ausführen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:IstGestorben
        /// </summary>
        /// <param name="todesart">Art des Todes</param>
        public override void IstGestorben(Todesart todesart)
        {
            // Wir sterben nicht
        }

        /// <summary>
        /// Diese Methode wird in jeder Simulationsrunde aufgerufen - ungeachtet von zusätzlichen 
        /// Bedingungen. Dies eignet sich für Aktionen, die unter Bedingungen ausgeführt werden 
        /// sollen, die von den anderen Methoden nicht behandelt werden.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:Tick
        /// </summary>
        public override void Tick()
        {
            if (Kaste == "Attacker")
            {
                // Sollte die Ameise am Ende ihrer Reichweite sein (Abzüglich einem Puffer
                // und der Strecke die sie noch zum Bau zurücklegen muss) soll sie nach
                // Hause gehen um aufzuladen.
                if (Reichweite - ZurückgelegteStrecke - 100 <
                    EntfernungZuBau)
                {
                    GeheZuBau();
                }

                // Sollte eine Ameise durch den Kampf unter die 2/3-Marke ihrer Energie
                // fallen soll sie nach Hause gehen um aufzuladen.
                if (AktuelleEnergie < MaximaleEnergie * 2 / 3)
                {
                    GeheZuBau();
                }
            }
            if (Kaste == "Appler")
            {
                // Sollte die Ameise gerade mit Nahrung unterwegs sein...
                if (Ziel != null && GetragenesObst != null)
                {
                    // ... und noch Helfer für den Apfel gebraucht werden...
                    if (BrauchtNochTräger(GetragenesObst))
                    {
                        // ... soll sie eine Markierung sprühen die die Information enthält,
                        // wie viele Ameisen noch beim Tragen helfen sollen.
                        SprüheMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                    }
                }

                // Sollte die Ameise, wärend sie Obst trägt, das Ziel "Bau" verlieren,
                // wird das Ziel neu gesetzt.
                if (GetragenesObst != null)
                {
                    GeheZuBau();
                }

                // Sollte die Ameise einem Stück Obst hinterher laufen das garkeine Träger
                // mehr braucht soll sie stehen bleiben um anschließend durch "wartet"
                // wieder umher geschickt zu werden.
                if (Ziel is Obst && !BrauchtNochTräger((Obst)Ziel))
                {
                    BleibStehen();
                }
            }
        }

        #endregion

        #region Nahrung

        /// <summary>
        /// Sobald eine Ameise innerhalb ihres Sichtradius einen Apfel erspäht wird 
        /// diese Methode aufgerufen. Als Parameter kommt das betroffene Stück Obst.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:Sieht(Obst)"
        /// </summary>
        /// <param name="obst">Das gesichtete Stück Obst</param>
        public override void Sieht(Obst obst)
        {
            if (Kaste == "Attacker")
            {
                // get rekt
            }
            if (Kaste == "Appler")
            {
                // Sofern der Apfel noch Träger braucht soll die Ameise zum Apfel.
                if (BrauchtNochTräger(obst))
                {
                    GeheZuZiel(obst);
                }
            }
        }

        /// <summary>
        /// Sobald eine Ameise innerhalb ihres Sichtradius einen Zuckerhügel erspäht wird 
        /// diese Methode aufgerufen. Als Parameter kommt der betroffene Zuckerghügel.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:Sieht(Zucker)"
        /// </summary>
        /// <param name="zucker">Der gesichtete Zuckerhügel</param>
        public override void Sieht(Zucker zucker)
        {
            // Zuckerameisen sind Lappen!!               
        }

        /// <summary>
        /// Hat die Ameise ein Stück Obst als Ziel festgelegt, wird diese Methode aufgerufen, 
        /// sobald die Ameise ihr Ziel erreicht hat. Ab jetzt ist die Ameise nahe genug um mit 
        /// dem Ziel zu interagieren.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:ZielErreicht(Obst)"
        /// </summary>
        /// <param name="obst">Das erreichte Stück Obst</param>
        public override void ZielErreicht(Obst obst)
        {
            if (Kaste == "Attacker")
            {
                // get rekt
            }
            if (Kaste == "Appler")
            {
                // Die Ameise soll nochmal prüfen ob der Apfel überhaupt noch Träger
                // braucht.
                if (BrauchtNochTräger(obst))
                {
                    // Wenn noch Träger gebraucht werden soll die Ameise eine Markierung
                    // sprühen die als Information die Menge benötigter Ameisen hat. Da die
                    // benötigte Menge nicht genau ermittelt werden kann wird hier nur
                    // geschätzt. Es wird erwartet, dass 20 gebraucht werden und dass in
                    // "AnzahlInSichtweite" etwa die Zahl tragenden Ameisen steckt.
                    SprüheMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                    Nimm(obst);
                    GeheZuBau();
                }
            }
        }

        /// <summary>
        /// Hat die Ameise eine Zuckerhügel als Ziel festgelegt, wird diese Methode aufgerufen, 
        /// sobald die Ameise ihr Ziel erreicht hat. Ab jetzt ist die Ameise nahe genug um mit 
        /// dem Ziel zu interagieren.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:ZielErreicht(Zucker)"
        /// </summary>
        /// <param name="zucker">Der erreichte Zuckerhügel</param>
        public override void ZielErreicht(Zucker zucker)
        {
            // Dito!
        }
        

        #endregion

        #region Kommunikation

        /// <summary>
        /// Markierungen, die von anderen Ameisen platziert werden, können von befreundeten Ameisen 
        /// gewittert werden. Diese Methode wird aufgerufen, wenn eine Ameise zum ersten Mal eine 
        /// befreundete Markierung riecht.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:RiechtFreund(Markierung)"
        /// </summary>
        /// <param name="markierung">Die gerochene Markierung</param>
        public override void RiechtFreund(Markierung markierung)
        {
            if (Kaste == "Attacker")
            {
                // Die Ameise soll, sofern sie nicht schon ein Ziel wie "Käfer",
                // "Markierung" oder "Bau" hat auf direktem Weg zum Markierungsmittelpunkt
                // laufen von wo aus man hoffentlich weitere Markierungen oder direkt den
                // Käfer sieht.
                if (Ziel == null)
                {
                    GeheZuZiel(markierung);
                }
            }
            if (Kaste == "Appler")
            {
                // Sollte die Ameise nicht schon Obst im Auge haben oder auf dem Weg zum
                // Bau sein soll sie, wenn die angeforderte Menge Ameisen die Ameisenmenge
                // der gerade in Sichtweite befindlichen Ameisen übersteigt, zum
                // Markierungsmittelpunkt gehen um dort hoffentlich den Apfel zu sehen.
                if (!(Ziel is Obst) &&
                !(Ziel is Bau) &&
                AnzahlAmeisenInSichtweite < markierung.Information)
                {
                    GeheZuZiel(markierung);
                    // Sollte die Entfernung mehr als 50 schritte zum Mittelpunkt betragen,
                    // soll eine Folgemarkierung gesprüht werden um denn Effektradius zu 
                    // erhöhen.
                    if (Koordinate.BestimmeEntfernung(this, markierung) > 50)
                    {
                        SprüheMarkierung(
                            Koordinate.BestimmeRichtung(this, markierung),
                            Koordinate.BestimmeEntfernung(this, markierung));
                    }
                }
                else
                {
                    // In allen anderen Fällen soll sie kurz stehen bleiben um zu
                    // verhindern, dass die Ameise dem Apfel ewig hinterher läuft.
                    BleibStehen();
                }
            }
        }

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus dem eigenen Volk, so 
        /// wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFreund(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte befreundete Ameise</param>
        public override void SiehtFreund(Ameise ameise)
        {
            if (Kaste == "Attacker")
            {
                // get rekt
            }
            if (Kaste == "Appler")
            {
                // joooooo
            }
         }

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus einem befreundeten Volk 
        /// (Völker im selben Team), so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtVerb%C3%BCndeten(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte verbündete Ameise</param>
        public override void SiehtVerbündeten(Ameise ameise)
        {

        }

        #endregion

        #region Kampf

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus einem feindlichen Volk, 
        /// so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFeind(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte feindliche Ameise</param>
        public override void SiehtFeind(Ameise ameise)
        {
            if(Kaste == "Attacker")
            {
                // Feindliche Ameisen werden bedingungslos angegriffen!
                GreifeAn(ameise);
            }            
        }

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Wanze, so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFeind(Wanze)"
        /// </summary>
        /// <param name="wanze">Erspähte Wanze</param>
        public override void SiehtFeind(Wanze wanze)
        {
            if (Kaste == "Attacker")
            {
                // Wenn ein Käfer gesehen wird muss eine angemessen große Markierung
                // gesprüht werden. Ist diese Markierung zu klein kommt zu wenig Hilfe,
                // ist sie zu groß haben die weit entfernten Ameisen eine zu große Strecke
                // und kommen erst nach dem Kampf an.
                SprüheMarkierung(0, 150);
                GreifeAn(wanze);
            }
            if (Kaste == "Appler")
            {
                // Bei Käfersicht wird ermittelt ob die Ameise evtl. kollidiert, wenn sie
                // geradeaus weitergeht.
                int relativeRichtung =
                Koordinate.BestimmeRichtung(this, wanze) - Richtung;
                if (relativeRichtung > -15 && relativeRichtung < 15)
                {
                    // Wenn ja, soll sie erstmal die Nahrung fallen lassen um schneller zu
                    // laufen und dann, je nachdem auf welcher Seite der Käfer ist, in einem
                    // 20 Grad-Winkel in die andere Richtung weggehen.
                    LasseNahrungFallen();
                    if (relativeRichtung < 0)
                    {
                        DreheUmWinkel(20 + relativeRichtung);
                    }
                    else
                    {
                        DreheUmWinkel(-20 - relativeRichtung);
                    }
                    GeheGeradeaus(100);
                }
            }
        }

        /// <summary>
        /// Es kann vorkommen, dass feindliche Lebewesen eine Ameise aktiv angreifen. Sollte 
        /// eine feindliche Ameise angreifen, wird diese Methode hier aufgerufen und die 
        /// Ameise kann entscheiden, wie sie darauf reagieren möchte.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:WirdAngegriffen(Ameise)"
        /// </summary>
        /// <param name="ameise">Angreifende Ameise</param>
        public override void WirdAngegriffen(Ameise ameise)
        {
            
        }

        /// <summary>
        /// Es kann vorkommen, dass feindliche Lebewesen eine Ameise aktiv angreifen. Sollte 
        /// eine Wanze angreifen, wird diese Methode hier aufgerufen und die Ameise kann 
        /// entscheiden, wie sie darauf reagieren möchte.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:WirdAngegriffen(Wanze)"
        /// </summary>
        /// <param name="wanze">Angreifende Wanze</param>
        public override void WirdAngegriffen(Wanze wanze)
        {
            if(Kaste == "Attacker")
            {
                // Wenn der Käfer angreift: Zurückschlagen.
                GreifeAn(wanze);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace METRO
{
    class Metro
    {
        StreamReader menetrend = new StreamReader("METRO.txt"); //Fájlkezelés, txt beolvasása soronként
        public string[][] metro; //Kétdimenziós tömb a metro megállók neveinek
        public int[] db; //Tömb a vonalak megállóinak számára
        public int[][] át; //Kétdimenziós tömb az átszállási helyek indexeinek tárolására (pl: 0,1 cellában az 1-es vonalon lévő megálló indexe ami közös a 2-es vonallal)
        public void Feltölt() //A beolvasott fájlból feltöltjük a tömbeinket
        {
            while (!menetrend.EndOfStream) //A fájl végéig fut
            {
                db[0] = int.Parse(menetrend.ReadLine()); //a fájl sorának a tömbbe rakása
                metro[0] = new string[db[0]]; //a metro oszlopainak a hossza a db tömb elemének az értéke
                for (int i = 0; i < db[0]; i++) //metro tömb feltöltése a megállók neveivel
                {
                    metro[0][i] = menetrend.ReadLine();
                }
                db[1] = int.Parse(menetrend.ReadLine());
                metro[1] = new string[db[1]];
                for (int i = 0; i < db[1]; i++)
                {
                    metro[1][i] = menetrend.ReadLine();
                }
                db[2] = int.Parse(menetrend.ReadLine());
                metro[2] = new string[db[2]];
                for (int i = 0; i < db[2]; i++)
                {
                    metro[2][i] = menetrend.ReadLine();
                }
            }
            menetrend.Close(); //A beolvasott fájl bezárása
        }
        public int[][] Közös_megállók(int[] db, string[][] metro) //A vonalok közötti átszállóhelyeket adja vissza egy kétdimenziós tömbként
        {
            for (int ki = 0; ki < db.Length - 1; ki++)// az első vonaltól kezdi majd lefut a második vonalig (1 és 2 vonal, 1 és 3 vonal, 2 és 3 vonal)
            {
                for (int kj = ki + 1; kj < db.Length; kj++) //a második vonaltól kezdi és lefut a harmadik vonalig
                {
                    át[ki][kj] = 0; //azonos vonalak között nincs értelmezve ezért 0 az érték
                    át[kj][ki] = 0; //azonos vonalak között nincs értelmezve ezért 0 az érték
                    bool van = false; //eldöntés miatti változó
                    int i = 0;
                    while (i <= db[ki] && !van) //addig fut amig megnem lesz az összes közös megálló
                    {
                        int j = 0;
                        while (j < db[kj] && metro[ki][i] != metro[kj][j]) //egyesével nézzük meg a megállókat
                        {
                            j += 1;
                        }
                        if (j < db[kj]) //ha még az adott vonalon vagyunk és meglett az azonos megálló az indexek bekerülnek a tömbbe
                        {
                            át[ki][kj] = i;
                            át[kj][ki] = j;
                            van = true;
                        }
                        else //a keresett tömben a megállók léptetése
                        {
                            i += 1;
                        }
                    }
                }
            }
            return át; //visszaadja a tömböt
        }
    }
    class Utas
    {
        public string indulás; //a jelenlegi megálló
        public string cél; //a cél megálló
        public void Kiíratás(int i, int j, int k, int l, string[][] metro, int[] db) //Az útvonalterv kiíratása
        {
            Console.Write("A(z) " + (i + 1) + ". vonalon kell utaznia, "); //Az induló vonal kiírása
            if (j < l) //ha a kezdő állomás indexe kisebb mint a cél állomásé akkor tudjuk melyik irányba kell indulni
            {
                Console.Write(metro[i][db[i] - 1] + " végállomás felé, ");
            }
            else //különben a másik irényba
            {
                Console.Write(metro[i][0] + " végállomás felé, ");
            }
            Console.Write(Math.Abs(l - j) + " megállót!"); //amennyi megállót utazni kell
            Console.WriteLine();
        }
        public int Vonal_keresés(string állomás, string[][] metro, int[] db) //A beírt állomások vonalának indexát adja vissza
        {
            int i = 0;
            int j = 0;
            while (i<db.Length && metro[i][j] != állomás) //eldöntés tétel, addig nézzük amig meg nem találjuk a tömben a beírt állomást
            {
                j += 1;
                if (j >= db[i]) //ha már a következő állomás megállói jönnek akkor növeljük az indexeket
                {
                    i += 1;
                    j = 0;
                }
            }

            return i; //index visszaadása
        }
        public int Megálló_keresés(string állomás, string[][] metro, int[] db) //A beírt állomások megállójának indexát adja vissza
        {
            int i = 0;
            int j = 0;
            while (i<db.Length && metro[i][j] != állomás) //eldöntés tétel, addig nézzük amig meg nem találjuk a tömben a beírt állomást
            {
                j += 1;
                if (j >= db[i]) //ha már a következő állomás megállói jönnek akkor növeljük az indexeket
                {
                    i += 1;
                    j = 1;
                }
            }

            return j; //index visszaadása
        }
        public void Átszállás(Metro h, int i, int j, int k, int l, int[] db, string[][] metro) //Ha átszállás lesz majd akkor ez alapján írunk ki
        {
            if (metro[i][h.át[i][k]] == indulás) //azt vizsgáljuk, hogy az induló állomás egy közös megálló-e
            {
                Kiíratás(k, h.át[k][i], k, l, metro, db);
            }
            else
            {
                Kiíratás(i, j, i, h.át[i][k], metro, db);
            }
            if (metro[i][j] != metro[i][h.át[i][k]] && metro[k][l] != metro[i][h.át[i][k]]) //akkor lépünk be ide, ha a kezdő vagy cél állomás nem egy közös megálló
            {
                Kiíratás(k, h.át[k][i], k, l, metro, db);
            }
        }
        public void Útvonal(Metro h, string indulás, string cél, string[][] metro, int[] db) //Az útvonal megtervezése
        {
            int i = Vonal_keresés(indulás, metro, db);
            int j = Megálló_keresés(indulás, metro, db);
            int k = Vonal_keresés(cél, metro, db);
            int l = Megálló_keresés(cél, metro, db);
            if (i == k && j == l) //Ha a bekért állomások azonosak
            {
                Console.WriteLine("Nincs szükség tovább utazni!");
            }
            else if (i == k) //ha azonos vonalon vannak a bekért állomások
            {
                Kiíratás(i, j, k, l, metro, db);
            }
            else if (h.át[i][k] != 0) //ha az állomások között kell átszállás
            {
                Átszállás(h,i, j, k, l, db, metro);
            }
        }
    }
class Program
    {
        static void Main(string[] args)
        {
            Metro tervez = new Metro(); //osztály példányosítás
            Utas utazik = new Utas(); //osztály példányosítás
            tervez.metro = new string[3][]; //tömb sorainak száma
            tervez.db = new int[3]; //a tömb elemeinek száma
            tervez.át = new int[tervez.db.Length][]; //tömb sorainak száma 
            tervez.át[0] = new int[tervez.db.Length]; //oszlopok hossza
            tervez.át[1] = new int[tervez.db.Length];
            tervez.át[2] = new int[tervez.db.Length];
            tervez.Feltölt(); //metódus meghívása
            tervez.Közös_megállók(tervez.db, tervez.metro); //metódus meghívása
            Console.Write("Az induló állomás: ");
            utazik.indulás = Console.ReadLine(); //Bekérjük az állomást
            Console.Write("A cél állomás: ");
            utazik.cél = Console.ReadLine(); //Bekérjük az állomást
            utazik.Útvonal(tervez,utazik.indulás, utazik.cél, tervez.metro, tervez.db); //metódus meghívása
            Console.ReadLine();
        }
    }
}

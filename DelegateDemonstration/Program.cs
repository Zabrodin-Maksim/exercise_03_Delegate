using DelegateLib;
using System.Security.Cryptography.X509Certificates;

namespace Program;

class Program
{
    static void Main(string[] args)
    {
        Faculty faculty = Faculty.fei;
        StudentsTable students = new StudentsTable(20, ziskaniKlice);
        // TODO: Vytvořte demonstrační program ...
        IComparable ziskaniKlice(Student? student)
        {
            if (student != null) {
                return student.Number;
            }
            return 0;
        }

        // Vytvořte nekonečný cyklus ve kterém je uživateli zobrazeno menu:
        // MENU
        while (true)
        {
            try { 
                Console.WriteLine("\n1) Vlož studenta\n" +
                    "2) Dej studenta\n" +
                    "3) Smaž studenta\n" +
                    "4) Vypiš studenty\n" +
                    "5) Seřadit studenty podle jména\n" +
                    "6) Seřadit studenty podle čísla\n" +
                    "7) Seřadit studenty podle fakulty\n" +
                    "0) Konec programu\n" +
                    "Print number: ...");
                int menu = int.Parse(Console.ReadLine());
                switch (menu)
                {
                    case 1:
                        zadaniNewStudent();
                        break;

                    case 2:
                        dej();
                        break;

                    case 3:
                        smaz();
                        break;
                    case 4:
                        vypis();
                        break;
                    case 5:
                        SortJmena();
                        break;
                    case 6:
                        SortCisla();
                        break;
                    case 7:
                        SortFakulty();
                        break;
                    case 0:
                        exit();
                        return;

                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Wrong format!!!");
            }
        }



        // 1) Vlož studenta - následuje zadání hodnot studenta a jeho vložení do tabulky
        void zadaniNewStudent()
        {
            try { 
            Console.WriteLine("\nPrint NAME of student: ...");
            string name = Console.ReadLine();
                if(name == "") { throw new FormatException(); }
            Console.WriteLine("\nPrint Number of student: ...");
            int number = int.Parse(Console.ReadLine());
            Console.WriteLine("\nPrint faculty of student: ...");
            string pr = Console.ReadLine();
                if (name == "") { throw new FormatException(); }
                switch (pr)
                {
                case "fes":
                    faculty = Faculty.fes;
                    break;
                case "ff":
                    faculty = Faculty.ff;
                    break;
                case "fei":
                    faculty = Faculty.fei;
                    break;
                case "fcht":
                    faculty = Faculty.fcht;
                    break;
                case "dfjp":
                    faculty = Faculty.dfjp;
                    break;
                case "fzs":
                    faculty = Faculty.fzs;
                    break;
                case "fr":
                    faculty = Faculty.fr;
                    break;
                }
            vloz(name, number, faculty);
            }
            catch (FormatException)
            {
                Console.WriteLine("Wrong format!!!");
            }

        }
        void vloz(string? name, int number, Faculty faculty)
        {
            Student student = new Student(name, number,faculty);
            students.Add(student);
        }




        //2) Dej studenta - následuje zadání hodnoty Number a vyhledání studenta a jeho výpis do konzole
        void dej()
        {
            try { 
                Console.WriteLine("\nPrint number of student: ...");
                int number = int.Parse(Console.ReadLine());
                Console.WriteLine("\n" + students.Get(number).ToString());
            }
            catch(FormatException) {
                Console.WriteLine("Wrong format!!!");
            }
        }


        // 3) Smaž studenta - následuje zadání hodnoty Number a odstranění studenta a jeho výpis do konzole
        void smaz()
        {
            try
            {
                Console.WriteLine("\nPrint number of student: ...");
                int number = int.Parse(Console.ReadLine());
                Console.WriteLine("\n" + students.Delete(number).ToString());
            }
            catch(FormatException) {
                Console.WriteLine("Wrong format!!!");
            }
        }

        // 4) Vypiš studenty - vypíše všechny studenty do konzole
        void vypis()
        {
            for (int i = 0;i < students.Capacity; i++)
            {
                if (students[i] != null) {
                    Console.WriteLine(students[i].ToString());
                }
            }
        }

        bool CompareByName(Student a, Student b)
        {
            if (a == null) return false;
            if (b == null) return true;
            return a.Name.CompareTo(b.Name) <= 0;
        }

        // 5) Seřadit studenty podle jména - seřadí studenty podle jména (využijte metodu Sort a vhodnou porovnávací metodu)
        void SortJmena() {
            CompareStudentsCallback compare = CompareByName;
            students.Sort(compare);
        }

        bool CompareByNumber(Student a, Student b)
        {
            if (a == null) return false;
            if (b == null) return true;
            return a.Number.CompareTo(b.Number) <= 0;
        }
        // 6) Seřadit studenty podle čísla - seřadí studenty podle čísla (využijte metodu Sort a vhodnou porovnávací metodu)
        void SortCisla() { 
            CompareStudentsCallback compare = CompareByNumber;
            students.Sort(compare); 
        }

        bool CompareByFakulty(Student a, Student b) { 
            if (a == null) return false; 
            if (b == null) return true;
            return a.Faculty.CompareTo(b.Faculty) <= 0;
        }
        // 7) Seřadit studenty podle fakulty - seřadí studenty podle fakulty (využijte metodu Sort a vhodnou porovnávací metodu)
        void SortFakulty()
        {
            CompareStudentsCallback compare = CompareByFakulty; 
            students.Sort(compare);
        }
        // 0) Konec programu
        void exit()
        {
            return;
        }
    }
}

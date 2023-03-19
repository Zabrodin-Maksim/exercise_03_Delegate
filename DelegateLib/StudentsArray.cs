namespace DelegateLib;

// TODO: Vytvořte delegát CompareStudentsCallback, který přijme pomocí dvou parametrů dva objekty "Student?" a umožní je porovnat.
// Výsledek porovnání může být ve tvaru: "první student je menší než druhý" - true, "první student není menší než druhý" - false (bool).
// Nebo: "první student je menší než druhý" - -1, "první student je shodný s druhým" - 0, "první student je větší než druhý" - 1 (int).
// Použití jiné logiky nebo datového typu povede k neúspěchu automatických testů
public delegate bool CompareStudentsCallback(Student a, Student b);
// TODO: Dokončete třídu StudentsArray...
public class StudentsArray
{
    // Interní pole studentů.
    private Student?[] students;

    // Kapacita pole studentů.
    public int Capacity => students.Length;
    
    

    // Indexer - umožňuje pracovat s objektem podobně jako s polem (objekt[index]).
    // Tento indexer umožňuje studenty číst i zapisovat, při zadání neplatných hodnot indexů se nevykoná žádná operace nebo je vrácena hodnota null.
    public Student? this[int index]
    {
        get
        {
            if (index < 0 || index >= students.Length)
                return null;

            return students[index];
        }

        set
        {
            if (index < 0 || index >= students.Length)
                return;

            students[index] = value;
        }
    }

    // TODO: Dokončete konstruktor a inicializujte pole na danou kapacitu
    public StudentsArray(int capacity)
    {
        students = new Student[capacity];
    }

    // TODO: Dokončete řadící algoritmus a uspořádejte všechny nenulové položky v poli dle porovnávací funkce předané delegátem v parametru.
    public void Sort(CompareStudentsCallback callback)
    {
        for (int i = 0; i < Capacity; i++) {
            for(int c = 0; c < Capacity -1 ; c++) { 
                if (students[c] != null && !callback(students[c], students[c+1]) ) {
                    Student? st = students[c+1];
                    students[c+1] = students[c];
                    students[c] = st;
                }
            }
        }
    }



}

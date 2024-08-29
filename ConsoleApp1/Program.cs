using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace P
{

    class P
    {
        enum Color { White, Gray, Black };

        /// <summary>
        /// Максимальный размер матрицы
        /// </summary>
        const int N_MAX = 20;
        /// <summary>
        /// Максимальный вес грани
        /// </summary>
        const int LENGTH_MAX = 100;
        /// <summary>
        /// Вес грани, принимаемый за бесконечность
        /// </summary>
        const int INF = 1000000000; 
        /// <summary>
        /// Размер матрицы
        /// </summary>
        static int N;
        /// <summary>
        /// Граф в виде матрицы смежности
        /// </summary>
        static int[,] M;
        /// <summary>
        /// Функция, позволяющая получить целое число в заданном диапозоне без риска
        /// некорректного ввода(т.е. либо выходящего за рамки, либо не целочисленного).
        /// </summary>
        /// <param name="UBound">Верхняя граница ввода</param>
        /// <param name="LBound">Нижняя граница ввода</param>
        /// <param name="errMsg">Сообщение, которое высветится пользователю в случае некорректного ввода</param>
        /// <returns>Число либо LBound-1 в случае некорректного ввода</returns>
        static int GetCorrectUserInt(int UBound, int LBound, string errMsg)
        {
            int res;
            try
            {
                res = int.Parse(Console.ReadLine());
            } 
            catch(Exception) 
            {
                Console.WriteLine(errMsg);
                return LBound-1;
            }
            if (res > UBound || res < LBound)
            {
                Console.WriteLine(errMsg);
                return LBound - 1;
            }
            return res;
        }
        /// <summary>
        /// Функция, которая дает пользователю возможность выбрать файл из текущей директории
        /// со встроенной обработкой ошибок
        /// </summary>
        /// <returns>Путь к выбранному файлу либо null в случае ошибки в ходе выбора</returns>
        static string? GetFileInfo()
        {
            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] files = dir.GetFiles("*.txt");
            int userChoice;
            if (files.Length == 0)
            {
                Console.WriteLine("Не обнаружено файлов в директории.");
                return null;
            }
            for(int i = 0; i< files.Length; i ++)
            {
                Console.WriteLine($"{i + 1}: {files[i]} ");
            }
            Console.WriteLine($"Введите номер файла(1...{files.Length}): ");
            userChoice = GetCorrectUserInt(files.Length, 1, "Неверный номер файла.");
            if (userChoice == 0)
                return null;
            return files[userChoice - 1].Name;
        }
        /// <summary>
        /// Заполняет общий массив M и дает значение N на основе данного файла
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        static bool FillArr(string fileName)
        {
            StreamReader file = new StreamReader(fileName);
            //перенос файла в массив
            try
            {
                //читаем N, проверяем на корректность
                if (!int.TryParse(file.ReadLine(), out N))
                    throw new Exception("Количество вершин графа нечисленно");
                if (N <= 0 || N > N_MAX)
                    throw new Exception("Недопустимое количество вершин графа");
                M = new int[N, N];
                //читаем матрицу, проверяем на корректность длину строк
                for (int i = 0; i < N; i++)
                {
                    string str = file.ReadLine();
                    if (str == null)
                        throw new Exception("Одна из строк пустая");
                    string[] buf = str.Split(" ");
                    if (buf.Length != N)
                        throw new Exception("Одна из строк содержит некорректное количество вершин");
                    for (int j = 0; j < N; j++)
                    {   
                        if (!int.TryParse(buf[j], out M[i,j]))
                            throw new Exception("Некорректное значение веса");
                        if (M[i, j] > LENGTH_MAX || M[i,j] < 0)
                            throw new Exception("Значение веса вне допустимого диапозона");
                        //нулевой грани на самом деле присваиваем псевдобесконечное значение
                        if (M[i, j] == 0)
                            M[i, j] = INF;
                    }
                }
                //если еще что то осталось, кинем экспешн, т.к. это неправильный формат файла
                string ostatok = file.ReadToEnd();
                if (ostatok != null && ostatok != "")
                    throw new Exception("Файл содержит излишнюю информацию после матрицы смежности графа");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка чтения файла, неправильный формат.");
                Console.WriteLine(e.Message);
                return false;
            } finally {
            file.Close();
            }
            return true;
        }
        /// <summary>
        /// Показывает меню с возможными действиями
        /// </summary>
        /// <param name="select">Выходной параметр выбора пользователя</param>
        /// <param name="exitIndex">Выходной параметр индекса поля "Выход", т.е. последнего поля</param>
        /// <param name="graphName">Имя данного графа</param>
        static void ShowMenu(out int select, out int exitIndex, string graphName)
        {
            string[] options =
            {
                "Вывод матрицы смежности", "Вывод списка ребер", "Вывод списков смежности",
                "Определение свойств графа", "Матрица кратчайших расстояний(алгоритм Флойда - Уоршелла)",
                "Кратчайшее расстояние от вершины до остальных вершин(алгоритм Декстейры)",
                "Минимум переходов от вершины до остальных вершин (поиск в ширину)",
                "Связность графа и определение циклов (поиск в глубину)",
                "Топологическая сортировка (алгоритм Тарьяна)", "Минимальное остовое дерево (алгоритм Краскала)"
                ,"Выход"
            };
            exitIndex = options.Length;
            Console.WriteLine($"Операции над графом \"{graphName}\": ");
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}.{options[i]}.");
            }
            Console.WriteLine($"Выберите номер действия(1...{options.Length}):");
            select = GetCorrectUserInt(options.Length, 1, "Неверный номер дествия");

        }
        /// <summary>
        /// Выводит матрицу смежности данного графа
        /// </summary>
        /// <param name="graph">Данный граф</param>
        static void PrintMatrix(int[,] T)
        {
            Console.Write(" ");
            for (int i = 0; i < N; ++i)
                Console.Write("{0,6}", Convert.ToChar('A' + i));
            Console.WriteLine();
            for (int i = 0; i < T.GetLength(0); ++i)
            {
                Console.Write("{0}", Convert.ToChar('A' + i));
                for (int j = 0; j < T.GetLength(1); ++j)
                {
                    Console.Write("{0,6}", (T[i, j] == INF || T[i,j] == 0) ? "-" : T[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Выводит Матрицу Смежности графа, над которым идет работа
        /// </summary>
        static void PrintAdjacencyMatrix()
        {
            Console.WriteLine("МАТРИЦА СМЕЖНОСТИ");
            PrintMatrix(M);
        }
        /// <summary>
        /// Выводи список ребер матрицы
        /// </summary>
        static void PrintEdgesList()
        {
            List<int[]> Edges = new List<int[]>();
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    if (M[i,j] != 0 && M[i, j] < INF)   
                    {
                        int[] temp = { i, j, M[i, j] };
                        Edges.Add(temp);
                    }

            string arrow = IsDirectedGraph()? "->": "-";

            Console.WriteLine("Вершины: A-{0}", Convert.ToChar('A' + N - 1));
            foreach (int[] E in Edges) {
                Console.Write($"{Convert.ToChar('A' + E[0])}" + arrow 
                            + $"{Convert.ToChar('A' + E[1])}");
                if (IsWeightedGraph())
                    Console.Write($"({E[2]})");
                Console.WriteLine();
            }


        }
        /// <summary>
        /// Выводит граф и виде списков смежности
        /// </summary>
        static void PrintAdjacencyLists()
        {
            List<int[]>[] AdjacencyLists = new List<int[]>[N];
            for (int i = 0; i < N; ++i)
            {
                AdjacencyLists[i] = new List<int[]>();
                for (int j = 0; j < N; ++j)
                    if (M[i,j] !=0 && M[i, j] < INF)
                        AdjacencyLists[i].Add(new int[] { j, M[i, j] });
            }
            for (int i = 0; i < AdjacencyLists.Length; ++i)
            {
                Console.Write("{0}: ", Convert.ToChar('A' + i));
                foreach (int[] E in AdjacencyLists[i])
                {
                    Console.Write("{0} ", Convert.ToChar('A' + E[0]));
                    if (IsWeightedGraph())
                        Console.Write("({0}) ", E[1]);
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Проверяет наличие петель в графе
        /// </summary>
        static void PrintLoops()
        {
            bool Loops = false;
            for (int i = 0; i < N; ++i)
                if (M[i, i] < INF)
                {
                    if (!Loops)
                    {
                        Console.Write("В графе есть петли: ");
                        Loops = true;
                    }
                    Console.Write("{0}({1}) ", Convert.ToChar('A' + i), M[i, i]);
                }
            if (Loops)
                Console.WriteLine();
            else
                Console.WriteLine("В графе нет петель.");
        }
        /// <summary>
        /// Проверяет, ориентированный ли граф
        /// </summary>
        /// <returns>True, если граф ориентированный, false, если неориентированный </returns>
        static bool IsDirectedGraph()
        {
            for (int i = 0; i < N; i++)
                for (int j = i; j < N; j++)
                    if (M[i,j] != M[j,i])
                        return true;
            return false;
        }
        /// <summary>
        /// Проверяет, взвешенный ли граф
        /// </summary>
        /// <returns>True, если граф взвешенный, false, если невзвешенный </returns>
        static bool IsWeightedGraph()
        {
            for (int i = 0; i<N; i++)
                for (int j = 0; j<N; j++)
                    if (M[i,j] != 1 && M[i,j] != 0 && M[i,j] < INF)
                        return true;
            return false;
        }
        /// <summary>
        /// Выводит список свойств графа
        /// </summary>
        static void PrintGraphProperties()
        {
            PrintLoops();
            if (IsDirectedGraph())
                Console.WriteLine("Граф направленный.");
            else
                Console.WriteLine("Граф ненаправленный.");

            if (IsWeightedGraph())
                Console.WriteLine("Граф взвешенный.");
            else
                Console.WriteLine("Граф невзвешенный.");
        }
        /// <summary>
        /// Выводит матрицу кратчайших расстояний по алгоритму Флойда-Уоршелла
        /// </summary>
        static void FloydWarshell()
        {
            //создаем копию графа с нулями на главной диагонали
            int[,] R = new int[N, N];
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    R[i, j] = i == j ? 0 : M[i, j];
            //Заполняем матрицу кратчайших расстояний
            for (int k = 0; k < N; ++k)
                for (int i = 0; i < N; ++i)
                    for (int j = 0; j < N; ++j)
                        R[i, j] = Math.Min(R[i, j], R[i, k] + R[k, j]);
            //Выводим полученную матрицу
            PrintMatrix(R);
        }
        /// <summary>
        /// Получает от пользователя ввод в виде символа в корректном диапозоне
        /// </summary>
        /// <returns>Полученный символ либо -1 в случае ошибки</returns>
        static int GetVertex()       
        {                            
            char V = ' ', MaxLetter = Convert.ToChar('A' + N - 1);
            Console.Write("Введите имя исходной вершины (A-{0:C}): ",
            MaxLetter);
            try
            {
                V = char.ToUpper(char.Parse(Console.ReadLine()));
            } catch(Exception)
            {
                Console.WriteLine("Некорректный ввод");
                return -1;
            }
            if (V > MaxLetter || V < 'A')
            {
                Console.WriteLine("Ошибка - неверно указана вершина.");
                return -1;
            }

            return V - 'A';
        }
        /// <summary>
        /// Выводит кратчайшее расстояние от вершины до других на основе 
        /// данного массива дистанций
        /// </summary>
        /// <param name="D">Массив дистанций, получаемый по методу Дейкстры</param>
        static void PrintByVertices(int[] D)
        {
            for (int i = 0; i < D.Length; ++i)
                Console.Write("{0, 6}", Convert.ToChar( 'A' + i ));
            Console.WriteLine();
            foreach (int elem in D)
            {
                if (elem != INF)
                    Console.Write("{0, 6}", elem);
                else
                    Console.Write("-".PadLeft(6));
            }
                
            Console.WriteLine();
        }

        /// <summary>
        /// Выводит кратчайшее расстояние от вершины до других вершин по методу Декстейры
        /// </summary>
        static void Dijsktra()
        {
            int S = GetVertex();
            if (S == -1)
                return;

            int[] Distance = new int[N];
            bool[] Visited = new bool[N];
            for (int i = 0; i < N; ++i)
            {
                Distance[i] = INF;
                Visited[i] = false;
            }

            Distance[S] = 0;
            int MinD;
            do
            {
                MinD = INF;
                int MinV = -1;
                for (int i = 0; i < N; ++i)
                    if (Distance[i] < MinD && !Visited[i])
                    {
                        MinD = Distance[i];
                        MinV = i;
                    }
                if (MinV == -1)
                    break;
                for (int i = 0; i < N; ++i)
                    if (M[MinV, i] < INF && !Visited[i])
                        Distance[i] = Math.Min(Distance[i],
                                      Distance[MinV] + M[MinV, i]);
                Visited[MinV] = true;
            }
            while (MinD < INF);

            Console.WriteLine("Кратчайшие расстояния до вершин:");
            PrintByVertices(Distance);

            Console.WriteLine("Кратчайшие пути:");
            for (int i = 0; i < N; ++i)
                if (Distance[i] > 0 && Distance[i] < INF)
                {
                    int T = i;
                    string R = "";
                    while (T != S)
                    {
                        for (int j = 0; j < N; ++j)
                            if (M[j, T] < INF && Distance[j] == Distance[T] - M[j, T])
                            {
                                T = j;
                                R = Convert.ToChar('A' + T) + "-" + R;
                                break;
                            }
                    }
                    Console.Write(R);
                    Console.WriteLine("{0:C}", Convert.ToChar('A' + i));
                }
        }

        static void BFS()
        {
            int S;
            if ((S = GetVertex()) == -1)
                return;

            int[] D = new int[N];
            for (int i = 0; i < N; ++i)
                D[i] = INF;
            Queue<int> Q = new Queue<int>();
            int T = S;
            D[T] = 0;
            Q.Enqueue(T);
            Console.Write("+" + Convert.ToChar('A' + T) +
                          "(" + Convert.ToString(D[T]) + ") ");
            while (Q.Count > 0)
            {
                T = Q.Dequeue();
                Console.Write("-" + Convert.ToChar('A' + T) +
                              "(" + Convert.ToString(D[T]) + ") ");
                for (int i = 0; i < N; ++i)
                    if (M[T, i] < INF && D[i] == INF)
                    {
                        D[i] = D[T] + 1;
                        Q.Enqueue(i);
                        Console.Write("+" + Convert.ToChar('A' + i) +
                                      "(" + Convert.ToString(D[i]) + ") ");
                    }
            }
            Console.WriteLine();
            Console.WriteLine("Минимум переходов:");
            PrintByVertices(D);

            Console.WriteLine("Минимальные переходы:");

            for (int i = 0; i < N; ++i)
                if (D[i] > 0 && D[i] < INF)
                {
                    int tmp = i;
                    string R = "";
                    while (tmp != S)
                    {
                        for (int j = 0; j < N; ++j)
                            if (M[j, tmp] < INF && D[j] == D[tmp] - Convert.ToInt32(Convert.ToBoolean(M[j, tmp])))
                            {
                                tmp = j;
                                R = Convert.ToChar('A' + tmp) + "-" + R;
                                break;
                            }
                    }
                    Console.Write(R);
                    Console.WriteLine("{0:C}", Convert.ToChar('A' + i));
                }

        }

        static void DFS()
        {
            bool Directed = IsDirectedGraph();
            if (Directed)
                Console.WriteLine("Граф ориентированный. Связность не определяется.");

            int[] Components = new int[N];
            List<int> Cycle = new List<int>();
            Stack<int> GrayPath = new Stack<int>();
            Color[] Colors = new Color[N];
            for (int i = 0; i < N; ++i)
            {
                Components[i] = 0;
                Colors[i] = Color.White;
            }
            int ComponentsCount = 0;
            for (int i = 0; i < N; ++i)
                if (Components[i] == 0)
                {
                    ComponentsCount++;
                    GrayPath.Push(i);
                    while (GrayPath.Count > 0)
                    {
                        int V = GrayPath.Peek();
                        if (Colors[V] == Color.White)
                        {
                            Colors[V] = Color.Gray;
                            Console.Write("(" + Convert.ToChar('A' + V) + " ");
                            Components[V] = ComponentsCount;
                        }
                        bool FoundWhite = false;
                        for (int j = 0; j < N; ++j)
                        {
                            if (M[V, j] < INF && Colors[j] == Color.Gray)
                            {
                                int tsk = GrayPath.Pop();
                                int Prev = GrayPath.Count == 0 ? -1 : GrayPath.Peek();
                                GrayPath.Push(tsk);
                                if (Directed || !Directed && j != Prev)
                                    {
                                    Cycle.Clear();
                                    while (j != GrayPath.Peek())
                                        Cycle.Insert(0, GrayPath.Pop());
                                    foreach (int U in Cycle)
                                        GrayPath.Push(U);
                                    Cycle.Insert(0, j);
                                }
                            }
                            if ((M[V, j] < INF) && (Colors[j] == Color.White))
                            {
                                FoundWhite = true;
                                GrayPath.Push(j);
                                break;
                            }
                        }
                        if (!FoundWhite)
                        {
                            Console.Write(Convert.ToChar('A' + V) + ") ");
                            Colors[V] = Color.Black;
                            GrayPath.Pop();
                        }
                    }
                }
            Console.WriteLine();

            if (!Directed)
                if (ComponentsCount == 1)
                    Console.WriteLine("Граф связный.");
                else
                {
                    
                Console.WriteLine("Граф несвязный. Количество компонент: {0:D}",
                                    ComponentsCount);
                    Console.WriteLine("Принадлежность к компонентам связности:");
                    for (int i = 1; i <= ComponentsCount; ++i)
                    {
                        Console.Write("{0:D}: ", i);
                        for (int j = 0; j < N; ++j)
                            if (Components[j] == i)
                                Console.Write("{0} ", Convert.ToChar('A' + j));
                        Console.WriteLine();
                    }
                }

            if (Cycle.Count == 0)
                Console.WriteLine("В графе нет циклов.");
            else
            {
                Console.Write("В графе есть цикл: ");
                foreach (int V in Cycle)
                    Console.Write("{0} ", Convert.ToChar('A' + V));
                Console.WriteLine();
            }

        }

        static void MinSpanningTree()
        {
            bool Directed = IsDirectedGraph();
            if (Directed)
            {
                Console.WriteLine("Граф ориентированный. Построение невозможно.");
                return;
            }
            //create edges list
            List<int[]> Edges = new();
            for (int i =0; i< N; i++)
                for (int j = i+1; j<N; j++)
                    if (M[i, j] < INF)
                        Edges.Add(new int[] { i, j, M[i, j] });
            //sort edges by weight asc
            Edges = Edges.OrderBy(x => x[2]).ToList();
            int[,] MST = new int[N, N];
            int[] Components = new int[N];
            for (int i =0; i< N; i++)
                Components[i] = i+1;
            int S = 0; // min weigth
            int E = 0; //num of selected edges
            foreach (int[] Edge in Edges)
            {
                int C1 = Components[Edge[0]];
                int C2 = Components[Edge[1]];
                if (C1 != C2)
                {
                    int CMin = Math.Min(C1, C2);
                    int CMax = Math.Max(C1, C2);
                    for (int i = 0; i< N; i++)
                        if (Components[i] == CMax)
                            Components[i] = CMin;
                    MST[Edge[0], Edge[1]] = Edge[2];
                    MST[Edge[1], Edge[0]] = Edge[2];
                    E++;
                    S += Edge[2];
                    Console.WriteLine("{0}-{1} ({2})",
                            Convert.ToChar('A' + Edge[0]),
                            Convert.ToChar('A' + Edge[1]),
                            Edge[2]
                            );

                    for (int i = 1; i < Components.Length; i++)
                    {
                        bool emp = true;
                        string res = " (";
                        for (int j = 0; j < Components.Length - 1; j++)
                        {
                            if (Components[j] == i)
                            {
                                res += String.Format(" {0} ", Convert.ToChar('A' + j));
                                emp = false;
                            }
                        }
                        res += ") ";
                        if (!emp)
                            Console.Write(res);
                    }
                    Console.WriteLine();
                }
                if (E == N - 1)
                    break; 
            }
            if (E < N - 1)
            {
                Console.WriteLine("Граф несвязный. Построение минимального остового дерева невозможнo.");
                return;
            }
            Console.WriteLine("Вес минимального остового дерева : {0}", S);
            Console.WriteLine("Минимальное остовое дерево");
            PrintMatrix(MST);
        }
        static void TopologicalSort()
        {
            //будут проверять граф без ребер, но с вершинами - для него она возможна!
            int[] Order = new int[N];
            Color[] Colors = new Color[N];
            int Key = N;
            bool Success = true;
            for (int i = 0; i < N; i++)
            {
                Colors[i] = Color.White;
            }
            for (int i = 0; i < N; i++)
            {
                if (Colors[i] == Color.White)
                    DFS(i, ref Colors, ref Order, ref Key, ref Success);
            }
            if (!Success)
            {
                Console.WriteLine("Есть циклы либо граф неориентированный. Сортировка невозможна");
                return;
            }
            PrintByVertices(Order);
        }
        static void DFS(int v, ref Color[] Colors, ref int[] Order, ref int Key, ref bool Success)
        {
            if (!Success)
                return;
            Colors[v] = Color.Gray;
            for (int i = 0; i < N; i++)
            {
                if (v != i && M[v, i] < INF) //есть ребро и не петля
                    if (Colors[i] == Color.White)
                        DFS(i, ref Colors, ref Order, ref Key, ref Success);
                    else if (Colors[i] == Color.Gray)
                    {
                        Success = false;
                        break;
                    }
            }
            Colors[v] = Color.Black;
            Order[v] = Key;
            Key--;
        }
        static void Main()
        {
            string fileName = GetFileInfo();
            if (fileName == null)
                return;
            bool readSuccess = FillArr(fileName);
            if (!readSuccess)
                return;
            FillArr(fileName);
            int select, exitAdress;
            do
            {
                ShowMenu(out select, out exitAdress, fileName);
                switch (select) {
                    case 1:
                        PrintAdjacencyMatrix();
                        continue;
                    case 2:
                        PrintEdgesList();
                        continue;
                    case 3:
                        PrintAdjacencyLists();
                        continue;
                    case 4:
                        PrintGraphProperties();
                        continue;
                    case 5:
                        FloydWarshell();
                        continue;
                    case 6:
                        Dijsktra();
                        continue;
                    case 7:
                        BFS();
                        continue;
                    case 8:
                        DFS();
                        continue;
                    case 9:
                        TopologicalSort();
                        continue;
                    case 10:
                        MinSpanningTree();
                        continue;
                }
            }
            while (select != exitAdress);

            return;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

    class Program
    {
        const int N = 1000000;
        static List<int> basePrime = new List<int>();
        static int[] Nums = new int[N + 1];
        static int M = 4;
        static int cnt;
        static int current_index = 0;
        static void Init()
        {
            for (int i = 2; i < Math.Sqrt(N); i++)
            {
                if (Nums[i] == 0)
                {
                    for (int j = i + 1; j < Math.Sqrt(N); j++)
                        if (j % i == 0)
                            Nums[j] = 1;
                    basePrime.Add(i);   
                }
            }
        }
        //Модифицированный последовательный алгоритм поиска
        static void Sequential()
        {
            Init();
            for (int i = (int)(Math.Sqrt(N)); i < N + 1; i++)
                foreach (var item in basePrime)
                {
                    if (i % item == 0)
                        Nums[i] = 1;
                }
        }
        // декомпозиция по данным
        static void Parallel1() 
        {
            Init();
            Thread[] arrThr = new Thread[M];
            for (int i = 0; i < M; i++)
            {

                arrThr[i] = new Thread(ParalellThr1);
                arrThr[i].Start(i);
            }
            for (int i = 0; i < M; i++)
                arrThr[i].Join();

        }
        static void ParalellThr1(object obj)
        {
            int idx = (int)obj;
            int end;
            int Sqrt = (int)Math.Sqrt(N);
            cnt = (N - Sqrt) / M;
            int start = Sqrt + cnt * idx;
            if (idx == M - 1) end = N + 1;
            else end = start + cnt;
            for (int i = start; i < end; i++)
                foreach (var item in basePrime)
                {
                    if (i % item == 0)
                        Nums[i] = 1;
                }
        }

        //декомпозиция набора простых чисел
        static void Parallel2()
        {
            Init();
            Thread[] arrThr = new Thread[M];
            for (int i = 0; i < M; i++)
            {

                arrThr[i] = new Thread(ParalellThr2);
                arrThr[i].Start(i);
            }
            for (int i = 0; i < M; i++)
                arrThr[i].Join();


        }
        private static void ParalellThr2(object obj)
        {
            int idx = (int)obj;
            int end;
            int Sqrt = (int)Math.Sqrt(N);
            int Len = basePrime.Count;
            cnt = Len / M;
            int start = cnt * idx;
            if (idx == M - 1) end = Len;
            else end = start + cnt;
            for (int i = Sqrt; i < N + 1; i++)
                for (int j = start; j < end; j++)
                {
                    if (i % basePrime[j] == 0)
                        Nums[i] = 1;
                }
        }
        // применение пула потоков
        static void Parallel3()
        {
            Init();
            int Len = basePrime.Count;
            CountdownEvent events = new CountdownEvent(Len);
            for (int i = 0; i < Len; i++)
            {
                ThreadPool.QueueUserWorkItem(ParalellFunc3, new object[] { basePrime[i], events });
            }
            events.Wait();

        }
        // обработка всех чисел диапазона на разложимость простому числу basePrime
        private static void ParalellFunc3(object obj)
        {
            int Sqrt = (int)Math.Sqrt(N);
            int prime = (int)((object[])obj)[0];
            CountdownEvent ev = ((object[])obj)[1] as CountdownEvent;
            for (int i = Sqrt; i < N + 1; i++)
                if (i % prime == 0)
                    Nums[i] = 1;
            ev.Signal();
        }

        //последовательный перебор простых чисел
        static void Parallel4()
        {
            Init();
            Thread[] arrThr = new Thread[M];
            for (int i = 0; i < M; i++)
            {

                arrThr[i] = new Thread(ParallelFunc4);
                arrThr[i].Start();
            }
            for (int i = 0; i < M; i++)
                arrThr[i].Join();
        }
        private static void ParallelFunc4()
        {
            int current_prime;
            int Len = basePrime.Count;
            while (true)
            {
                if (current_index >= Len)
                    break;
                lock ("Critical")
                {
                    current_prime = basePrime[current_index];
                    current_index++;
                }
                for (int i = (int)Math.Sqrt(N); i < N + 1; i++)
                    if (i % current_prime == 0)
                        Nums[i] = 1;
            }
        }


        static void Main(string[] args)
        {
            DateTime Start, End;
            Start = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                Sequential();
            }
            End = DateTime.Now;
            Console.WriteLine(((End - Start).TotalMilliseconds) + " ms");
            Start = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                Parallel1();
            }
            End = DateTime.Now;
            Console.WriteLine(((End - Start).TotalMilliseconds) + " ms");

            Start = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                Parallel2();
            }
            End = DateTime.Now;
            Console.WriteLine(((End - Start).TotalMilliseconds) + " ms");

            Start = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                Parallel3();
            }
            End = DateTime.Now;
            Console.WriteLine(((End - Start).TotalMilliseconds) + " ms");

            Start = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                Parallel4();
            }
            End = DateTime.Now;
            Console.WriteLine(((End - Start).TotalMilliseconds) + " ms");
        }
    }

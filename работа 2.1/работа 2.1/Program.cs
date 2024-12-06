using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
class Program
{
    static int[] arr;
    static int[] a, b;

    static void Circ(object o)
    {
        int[] args = (int[])o;
        int iThread = args[0];
        int M = args[1];
        for (int i = iThread; i < b.Length; i += M)
        {
            for (int j = 0; j <= i; j++)
            {
                b[i] = a[i] / 100;
            }
        }
    }
    static void DivideBy(int[] arr, int k, int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            for (int dif = 0; dif < 30000; dif++)
            {
                arr[i] = arr[i] / k;
            }
        }
    }
    static void LastTask()
    {
        int[] th = new int[] { 2, 3, 4, 5, 6, 8, 10, 12, System.Environment.ProcessorCount };
        Console.WriteLine("Всего потоков в системе {0}", System.Environment.ProcessorCount);
        Console.WriteLine("N\tM\tTime");
        TimeSpan ts;
        double minPar = double.MaxValue;
        for (int n = 10; n <= 100000; n = n * 10)
        {
            a = new int[n];
            b = new int[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = i + 1;
                b[i] = 0;
            }
            
            foreach (int m in th)
            {
                Thread[] threads = new Thread[m];
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int i = 0; i < m; i++)
                {
                    threads[i] = new Thread(Circ);
                    threads[i].Start(new int[] { i, m });
                }

                for (int i = 0; i < m; i++)
                {
                    threads[i].Join();
                }

                sw.Stop();
                ts = sw.Elapsed;
                if (ts.TotalMilliseconds < minPar) minPar = ts.TotalMilliseconds;
                Console.WriteLine("{0}\t{1}\t{2}", n, m, ts.TotalMilliseconds);
                
            }
            Console.WriteLine("Min Par:\t\t{0}", minPar);
            minPar = double.MaxValue;
            Console.WriteLine();
        }
        
    }
    static void Main()
    {
        int[] th = new int[] { 2, 3, 4, 5, 6, 8, 10, 12, System.Environment.ProcessorCount };
        int k = 100;
        double minDiap = double.MaxValue;
        Console.WriteLine("Всего потоков в системе {0}", System.Environment.ProcessorCount);
        Console.WriteLine("N\tM\tTime");
        for (int n = 10; n <= 100000; n = n * 10)
        {
            arr = new int[n];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i + 1;
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DivideBy(arr, k, 0, n-1);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.Write("{0}\t1\t{1}\n", n, ts.TotalMilliseconds);
            foreach(int m in th)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = i + 1;
                }
                Thread[] threads = new Thread[(int)m];
                int range = (int)Math.Ceiling((double)n / m);
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                for (int i = 0; i < m; i++)
                {
                    int start = i * range;
                    int end = start + range - 1;
                    if (start > n - 1)
                        break;
                    if (end > n - 1)
                        end = n - 1;
                    threads[i] = new Thread(() => { DivideBy(arr, k, start, end); });
                    
                }
                for(int i = 0; i < m; i++)
                {
                    if (threads[i] != null) threads[i].Start();
                }
                for(int i = 0; i < m; i++)
                {
                    if (threads[i] != null)  threads[i].Join();
                }
                sw1.Stop();
                ts = sw1.Elapsed;
                if (ts.TotalMilliseconds < minDiap) minDiap = ts.TotalMilliseconds;
                Console.Write("{0}\t{1}\t{2}\n", n, m, ts.TotalMilliseconds);
                
            }
            Console.WriteLine("Min Diap:\t\t{0}", minDiap);
            minDiap = double.MaxValue;
            Console.WriteLine();
        }
        LastTask();
    }
    
}
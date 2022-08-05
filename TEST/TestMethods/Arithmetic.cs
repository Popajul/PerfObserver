namespace TEST.TestMethods
{
    public class Arithmetic
    {
        private readonly int _nbr;
        public Arithmetic(int nbr)
        {
            _nbr = nbr;
        }

        private bool IsEven()
        {
            if(_nbr % 2 == 0)
                return true;
            return false;
        }
        public static bool IsEven(string strnbr)
        {
            int nbr = int.Parse(strnbr);
            if (nbr % 2 == 0)
                return true;
            return false;
        }
        public  void LogIsEven()
        {
            Console.WriteLine($"IS {_nbr} even : {IsEven()}");
        }
        
        public static int Fibonnaci(uint index)
        {
            var fibo_0 = 1;
            var fibo_1 = 2;
            int fiboPivot;
            switch (index)
            {
                case 0:
                    return fibo_0;
                case 1:
                    return fibo_1;
                default:
                    break;
            }
            
            var i = 2;
            while(i < index + 1)
            {
                fiboPivot = fibo_0;
                fibo_0 = fibo_1;
                fibo_1 = fiboPivot + fibo_1;
                i++;
            }
            Console.WriteLine($"fibo_{index} = {fibo_1}");
            return fibo_1;
        }
    }
}

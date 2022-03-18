using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public enum CalType
    {
        ADD,
        SUBTRACT
    }

    public class Program
    {
        private int ret = 0;
        // 생성자 (파라미터는 두 개의 숫자로 된 string 값과 계산 타입을 받는다.)
        public Program(string a, string b, CalType type)
        {
            // string을 int형으로 변환한다.
            int aa = int.Parse(a);
            int bb = int.Parse(b);
            // 계산 타입에 따라 덧셈으로 계산한다.
            if (CalType.ADD.Equals(type))
            {
                // 덧셈 함수 호출, 결과를 맴버 필드에 저장
                ret = Add(aa, bb);
            }
            // 계산 타입에 따라 뺄셈으로 계산한다.
            else if (CalType.SUBTRACT.Equals(type))
            {
                // 뺄셈 함수 호출, 결과를 맴버 필드에 저장
                ret = Subtract(aa, bb);
            }
        }
        // 덧셈 함수
        private int Add(int a, int b)
        {
            // 두 개의 파라미터를 더한다.
            return a + b;
        }
        // 뺄셈 함수
        private int Subtract(int a, int b)
        {
            // 두 개의 파라미터를 뺀다.
            return a - b;
        }
        // 맴버 필드를 출력한다.
        public int ToResult()
        {
            return this.ret;
        }
        // 메인 함수
        static void Main(string[] args)
        {
            // 파라미터 변수를 3개를 받지 않으면 에러 발생
            if (args.Length != 3)
            {
                // Exception 발생
                throw new Exception("The usage is wrong.");
            }
            // 세번째 파라미터는 계산 타입
            CalType? type = null;
            // Add의 문자라면 덧셈을
            if (CalType.ADD.ToString().Equals(args[2], StringComparison.OrdinalIgnoreCase))
            {
                // 타입 설정
                type = CalType.ADD;
            }
            // Subtract의 문자라면 뺄셈을
            else if (CalType.SUBTRACT.ToString().Equals(args[2], StringComparison.OrdinalIgnoreCase))
            {
                // 타입 설정
                type = CalType.SUBTRACT;
            }
            // 그 외의 문자는 에러를
            else
            {
                // Exception 발생
                throw new Exception("The usage is wrong.");
            }
            // 생성자를 생성하고
            var p = new Program(args[0], args[1], type.Value);
            // 결과를 콘솔에 출력한다.
            Console.WriteLine($"Result - {p.ToResult()}");
        }
    }
}

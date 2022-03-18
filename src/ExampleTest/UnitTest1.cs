using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Example;
using System.Reflection;
using System.Linq;

namespace ExampleTest
{
    [TestClass]
    public class UnitTest1
    {
        // Reflection으로 가져올 타입 설정
        private static BindingFlags ALLBIND = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        // 라이브러리 및 실행 파일을 동적으로 읽어 올 클래스
        private readonly Assembly ASSEMBLY;
        // 클래스 typeof
        private readonly Type PROGRAM_TYPE;
        // 메소드 종류
        private readonly MethodInfo[] PROGRAM_METHODS;
        // 맴버 필드 종류
        private readonly FieldInfo[] PROGRAM_FIELDS;

        // 생성자
        public UnitTest1()
        {
            // 라이브러리 및 실행 파일을 읽어 온다.
            ASSEMBLY = Assembly.LoadFrom("Example.exe");
            // 해당 파일에서 Program 클래스를 읽어 온다.
            PROGRAM_TYPE = ASSEMBLY.GetType("Example.Program");
            // 메소드를 읽어 온다.
            PROGRAM_METHODS = PROGRAM_TYPE.GetMethods(ALLBIND);
            // 맴버 필드를 읽어 온다.
            PROGRAM_FIELDS = PROGRAM_TYPE.GetFields(ALLBIND);
        }
        // 인스턴스를 생성하는 함수
        private object NewProgram(int a, int b, CalType type)
        {
            // 접근 제한자의 관계없이 파라미터가 세개인 생성자를 찾는다.
            var construnct = PROGRAM_TYPE.GetConstructors(ALLBIND).Where(x => x.GetParameters().Length == 3).SingleOrDefault();
            // 없으면 에러
            if (construnct == null)
            {
                throw new NotImplementedException();
            }
            // 생성자를 실행하여 인스턴스를 생성한다.
            return construnct.Invoke(new object[] { a.ToString(), b.ToString(), type });
        }
        // 메소드를 읽어 오는 함수(GetMethods의 함수로 읽어 올 수 있으나 정확성과 편의성을 위해 만들었다.)
        private MethodInfo GetProgramMethods(string name, params Type[] types)
        {
            // 메소드 이름과 파라미터의 개수가 같은 것을 취득한다.
            var list = PROGRAM_METHODS.Where(x => x.Name.Equals(name) && x.GetParameters().Length == types.Length).ToList();
            if (types.Length > 0)
            {
                // 파라미터의 순서와 타입이 맞는지 체크한다.
                Func<MethodInfo, bool> check = (m) =>
                {
                    for (var i = 0; i < types.Length; i++)
                    {
                        // 파라미터의 이름이 같은 것인지 확인한다. FullName인 경우, 네임스페이스 및 클래스까지 같은 것인지 확인하는 것
                        if (!String.Equals(types[i].FullName, m.GetParameters()[i].ParameterType.FullName))
                        {
                            return false;
                        }
                    }
                    return true;
                };
                // 검색된 메소드가 복수일 경우
                foreach (var m in list)
                {
                    // 파라미터 순서와 타입이 맞는 것으로 리턴한다.
                    if (check(m))
                    {
                        return m;
                    }
                }
            }
            // 파라미터가 없을 경우.
            else
            {
                return list.SingleOrDefault();
            }
            return null;
        }
        // 맴버 변수를 취득하는 함수
        private FieldInfo GetProgramFields(string name, params Type[] types)
        {
            // 이름이 같은 것을 취득한다.
            return PROGRAM_FIELDS.Where(x => x.Name.Equals(name)).SingleOrDefault();
        }

        // 테스트 카테고리 설정(Jenkins 나 Github에서 카테고리 별 테스트 설정할 때 사용하는 옵션)
        [TestCategory("Program")]
        // 검사할 데이터 설정
        [DataRow(1, 2, 3)]
        // Unit 테스트 함수 설정
        [TestMethod]
        public void AddTest(int a, int b, int c)
        {
            // 인스턴스를 생성한다.
            var instance = NewProgram(a, b, CalType.ADD);
            // Add 함수를 가져와서 실행해서 확인한다.
            Assert.AreEqual(c, GetProgramMethods("Add", typeof(int), typeof(int)).Invoke(instance, new object[] { a, b }), "Method calculation");
            // 맴버 변수를 확인한다.
            Assert.AreEqual(c, GetProgramFields("ret").GetValue(instance), "Program field result");
            // ToResult 함수를 호출하여 나오는 값을 확인한다.
            Assert.AreEqual(c, GetProgramMethods("ToResult").Invoke(instance, null), "ToResult return value");
        }
        // 테스트 카테고리 설정(Jenkins 나 Github에서 카테고리 별 테스트 설정할 때 사용하는 옵션)
        [TestCategory("Program")]
        // 검사할 데이터 설정
        [DataRow(3, 2, 1)]
        // Unit 테스트 함수 설정
        [TestMethod]
        public void SubtractTest(int a, int b, int c)
        {
            // 인스턴스를 생성한다.
            var instance = NewProgram(a, b, CalType.SUBTRACT);
            // Subtract 함수를 가져와서 실행해서 확인한다.
            Assert.AreEqual(c, GetProgramMethods("Subtract", typeof(int), typeof(int)).Invoke(instance, new object[] { a, b }), "Method calculation");
            // 맴버 변수를 확인한다.
            Assert.AreEqual(c, GetProgramFields("ret").GetValue(instance), "Program field result");
            // ToResult 함수를 호출하여 나오는 값을 확인한다.
            Assert.AreEqual(c, GetProgramMethods("ToResult").Invoke(instance, null), "ToResult return value");
        }
        // 테스트 카테고리 설정(Jenkins 나 Github에서 카테고리 별 테스트 설정할 때 사용하는 옵션)
        [TestCategory("Program")]
        // Unit 테스트 함수 설정
        [TestMethod]
        public void MainTest()
        {
            // static 메인 함수를 찾는다.
            var main = PROGRAM_TYPE.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);
            try
            {
                // 파라미터를 넣지 않는다.
                main.Invoke(null, null);
                // 만약 Exception이 발생하지 않으면 에러다.
                Assert.Fail("When the parameter is null, the Exception is not occured.");
            }
            catch (Exception)
            {
            }
            try
            {
                // 세번째 파리미터가 계산 타입이 아닐 경우
                main.Invoke(null, new object[] { new string[] { "1", "2", "3" } });
                // 만약 Exception이 발생하지 않으면 에러다.
                Assert.Fail("When the third parameter is not calculate type, the Exception is not occured.");
            }
            catch (Exception)
            {
            }
            try
            {
                // 정상 덧셈 파라미터를 넣을 경우
                main.Invoke(null, new object[] { new string[] { "1", "2", "Add" } });
            }
            catch (Exception)
            {
                // Exception이 발생하면 에러다.
                Assert.Fail("When the parameter is [1], [2], [Add], the Exception is occured.");
            }
            try
            {
                // 정상 뺄셈 파라미터를 넣을 경우
                main.Invoke(null, new object[] { new string[] { "2", "1", "Subtract" } });
            }
            catch (Exception)
            {
                // Exception이 발생하면 에러다.
                Assert.Fail("When the parameter is [2], [1], [Subtract], the Exception is occured.");
            }
        }
    }
}

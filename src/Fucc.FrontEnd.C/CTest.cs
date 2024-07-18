using ClangSharp;
using ClangSharp.Interop;

namespace Fucc.FrontEnd.C;

public static class CTest
{
    public static void Test()
    {
#if false
        var dir = Path.GetRandomFileName();
        _ = Directory.CreateDirectory(dir);

        try
        {
            var file = new FileInfo(Path.Combine(dir, "test.c"));
            File.WriteAllText(file.FullName, "int main() { return 0; }");

            using var index = CXIndex.Create();
            using var tu = CXTranslationUnit.Parse(index, file.FullName, [], [], CXTranslationUnit_Flags.CXTranslationUnit_None);

            ;
        }
        finally
        {
            Directory.Delete(dir, true);
        }
#endif
    }
}

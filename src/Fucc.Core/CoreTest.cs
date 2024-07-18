using LLVMSharp.Interop;

namespace Fucc;

public static class CoreTest
{
    public static void Test()
    {
        var context = LLVMContextRef.Create();
        var builder = context.CreateBuilder();
        
        LLVM.LinkInMCJIT();
        LLVM.InitializeAllTargetMCs();
        LLVM.InitializeAllTargets();
        LLVM.InitializeAllTargetInfos();
        LLVM.InitializeAllAsmParsers();
        LLVM.InitializeAllAsmPrinters();

        var module = context.CreateModuleWithName("test");
        var mainFunction = module.AddFunction("main", LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, [], false));
        var entryBlock = mainFunction.AppendBasicBlock(".entry");

        builder.PositionAtEnd(entryBlock);
        builder.BuildRet(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 69, true));

        try
        {
            var target = LLVMTargetRef.GetTargetFromTriple(LLVMTargetRef.DefaultTriple);
            var machine = target.CreateTargetMachine(LLVMTargetRef.DefaultTriple, "generic", "", LLVMCodeGenOptLevel.LLVMCodeGenLevelNone, LLVMRelocMode.LLVMRelocDefault, LLVMCodeModel.LLVMCodeModelDefault);
            machine.EmitToFile(module, "test.o", LLVMCodeGenFileType.LLVMObjectFile);
        }
        finally
        {
            File.Delete("test.o");
        }
    }
}

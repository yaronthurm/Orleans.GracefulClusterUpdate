#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
[assembly: global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0")]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("Silo.Grains, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace Silo.Grains
{
    using global::Orleans.Async;
    using global::Orleans;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof (global::Silo.Grains.ICounterGrain))]
    internal class OrleansCodeGenCounterGrainReference : global::Orleans.Runtime.GrainReference, global::Silo.Grains.ICounterGrain
    {
        protected @OrleansCodeGenCounterGrainReference(global::Orleans.Runtime.GrainReference @other): base (@other)
        {
        }

        protected @OrleansCodeGenCounterGrainReference(global::System.Runtime.Serialization.SerializationInfo @info, global::System.Runtime.Serialization.StreamingContext @context): base (@info, @context)
        {
        }

        protected override global::System.Int32 InterfaceId
        {
            get
            {
                return 1908996515;
            }
        }

        public override global::System.String InterfaceName
        {
            get
            {
                return "global::Silo.Grains.ICounterGrain";
            }
        }

        public override global::System.Boolean @IsCompatible(global::System.Int32 @interfaceId)
        {
            return @interfaceId == 1908996515;
        }

        protected override global::System.String @GetMethodName(global::System.Int32 @interfaceId, global::System.Int32 @methodId)
        {
            switch (@interfaceId)
            {
                case 1908996515:
                    switch (@methodId)
                    {
                        case 2092814014:
                            return "Increment";
                        case 637921746:
                            return "GetValue";
                        case 1834577625:
                            return "Deactivate";
                        default:
                            throw new global::System.NotImplementedException("interfaceId=" + 1908996515 + ",methodId=" + @methodId);
                    }

                default:
                    throw new global::System.NotImplementedException("interfaceId=" + @interfaceId);
            }
        }

        public global::System.Threading.Tasks.Task @Increment()
        {
            return base.@InvokeMethodAsync<global::System.Object>(2092814014, null);
        }

        public global::System.Threading.Tasks.Task<global::System.Int32> @GetValue()
        {
            return base.@InvokeMethodAsync<global::System.Int32>(637921746, null);
        }

        public global::System.Threading.Tasks.Task @Deactivate()
        {
            return base.@InvokeMethodAsync<global::System.Object>(1834577625, null);
        }
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Orleans-CodeGenerator", "1.2.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute("global::Silo.Grains.ICounterGrain", 1908996515, typeof (global::Silo.Grains.ICounterGrain)), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenCounterGrainMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        public global::System.Threading.Tasks.Task<global::System.Object> @Invoke(global::Orleans.Runtime.IAddressable @grain, global::Orleans.CodeGeneration.InvokeMethodRequest @request)
        {
            global::System.Int32 interfaceId = @request.@InterfaceId;
            global::System.Int32 methodId = @request.@MethodId;
            global::System.Object[] arguments = @request.@Arguments;
            try
            {
                if (@grain == null)
                    throw new global::System.ArgumentNullException("grain");
                switch (interfaceId)
                {
                    case 1908996515:
                        switch (methodId)
                        {
                            case 2092814014:
                                return ((global::Silo.Grains.ICounterGrain)@grain).@Increment().@Box();
                            case 637921746:
                                return ((global::Silo.Grains.ICounterGrain)@grain).@GetValue().@Box();
                            case 1834577625:
                                return ((global::Silo.Grains.ICounterGrain)@grain).@Deactivate().@Box();
                            default:
                                throw new global::System.NotImplementedException("interfaceId=" + 1908996515 + ",methodId=" + methodId);
                        }

                    default:
                        throw new global::System.NotImplementedException("interfaceId=" + interfaceId);
                }
            }
            catch (global::System.Exception exception)
            {
                return global::Orleans.Async.TaskUtility.@Faulted(exception);
            }
        }

        public global::System.Int32 InterfaceId
        {
            get
            {
                return 1908996515;
            }
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 649
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif

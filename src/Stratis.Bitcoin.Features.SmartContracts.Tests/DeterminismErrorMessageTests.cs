﻿using System.Linq;
using Stratis.SmartContracts.Core.Validation;
using Stratis.SmartContracts.Core.Validation.Validators;
using Stratis.SmartContracts.Executor.Reflection;
using Stratis.SmartContracts.Executor.Reflection.Compilation;
using Xunit;

namespace Stratis.Bitcoin.Features.SmartContracts.Tests
{
    public sealed class DeterminismErrorMessageTests
    {
        private readonly ISmartContractValidator validator = new SmartContractDeterminismValidator();

        [Fact]
        public void Validate_Determinism_ErrorMessages_Simple()
        {
            string source = @"
                    using Stratis.SmartContracts;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestFloat()
                        {
                            float test = (float) 3.5;
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Float usage", result.Errors.First().ValidationType);
        }

        [Fact]
        public void Validate_Determinism_ErrorMessages_Simple_DateTime()
        {
            string source = @"
                    using Stratis.SmartContracts;
                    using System;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestDateTime()
                        {
                            var test = DateTime.Now;
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.True(result.Errors.All(e => e is WhitelistValidator.WhitelistValidationResult));
        }

        [Fact]
        public void Validate_Determinism_ErrorMessages_TwoMethods()
        {
            string source = @"
                    using Stratis.SmartContracts;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestFloat1()
                        {
                            float test = (float) 3.5;
                        }

                        public void MessageTestFloat2()
                        {
                            float test = (float) 3.5;
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count());
            Assert.Equal("Float usage", result.Errors.First().ValidationType);
            Assert.Equal("Float usage", result.Errors.Skip(1).Take(1).First().ValidationType);
        }

        [Fact]
        public void Validate_Determinism_ErrorMessages_ThreeMethods_OneValid()
        {
            string source = @"
                    using Stratis.SmartContracts;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestFloat1()
                        {
                            float test = (float) 3.5;
                        }

                        public void MessageTestFloat2()
                        {
                            var test2 = 5;
                        }

                        public void MessageTestFloat3()
                        {
                            float test = (float) 3.5;
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count());
            Assert.Equal("Float usage", result.Errors.First().ValidationType);
            Assert.Equal("Float usage", result.Errors.Skip(1).Take(1).First().ValidationType);
        }

        [Fact]
        public void Validate_Determinism_ErrorMessages_Referenced_OneLevel()
        {
            string source = @"
                    using Stratis.SmartContracts;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestValid()
                        {
                            MessageTestFloat1();
                        }

                        public void MessageTestFloat1()
                        {
                            float test = (float) 3.5;
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Float usage", result.Errors.First().ValidationType);
        }

        [Fact]
        public void Validate_Determinism_ErrorMessages_Referenced_TwoLevels()
        {
            string source = @"
                    using Stratis.SmartContracts;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestValid()
                        {
                            MessageTestValid1();
                        }

                        public void MessageTestValid1()
                        {
                            MessageTestFloat1();
                        }

                        public void MessageTestFloat1()
                        {
                            float test = (float) 3.5;
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Float usage", result.Errors.First().ValidationType);
        }

        [Fact]
        public void Validate_Determinism_ErrorMessages_Referenced_ThreeLevels()
        {
            string source = @"
                    using Stratis.SmartContracts;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestValid()
                        {
                            MessageTestValid1();
                        }

                        public void MessageTestValid1()
                        {
                            MessageTestValid2();
                        }

                        public void MessageTestValid2()
                        {
                            MessageTestFloat1();
                        }

                        public void MessageTestFloat1()
                        {
                            float test = (float) 3.5;
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Float usage", result.Errors.First().ValidationType);
        }

        [Fact]
        public void Validate_Determinism_ErrorMessages_Recursion_OneLevel()
        {
            string source = @"
                    using Stratis.SmartContracts;

                    public class MessageTest : SmartContract
                    {
                        public MessageTest(ISmartContractState smartContractState)
                            : base(smartContractState)
                        {
                        }

                        public void MessageTestValid()
                        {
                            MessageTestValid1();
                        }

                        public void MessageTestValid1()
                        {
                            float test = (float)3.5;
                            MessageTestValid();
                        }
                    }";

            SmartContractCompilationResult compilationResult = SmartContractCompiler.Compile(source);
            Assert.True(compilationResult.Success);

            IContractModuleDefinition moduleDefinition = SmartContractDecompiler.GetModuleDefinition(compilationResult.Compilation);
            SmartContractValidationResult result = this.validator.Validate(moduleDefinition.ModuleDefinition);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Float usage", result.Errors.First().ValidationType);
        }
    }
}